using System.Net;
using System.Net.Http.Json;
using Course.WebApi.Controllers;
using Course.WebApi.Features.Batches.DTOs;
using Course.WebApi.Infrastructure.Persistence;
using Course.WebApi.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Course.WebApi.Tests.SystemTests;

public class BatchesControllerSystemTests : BaseSystemTest
{
    public BatchesControllerSystemTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    private async Task<Product> SeedUniqueProductAsync(ApplicationDbContext context, string key)
    {
        var productClass = new ProductClass($"CLS-{key}", $"Class {key}", "Desc", Guid.NewGuid());
        var productCategory = new ProductCategory(null, $"CAT-{key}", $"Category {key}", "Desc", Guid.NewGuid());
        
        var product = new Product(
            sku: $"SKU-{key}",
            barcode: $"BAR-{key}",
            name: $"Product {key}",
            classId: productClass.Id,
            categoryId: productCategory.Id,
            unit: "pcs",
            defaultPurchasePrice: 10.00m,
            salePrice: 15.00m,
            attributes: null,
            userId: Guid.NewGuid()
        );

        context.ProductClasses.Add(productClass);
        context.ProductCategories.Add(productCategory);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        return product;
    }

    private async Task<Batch> SeedBatchAsync(ApplicationDbContext context, Guid productId, decimal quantity, int daysUntilExpiration)
    {
        var batch = new Batch(
            productId: productId,
            supplyItemId: Guid.NewGuid(),
            quantity: quantity,
            purchasePrice: 10.00m,
            expirationDate: DateTime.UtcNow.AddDays(daysUntilExpiration),
            userId: Guid.NewGuid()
        );

        context.Batches.Add(batch);
        await context.SaveChangesAsync();
        return batch;
    }

    [Fact]
    public async Task GetBatches_WithoutProductId_ShouldReturn200OkWithAllBatches()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("api/batches");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var batches = await response.Content.ReadFromJsonAsync<List<BatchDto>>();
        Assert.NotNull(batches);
    }

    [Fact]
    public async Task GetBatches_WithProductIdFilter_ShouldReturn200OkWithFilteredBatches()
    {
        await AuthenticateAsAdminAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var product = await SeedUniqueProductAsync(context, Guid.NewGuid().ToString()[..5]);
        await SeedBatchAsync(context, product.Id, 100m, 30);

        var response = await Client.GetAsync($"api/batches?productId={product.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var batches = await response.Content.ReadFromJsonAsync<List<BatchDto>>();
        Assert.NotNull(batches);
        Assert.All(batches, b => Assert.Equal(product.Id, b.ProductId));
    }

    [Fact]
    public async Task GetExpiringStocks_WithThreshold_ShouldReturn200OkWithExpiringBatches()
    {
        await AuthenticateAsAdminAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var product = await SeedUniqueProductAsync(context, Guid.NewGuid().ToString()[..5]);
        await SeedBatchAsync(context, product.Id, 50m, 3);

        var response = await Client.GetAsync("api/batches/expiring-stocks?daysThreshold=7");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var batches = await response.Content.ReadFromJsonAsync<List<BatchDto>>();
        Assert.NotNull(batches);
    }

    [Fact]
    public async Task GetById_ExistingBatch_ShouldReturn200Ok()
    {
        await AuthenticateAsAdminAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var product = await SeedUniqueProductAsync(context, Guid.NewGuid().ToString()[..5]);
        var batch = await SeedBatchAsync(context, product.Id, 20m, 45);

        var response = await Client.GetAsync($"api/batches/{batch.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var batchDto = await response.Content.ReadFromJsonAsync<BatchDto>();
        Assert.NotNull(batchDto);
        Assert.Equal(batch.Id, batchDto.Id);
    }

    [Fact]
    public async Task GetById_NonExistingBatch_ShouldReturn404NotFound()
    {
        await AuthenticateAsAdminAsync();
        Guid nonExistingId = Guid.NewGuid();

        var response = await Client.GetAsync($"api/batches/{nonExistingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ReduceStock_ValidRequest_ShouldReturn204NoContent()
    {
        await AuthenticateAsAdminAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var product = await SeedUniqueProductAsync(context, Guid.NewGuid().ToString()[..5]);
        var batch = await SeedBatchAsync(context, product.Id, 50m, 30);

        var input = new ReduceStockInputModel{Quantity = 10m, Version = batch.Version};

        var response = await Client.PutAsJsonAsync($"api/batches/{batch.Id}/reduce-stock", input);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ReduceStock_WithWrongVersion_ShouldReturn409Conflict()
    {
        await AuthenticateAsAdminAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var product = await SeedUniqueProductAsync(context, Guid.NewGuid().ToString()[..5]);
        var batch = await SeedBatchAsync(context, product.Id, 50m, 30);

        var input = new ReduceStockInputModel{Quantity = 5m, Version = batch.Version + 99};

        var response = await Client.PutAsJsonAsync($"api/batches/{batch.Id}/reduce-stock", input);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}