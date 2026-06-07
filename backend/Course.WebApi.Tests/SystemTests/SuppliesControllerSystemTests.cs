using System.Net;
using System.Net.Http.Json;
using Course.WebApi.Controllers;
using Xunit;

namespace Course.WebApi.Tests.SystemTests;

public class SuppliesControllerSystemTests : BaseSystemTest
{
    public SuppliesControllerSystemTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateSupply_WithValidData_ShouldReturn201Created()
    {
        await AuthenticateAsAdminAsync();
        Guid validProductId = await GetExistingProductIdAsync(); 
        
        var items = new List<CreateSupplyItemInputModel>
        {
            new (validProductId, 50m, 20.00m, 10) 
        };
        var command = new CreateSupplyInputModel("TEST", DateTime.UtcNow, items);
        var response = await Client.PostAsJsonAsync("api/supplies", command);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdSupplyId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, createdSupplyId);
    }

    [Fact]
    public async Task CreateSupply_WithNonExistingProduct_ShouldReturn400BadRequest()
    {
        await AuthenticateAsAdminAsync();
        Guid nonExistingProductId = Guid.NewGuid();
        
        var items = new List<CreateSupplyItemInputModel>
        {
            new (nonExistingProductId, 50m, 20.00m, 10) 
        };
        var command = new CreateSupplyInputModel("TEST_FAIL", DateTime.UtcNow, items);
        var response = await Client.PostAsJsonAsync("api/supplies", command);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostSupply_ExistingDraftSupply_ShouldReturn204NoContent()
    {
        await AuthenticateAsAdminAsync();
        Guid validProductId = await GetExistingProductIdAsync();
        
        var items = new List<CreateSupplyItemInputModel> 
        { 
            new (validProductId, 10m, 15.00m, 5) 
        };
        var createCommand = new CreateSupplyInputModel("Draft", DateTime.UtcNow, items);
        var createResponse = await Client.PostAsJsonAsync("api/supplies", createCommand);
        
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var supplyId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        var response = await Client.PutAsync($"api/supplies/{supplyId}/post?version=1", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PostSupply_AlreadyPosted_ShouldReturn409Conflict()
    {
        await AuthenticateAsAdminAsync();
        Guid validProductId = await GetExistingProductIdAsync();
        
        var items = new List<CreateSupplyItemInputModel> 
        { 
            new (validProductId, 10m, 15.00m, 5) 
        };
        var createCommand = new CreateSupplyInputModel("ConflictDraft", DateTime.UtcNow, items);
        var createResponse = await Client.PostAsJsonAsync("api/supplies", createCommand);
        
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var supplyId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        
        var firstPostResponse = await Client.PutAsync($"api/supplies/{supplyId}/post?version=1", null);
        Assert.Equal(HttpStatusCode.NoContent, firstPostResponse.StatusCode);

        var secondPostResponse = await Client.PutAsync($"api/supplies/{supplyId}/post?version=1", null);
        
        Assert.Equal(HttpStatusCode.Conflict, secondPostResponse.StatusCode);
    }
}