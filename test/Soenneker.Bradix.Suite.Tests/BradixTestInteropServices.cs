using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;
using Soenneker.Blazor.Utils.ModuleImport.Dtos;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;

namespace Soenneker.Bradix.Suite.Tests;

internal static class BradixTestInteropServices
{
    private const string BradixModulePath = "./_content/Soenneker.Bradix.Suite/js/bradix.js";

    public static IServiceCollection AddBradixTestInterops(this IServiceCollection services)
    {
        services.AddBradixSuiteAsScoped();

        services.RemoveAll<IModuleImportUtil>();
        services.RemoveAll<IResourceLoader>();

        services.AddScoped<IModuleImportUtil, BradixTestModuleImportUtil>();
        services.AddScoped<IResourceLoader, BradixTestResourceLoader>();

        return services;
    }
}
