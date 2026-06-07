using Serilog;

namespace Course.WebApi.Infrastructure.Logging;

public static class SerilogConfig
{
    public static void Configure(IHostBuilder host)
    {
        host.UseSerilog((context, configuration) =>
        {
            configuration
                
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .MinimumLevel.Information();
        });
    }
}