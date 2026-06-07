namespace Course.WebApi.Infrastructure.Services.Authentication;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string login, List<string> roles);
}