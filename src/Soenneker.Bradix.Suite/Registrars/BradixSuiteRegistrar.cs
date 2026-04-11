using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Blazor.Utils.ModuleImport.Registrars;

namespace Soenneker.Bradix;

/// <summary>
/// Registration for the interop and utility services.
/// </summary>
public static class BradixSuiteRegistrar
{
    public static IServiceCollection AddBradixSuiteAsScoped(this IServiceCollection services)
    {
        services.AddModuleImportUtilAsScoped();
        services.TryAddScoped<IBradixIdGenerator, BradixIdGenerator>();
        services.TryAddScoped<IBradixSuiteInterop, BradixSuiteInterop>();

        return services;
    }
}
