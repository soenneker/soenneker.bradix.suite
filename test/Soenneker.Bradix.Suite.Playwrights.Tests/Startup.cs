using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Soenneker.Playwrights.Installation.Registrars;
using Soenneker.Utils.Dotnet.Registrars;
using Soenneker.Utils.HttpClientCache.Registrar;
using Soenneker.Utils.Network.Registrars;
using Soenneker.Utils.Test;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.SetupIoC();
    }

    public static IServiceCollection SetupIoC(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSimpleConsole(options =>
            {
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
            builder.SetMinimumLevel(LogLevel.Information);
        });
        services.AddPlaywrightInstallationUtilAsSingleton();
        services.AddNetworkUtilAsSingleton();
        services.AddDotnetUtilAsSingleton();

        IConfiguration configuration = TestUtil.BuildConfig();
        services.AddSingleton(configuration);

        services.AddHttpClientCacheAsSingleton();

        services.AddSingleton<FixtureRuntime>();
        services.AddSingleton<FixtureOrchestrator>();

        return services;
    }
}