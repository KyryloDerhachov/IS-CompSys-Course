using System.Net;
using System.Net.Http.Json;
using Course.WebApi.Controllers;
using Course.WebApi.Features.Classification.DTOs;
using Course.WebApi.Infrastructure.Persistence;
using Course.WebApi.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Course.WebApi.Tests.SystemTests;

public class ProductCategoriesControllerSystemTests : BaseSystemTest
{
    public ProductCategoriesControllerSystemTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    private async Task<ProductCategory> SeedRootCategoryAsync(ApplicationDbContext context, string key)
    {
        var category = new ProductCategory(
            parentId: null,
            code: $"CAT-{key}",
            name: $"Category {key}",
            description: "Initial Description",
            userId: Guid.NewGuid()
        );
        
        context.ProductCategories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    [Fact]
    public async Task GetAll_WhenCalled_ShouldReturn200OkWithCategories()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("api/product-categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var categories = await response.Content.ReadFromJsonAsync<List<ProductCategoryDto>>();
        Assert.NotNull(categories);
        Assert.NotEmpty(categories);
    }

    [Fact]
    public async Task GetById_ExistingId_ShouldReturn200Ok()
    {
        await AuthenticateAsAdminAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var existingCategory = await SeedRootCategoryAsync(context, Guid.NewGuid().ToString()[..5]);

        var response = await Client.GetAsync($"api/product-categories/{existingCategory.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var categoryDto = await response.Content.ReadFromJsonAsync<ProductCategoryDto>();
        Assert.NotNull(categoryDto);
        Assert.Equal(existingCategory.Id, categoryDto.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ShouldReturn404NotFound()
    {
        await AuthenticateAsAdminAsync();
        Guid nonExistingId = Guid.NewGuid();

        var response = await Client.GetAsync($"api/product-categories/{nonExistingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_RootCategory_ShouldReturn201Created()
    {
        await AuthenticateAsAdminAsync();
        var suffix = Guid.NewGuid().ToString()[..5];
        var input = new CreateCategoryInput(
            ParentId: null,
            Code: $"NEW-{suffix}",
            Name: $"New Category {suffix}",
            Description: "Root level category description"
        );

        var response = await Client.PostAsJsonAsync("api/product-categories", input);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, createdId);
    }

    [Fact]
    public async Task Create_SubCategory_ShouldReturn201Created()
    {
        await AuthenticateAsAdminAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var rootCategory = await SeedRootCategoryAsync(context, Guid.NewGuid().ToString()[..5]);
        
        var suffix = Guid.NewGuid().ToString()[..5];
        var input = new CreateCategoryInput(
            ParentId: rootCategory.Id,
            Code: $"SUB-{suffix}",
            Name: $"Sub Category {suffix}",
            Description: "Child level category description"
        );

        var response = await Client.PostAsJsonAsync("api/product-categories", input);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, createdId);
    }

    [Fact]
    public async Task UpdateDescription_ExistingCategory_ShouldReturn204NoContent()
    {
        await AuthenticateAsAdminAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var category = await SeedRootCategoryAsync(context, Guid.NewGuid().ToString()[..5]);

        var input = new UpdateDescriptionInput("Brand new updated description", category.Version);

        var response = await Client.PutAsJsonAsync($"api/product-categories/{category.Id}/description", input);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Deactivate_And_Activate_ProductCategory_ShouldWorkCorrectly()
    {
        await AuthenticateAsAdminAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var category = await SeedRootCategoryAsync(context, Guid.NewGuid().ToString()[..5]);

        var deactivateResponse = await Client.PutAsync($"api/product-categories/{category.Id}/deactivate?version={category.Version}", null);
        Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);

        var activateResponse = await Client.PutAsync($"api/product-categories/{category.Id}/activate?version={category.Version + 1}", null);
        Assert.Equal(HttpStatusCode.NoContent, activateResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateDescription_WithWrongVersion_ShouldReturn409Conflict()
    {
        await AuthenticateAsAdminAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var category = await SeedRootCategoryAsync(context, Guid.NewGuid().ToString()[..5]);

        var input = new UpdateDescriptionInput("Conflict description", category.Version + 99);

        var response = await Client.PutAsJsonAsync($"api/product-categories/{category.Id}/description", input);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}