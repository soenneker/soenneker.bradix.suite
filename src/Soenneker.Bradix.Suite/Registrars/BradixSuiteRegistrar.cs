using Microsoft.Extensions.DependencyInjection;
using Soenneker.Blazor.Utils.ResourceLoader.Registrars;

namespace Soenneker.Bradix;

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
