using System.Net;
using System.Net.Http.Json;
using Course.WebApi.Controllers;
using Course.WebApi.Features.Classification.DTOs;
using Course.WebApi.Infrastructure.Persistence;
using Course.WebApi.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Course.WebApi.Tests.SystemTests;

public class ProductClassesControllerSystemTests : BaseSystemTest
{
    public ProductClassesControllerSystemTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    private async Task<ProductClass> SeedProductClassAsync(ApplicationDbContext context, string key)
    {
        var productClass = new ProductClass(
            code: $"CLS-{key}",
            name: $"Class {key}",
            description: "Initial Description",
            userId: Guid.NewGuid()
        );

        context.ProductClasses.Add(productClass);
        await context.SaveChangesAsync();
        return productClass;
    }

    [Fact]
    public async Task GetAll_WhenCalled_ShouldReturn200OkWithClasses()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.GetAsync("api/product-classes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var classes = await response.Content.ReadFromJsonAsync<List<ProductClassDto>>();
        Assert.NotNull(classes);
        Assert.NotEmpty(classes);
    }

    [Fact]
    public async Task GetById_ExistingId_ShouldReturn200Ok()
    {
        await AuthenticateAsAdminAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var existingClass = await SeedProductClassAsync(context, Guid.NewGuid().ToString()[..5]);

        var response = await Client.GetAsync($"api/product-classes/{existingClass.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var classDto = await response.Content.ReadFromJsonAsync<ProductClassDto>();
        Assert.NotNull(classDto);
        Assert.Equal(existingClass.Id, classDto.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ShouldReturn404NotFound()
    {
        await AuthenticateAsAdminAsync();
        Guid nonExistingId = Guid.NewGuid();

        var response = await Client.GetAsync($"api/product-classes/{nonExistingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidClass_ShouldReturn201Created()
    {
        await AuthenticateAsAdminAsync();
        var suffix = Guid.NewGuid().ToString()[..5];
        var input = new CreateClassInput(
            Code: $"NEW-{suffix}",
            Name: $"New Class {suffix}",
            Description: "Brand new product class description"
        );

        var response = await Client.PostAsJsonAsync("api/product-classes", input);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, createdId);
    }

    [Fact]
    public async Task UpdateDescription_ExistingClass_ShouldReturn204NoContent()
    {
        await AuthenticateAsAdminAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var productClass = await SeedProductClassAsync(context, Guid.NewGuid().ToString()[..5]);

        var input = new UpdateDescriptionInput("Updated description details", productClass.Version);

        var response = await Client.PutAsJsonAsync($"api/product-classes/{productClass.Id}/description", input);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Deactivate_And_Activate_ProductClass_ShouldWorkCorrectly()
    {
        await AuthenticateAsAdminAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var productClass = await SeedProductClassAsync(context, Guid.NewGuid().ToString()[..5]);

        var deactivateResponse = await Client.PutAsync($"api/product-classes/{productClass.Id}/deactivate?version={productClass.Version}", null);
        Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);

        var activateResponse = await Client.PutAsync($"api/product-classes/{productClass.Id}/activate?version={productClass.Version + 1}", null);
        Assert.Equal(HttpStatusCode.NoContent, activateResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateDescription_WithWrongVersion_ShouldReturn409Conflict()
    {
        await AuthenticateAsAdminAsync();
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var productClass = await SeedProductClassAsync(context, Guid.NewGuid().ToString()[..5]);

        var input = new UpdateDescriptionInput("Conflict version description", productClass.Version + 99);

        var response = await Client.PutAsJsonAsync($"api/product-classes/{productClass.Id}/description", input);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}