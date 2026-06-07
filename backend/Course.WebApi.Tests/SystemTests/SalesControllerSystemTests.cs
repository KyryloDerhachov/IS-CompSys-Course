using System.Net;
using System.Net.Http.Json;
using Course.WebApi.Controllers;
using Course.WebApi.Features.Sales.DTOs;
using Xunit;

namespace Course.WebApi.Tests.SystemTests;

public class SalesControllerSystemTests : BaseSystemTest
{
    public SalesControllerSystemTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    private async Task PrepareStockForProductAsync(Guid productId, decimal quantity)
    {
        var supplyItems = new List<object> 
        { 
            new 
            { 
                productId = productId, 
                quantity = quantity, 
                purchasePrice = 1.00m,
                shelfLifeDays = 30
            } 
        };
        
        var supplyCommand = new 
        { 
            supplierName = $"Supplier-{Guid.NewGuid().ToString()[..5]}", 
            supplyDate = DateTime.UtcNow, 
            items = supplyItems 
        };
        
        var supplyResponse = await Client.PostAsJsonAsync("api/supplies", supplyCommand);
        supplyResponse.EnsureSuccessStatusCode();
        
        var supplyId = await supplyResponse.Content.ReadFromJsonAsync<Guid>();
        var postResponse = await Client.PutAsync($"api/supplies/{supplyId}/post?version=1", null);
        postResponse.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task CreateReceipt_WithValidData_ShouldReturn201Created()
    {
        await AuthenticateAsAdminAsync();
        Guid validProductId = await GetExistingProductIdAsync();
        await PrepareStockForProductAsync(validProductId, 50m);
        
        var createCommand = new CreateReceiptInputModel(
            new List<SaleItemInputModel> { new(validProductId, 1m, 150.00m) }
        );
        
        var response = await Client.PostAsJsonAsync("api/sales/receipts", createCommand);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdReceiptId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, createdReceiptId);
    }

    [Fact]
    public async Task GetReceiptById_ExistingReceipt_ShouldReturn200Ok()
    {
        await AuthenticateAsAdminAsync();
        Guid validProductId = await GetExistingProductIdAsync();
        await PrepareStockForProductAsync(validProductId, 50m);

        var createCommand = new CreateReceiptInputModel(
            new List<SaleItemInputModel> { new(validProductId, 1m, 150.00m) }
        );
        
        var createResponse = await Client.PostAsJsonAsync("api/sales/receipts", createCommand);
        createResponse.EnsureSuccessStatusCode();
        var receiptId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var response = await Client.GetAsync($"api/sales/receipts/{receiptId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var receiptDto = await response.Content.ReadFromJsonAsync<ReceiptDto>();
        Assert.NotNull(receiptDto);
        Assert.Equal(receiptId, receiptDto.Id);
    }

    [Fact]
    public async Task ProcessReturn_ValidItems_ShouldReturn200Ok()
    {
        await AuthenticateAsAdminAsync();
        Guid validProductId = await GetExistingProductIdAsync();
        await PrepareStockForProductAsync(validProductId, 50m);

        var createCommand = new CreateReceiptInputModel(
            new List<SaleItemInputModel> { new(validProductId, 2m, 150.00m) }
        );
        
        var createResponse = await Client.PostAsJsonAsync("api/sales/receipts", createCommand);
        createResponse.EnsureSuccessStatusCode();
        var receiptId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var returnCommand = new CreateReturnInputModel(
            new List<ReturnItemInputModel> { new(validProductId, 2m) }
        );

        var response = await Client.PostAsJsonAsync($"api/sales/receipts/{receiptId}/returns", returnCommand);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var returnTxId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, returnTxId);
    }

    [Fact]
    public async Task GetHistory_WhenCalled_ShouldReturn200Ok()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("api/sales/receipts");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var history = await response.Content.ReadFromJsonAsync<List<ReceiptDto>>();
        Assert.NotNull(history);
    }

    [Fact]
    public async Task GetReturnsHistory_WhenCalled_ShouldReturn200Ok()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("api/sales/returns");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var returnsHistory = await response.Content.ReadFromJsonAsync<List<ReturnTransactionDto>>();
        Assert.NotNull(returnsHistory);
    }
}