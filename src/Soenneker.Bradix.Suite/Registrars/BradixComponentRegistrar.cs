using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Bradix.Suite.Abstract;
using Soenneker.Blazor.Utils.ResourceLoader.Registrars;

namespace Soenneker.Bradix.Suite.Registrars;

/// <summary>
/// Registration for the interop and utility services.
/// </summary>
public static class BradixComponentRegistrar
{
    /// <summary>
    /// Adds <see cref="ISuiteInterop"/> and <see cref="IBradixComponent"/> as scoped services.
    /// </summary>
    public static IServiceCollection AddBradixComponentAsScoped(this IServiceCollection services)
    {
        services.AddResourceLoaderAsScoped();

        return services;
    }
}
