using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;
using Soenneker.Blazor.Utils.ModuleImport.Dtos;

namespace Soenneker.Bradix.Suite.Tests;

internal sealed class BradixTestModuleImportUtil : IModuleImportUtil
{
    private const string BradixModulePath = "./_content/Soenneker.Bradix.Suite/js/bradix.js";
    private readonly IJSRuntime _jsRuntime;

    public BradixTestModuleImportUtil(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async ValueTask<ModuleImportItem> GetContentModule(string path, CancellationToken cancellationToken = default)
    {
        IJSObjectReference reference = await GetContentModuleReference(path, cancellationToken);
        var item = new ModuleImportItem { ScriptReference = reference };
        item.ModuleLoadedTcs.TrySetResult(true);
        return item;
    }

    public async ValueTask<ModuleImportItem> GetExternalModule(string url, CancellationToken cancellationToken = default)
    {
        IJSObjectReference reference = await GetExternalModuleReference(url, cancellationToken);
        var item = new ModuleImportItem { ScriptReference = reference };
        item.ModuleLoadedTcs.TrySetResult(true);
        return item;
    }

    public ValueTask<IJSObjectReference> GetContentModuleReference(string path, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeAsync<IJSObjectReference>("import", cancellationToken, BradixModulePath);
    }

    public ValueTask<IJSObjectReference> GetExternalModuleReference(string url, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeAsync<IJSObjectReference>("import", cancellationToken, BradixModulePath);
    }

    public ValueTask<bool> DisposeContentModule(string name)
    {
        return ValueTask.FromResult(true);
    }

    public ValueTask<bool> DisposeExternalModule(string url)
    {
        return ValueTask.FromResult(true);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
