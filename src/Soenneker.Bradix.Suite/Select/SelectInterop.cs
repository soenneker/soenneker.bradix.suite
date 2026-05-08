using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;

namespace Soenneker.Bradix;

public sealed class SelectInterop : ISelectInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;

    private const string _modulePath = "./_content/Soenneker.Bradix.Suite/js/bradix/select.js";

    public SelectInterop(IModuleImportUtil moduleImportUtil)
    {
        _moduleImportUtil = moduleImportUtil;
    }

    public ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default)
    {
        return _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask RegisterSelectItemAlignedPosition(ElementReference wrapper, ElementReference content, ElementReference viewport, ElementReference trigger,
        ElementReference valueNode, ElementReference selectedItem, ElementReference selectedItemText, string dir, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerSelectItemAlignedPosition", cancellationToken, wrapper, content, viewport, trigger, valueNode, selectedItem,
            selectedItemText, dir);
    }

    public async ValueTask UpdateSelectItemAlignedPosition(ElementReference wrapper, ElementReference content, ElementReference viewport, ElementReference trigger,
        ElementReference valueNode, ElementReference selectedItem, ElementReference selectedItemText, string dir, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("updateSelectItemAlignedPosition", cancellationToken, wrapper, content, viewport, trigger, valueNode, selectedItem,
            selectedItemText, dir);
    }

    public async ValueTask UnregisterSelectItemAlignedPosition(ElementReference wrapper, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterSelectItemAlignedPosition", cancellationToken, wrapper);
    }

    public async ValueTask RegisterSelectViewport(ElementReference viewport, ElementReference content, ElementReference wrapper,
        DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerSelectViewport", cancellationToken, viewport, content, wrapper, dotNetReference);
    }

    public async ValueTask UnregisterSelectViewport(ElementReference viewport, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterSelectViewport", cancellationToken, viewport);
    }

    public async ValueTask RegisterSelectContentKeyboard(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerSelectContentKeyboard", cancellationToken, content, dotNetReference);
    }

    public async ValueTask UnregisterSelectContentKeyboard(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterSelectContentKeyboard", cancellationToken, content);
    }

    public async ValueTask ScrollSelectViewportByItem(ElementReference viewport, ElementReference item, bool upward, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("scrollSelectViewportByItem", cancellationToken, viewport, item, upward);
    }

    public async ValueTask RegisterSelectContentPointerTracker(ElementReference content, DotNetObjectReference<object> dotNetReference, double pageX, double pageY,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerSelectContentPointerTracker", cancellationToken, content, dotNetReference, pageX, pageY);
    }

    public async ValueTask UnregisterSelectContentPointerTracker(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterSelectContentPointerTracker", cancellationToken, content);
    }

    public async ValueTask RegisterSelectWindowDismiss(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerSelectWindowDismiss", cancellationToken, content, dotNetReference);
    }

    public async ValueTask UnregisterSelectWindowDismiss(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterSelectWindowDismiss", cancellationToken, content);
    }

    public async ValueTask<string?> GetSelectOptionValueAtPoint(double clientX, double clientY, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        return await module.InvokeAsync<string?>("getSelectOptionValueAtPoint", cancellationToken, clientX, clientY);
    }

    public async ValueTask DisposeAsync()
    {
        await _moduleImportUtil.DisposeContentModule(_modulePath);
    }
}
