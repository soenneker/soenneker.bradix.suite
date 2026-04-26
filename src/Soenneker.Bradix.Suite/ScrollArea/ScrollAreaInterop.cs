using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;

namespace Soenneker.Bradix;

public sealed class ScrollAreaInterop : IScrollAreaInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;

    private const string _modulePath = "./_content/Soenneker.Bradix.Suite/js/bradix/scrollArea.js";

    public ScrollAreaInterop(IModuleImportUtil moduleImportUtil)
    {
        _moduleImportUtil = moduleImportUtil;
    }

    public ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default)
    {
        return _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask RegisterScrollAreaRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerScrollAreaRoot", cancellationToken, element, dotNetReference);
    }

    public async ValueTask UnregisterScrollAreaRoot(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterScrollAreaRoot", cancellationToken, element);
    }

    public async ValueTask RegisterScrollAreaViewport(ElementReference viewport, ElementReference content, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerScrollAreaViewport", cancellationToken, viewport, content, dotNetReference);
    }

    public async ValueTask UnregisterScrollAreaViewport(ElementReference viewport, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterScrollAreaViewport", cancellationToken, viewport);
    }

    public async ValueTask RegisterScrollAreaScrollbar(ElementReference scrollbar, ElementReference thumb, ElementReference viewport, string orientation, string dir, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerScrollAreaScrollbar", cancellationToken, scrollbar, thumb, viewport, orientation, dir, dotNetReference);
    }

    public async ValueTask UnregisterScrollAreaScrollbar(ElementReference scrollbar, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterScrollAreaScrollbar", cancellationToken, scrollbar);
    }

    public async ValueTask DisposeAsync()
    {
        await _moduleImportUtil.DisposeContentModule(_modulePath);
    }
}