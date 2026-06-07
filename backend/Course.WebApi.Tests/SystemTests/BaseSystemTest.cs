using System.Net.Http.Headers;
using System.Net.Http.Json;
using Course.WebApi.Features.Users.DTOs;
using Course.WebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Course.WebApi.Tests;

public abstract class BaseSystemTest : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory Factory;

    protected BaseSystemTest(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected async Task AuthenticateAsAdminAsync()
    {
        var loginCommand = new { Login = "admin", Password = "AdminPassword123" };
        var response = await Client.PostAsJsonAsync("api/users/login", loginCommand);
        response.EnsureSuccessStatusCode();

        var authData = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authData?.Token);
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", authData.Token);
    }

    protected async Task<Guid> GetExistingProductIdAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var product = await context.Products.FirstOrDefaultAsync();
        
        if (product == null)
            throw new Exception("The database is empty; DbInitializer failed!");
            
        return product.Id;
    }
}