namespace Course.WebApi.Infrastructure.Services.Authentication;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}