using Microsoft.Extensions.DependencyInjection;
using Soenneker.Blazor.Utils.ResourceLoader.Registrars;
using Soenneker.Bradix.Suite.Abstract;
using Soenneker.Bradix.Suite.Id;
using Soenneker.Bradix.Suite.Interop;

namespace Soenneker.Bradix.Suite.Registrars;

/// <summary>
/// Registration for the interop and utility services.
/// </summary>
public static class BradixSuiteRegistrar
{
    public static IServiceCollection AddBradixSuiteAsScoped(this IServiceCollection services)
    {
        services.AddResourceLoaderAsScoped();
        services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
        services.AddScoped<BradixSuiteInterop>();
        services.AddScoped<ISuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
        services.AddScoped<IBradixComponent, BradixComponent>();

        return services;
    }
}
