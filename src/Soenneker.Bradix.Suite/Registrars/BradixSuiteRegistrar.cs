using Microsoft.Extensions.DependencyInjection;
using Soenneker.Blazor.Utils.ResourceLoader.Registrars;

namespace Soenneker.Bradix.Suite.Registrars;

/// <summary>
/// Registration for the interop and utility services.
/// </summary>
public static class BradixSuiteRegistrar
{
    public static IServiceCollection AddBradixSuiteAsScoped(this IServiceCollection services)
    {
        services.AddResourceLoaderAsScoped();

        return services;
    }
}
