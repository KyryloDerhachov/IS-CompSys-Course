using System.Net;
using System.Net.Http.Json;
using Course.WebApi.Features.Users.Commands;
using Course.WebApi.Features.Users.DTOs;
using Xunit;

namespace Course.WebApi.Tests.SystemTests;

public class UsersControllerSystemTests : BaseSystemTest
{
    public UsersControllerSystemTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateUser_WithValidData_ShouldReturn200OkAndGuid()
    {
        await AuthenticateAsAdminAsync();
        var command = new CreateUserCommand(
            "ivan_cashier_unique", 
            "ivan@store.com",
            "Іван",
            "Петренко",
            "SecurePassword123",
            new List<Guid>()
        );
        
        var response = await Client.PostAsJsonAsync("api/users", command);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var createdUserId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, createdUserId);
    }

    [Fact]
    public async Task CreateUser_WithoutToken_ShouldReturn401Unauthorized()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var command = new CreateUserCommand(
            "fail_user",
            "fail@store.com",
            "Test",
            "Test",
            "SecurePassword123",
            new List<Guid>()
        );
        
        var response = await Client.PostAsJsonAsync("api/users", command);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_DuplicateLogin_ShouldReturn400BadRequest()
    {
        await AuthenticateAsAdminAsync();
        var command = new CreateUserCommand(
            "admin", 
            "admin_new@store.com",
            "Дублікат",
            "Адміна",
            "SecurePassword123",
            new List<Guid>()
        );
        
        var response = await Client.PostAsJsonAsync("api/users", command);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetUserById_ExistingUser_ShouldReturn200OkWithUserDto()
    {
        await AuthenticateAsAdminAsync();
        var createCommand = new CreateUserCommand(
            "get_by_id_user",
            "getbyid@store.com",
            "Тест",
            "Айді",
            "SecurePassword123",
            new List<Guid>()
        );
        var createResponse = await Client.PostAsJsonAsync("api/users", createCommand);
        var createdUserId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        
        var response = await Client.GetAsync($"api/users/{createdUserId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(userDto);
        Assert.Equal("get_by_id_user", userDto.Login);
    }

    [Fact]
    public async Task GetAllUsers_WhenCalled_ShouldReturn200OkWithList()
    {
        await AuthenticateAsAdminAsync();
        
        var response = await Client.GetAsync("api/users?includeInactive=false");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var allUsers = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        Assert.NotNull(allUsers);
        Assert.Contains(allUsers, u => u.Login == "admin");
    }

    [Fact]
    public async Task DeactivateUser_ExistingUser_ShouldReturn204NoContent()
    {
        await AuthenticateAsAdminAsync();
        var createCommand = new CreateUserCommand(
            "user_for_deactivate",
            "deactivate@store.com",
            "Тест",
            "Деактивація",
            "SecurePassword123",
            new List<Guid>()
        );
        var createResponse = await Client.PostAsJsonAsync("api/users", createCommand);
        var createdUserId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var response = await Client.PutAsync($"api/users/{createdUserId}/deactivate", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}