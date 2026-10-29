using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.Extensions.IJSObjectReferences;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;

namespace Soenneker.Bradix;

public sealed class DismissableLayerInterop : IDismissableLayerInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;

    private const string _modulePath = "./_content/Soenneker.Bradix.Suite/js/bradix/dismissableLayer.js";

    public DismissableLayerInterop(IModuleImportUtil moduleImportUtil)
    {
        _moduleImportUtil = moduleImportUtil;
    }

    public async ValueTask RegisterDismissableLayer(ElementReference element, object dotNetReference, bool disableOutsidePointerEvents, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("registerDismissableLayer", cancellationToken, element, dotNetReference, (disableOutsidePointerEvents ? BradixAttributeValues.True : BradixAttributeValues.False), element.Id);
    }

    public ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default)
    {
        return _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask UpdateDismissableLayer(ElementReference element, bool disableOutsidePointerEvents, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("updateDismissableLayer", cancellationToken, element, (disableOutsidePointerEvents ? BradixAttributeValues.True : BradixAttributeValues.False));
    }

    public async ValueTask UnregisterDismissableLayer(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("unregisterDismissableLayer", cancellationToken, element, element.Id);
    }

    public async ValueTask RegisterDismissableLayerBranch(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("registerDismissableLayerBranch", cancellationToken, element);
    }

    public async ValueTask UnregisterDismissableLayerBranch(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("unregisterDismissableLayerBranch", cancellationToken, element);
    }

    /// <summary>
    /// Asynchronously releases resources used by the current instance.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await _moduleImportUtil.DisposeContentModule(_modulePath);
    }
}