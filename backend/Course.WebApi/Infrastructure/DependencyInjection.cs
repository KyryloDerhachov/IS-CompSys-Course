
using Course.WebApi.Application.Interfaces;
using Course.WebApi.Infrastructure.Persistence;
using Course.WebApi.Infrastructure.Services.Authentication;

namespace Course.WebApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IRoleRepository, EfRoleRepository>();
        services.AddScoped<IProductClassRepository, EfProductClassRepository>();
        services.AddScoped<IProductCategoryRepository, EfProductCategoryRepository>();
        services.AddScoped<IProductRepository, EfProductRepository>();
        services.AddScoped<ISupplyRepository, EfSupplyRepository>();
        services.AddScoped<IBatchRepository,EfBatchRepository>();
        services.AddScoped<IReceiptRepository, EfReceiptRepository>();
        services.AddScoped<IReturnRepository, EfReturnRepository>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        return services;
    }
}