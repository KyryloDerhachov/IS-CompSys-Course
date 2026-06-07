using System.Net;
using System.Net.Http.Json;
using Course.WebApi.Features.Products.Commands;
using Course.WebApi.Features.Products.DTOs;
using Course.WebApi.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Course.WebApi.Tests.SystemTests;

public class ProductsControllerSystemTests : BaseSystemTest
{
    public ProductsControllerSystemTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAll_WhenCalled_ShouldReturn200OkWithProducts()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        Assert.NotNull(products);
        Assert.NotEmpty(products);
    }

    [Fact]
    public async Task GetById_ExistingId_ShouldReturn200Ok()
    {
        await AuthenticateAsAdminAsync();
        Guid existingProductId = await GetExistingProductIdAsync();

        var response = await Client.GetAsync($"api/products/{existingProductId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);
        Assert.Equal(existingProductId, product.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ShouldReturn404NotFound()
    {
        await AuthenticateAsAdminAsync();
        Guid nonExistingId = Guid.NewGuid();

        var response = await Client.GetAsync($"api/products/{nonExistingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturn201Created()
    {
        await AuthenticateAsAdminAsync();
        
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = context.ProductClasses.First().Id;
        var categoryId = context.ProductCategories.First().Id;

        var command = new CreateProductCommand(
            Sku: "ITEM-NEW-999",
            Barcode: "1111111111111",
            Name: "New Test Product",
            ClassId: classId,
            CategoryId: categoryId,
            Unit: "unit",
            DefaultShelfLifeDays: 30,
            DefaultPurchasePrice: 10.00m,
            DefaultSalePrice: 20.00m,
            Attributes: new Dictionary<string, string> { { "Type", "SystemTest" } },
            CurrentUserId: null
        );

        var response = await Client.PostAsJsonAsync("api/products", command);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, createdId);
    }

    [Fact]
    public async Task Update_ExistingProduct_ShouldReturn204NoContent()
    {
        await AuthenticateAsAdminAsync();
        
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var product = context.Products.First();

        var input = new UpdateProductDTO
        {
            Name = "Updated Product Name",
            ClassId = product.ClassId,
            CategoryId = product.CategoryId,
            Unit = "kg",
            Version = 1,
            Attributes = new Dictionary<string, string> { { "Status", "Updated" } }
        };

        var response = await Client.PutAsJsonAsync($"api/products/{product.Id}", input);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Deactivate_And_Activate_Product_ShouldWorkCorrectly()
    {
        await AuthenticateAsAdminAsync();

        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var product = context.Products.First();

        var deactivateResponse = await Client.PutAsync($"api/products/{product.Id}/deactivate?version=1", null);
        Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);

        var activateResponse = await Client.PutAsync($"api/products/{product.Id}/activate?version=2", null);
        Assert.Equal(HttpStatusCode.NoContent, activateResponse.StatusCode);
    }
}