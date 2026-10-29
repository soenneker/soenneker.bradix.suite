using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.Extensions.IJSObjectReferences;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;

namespace Soenneker.Bradix;

/// <inheritdoc cref="ISelectInterop"/>
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
        await module.InvokeVoidWithContextAsync("registerSelectItemAlignedPosition", cancellationToken, wrapper, content, viewport, trigger, valueNode, selectedItem,
            selectedItemText, dir, wrapper.Id);
    }

    public async ValueTask UpdateSelectItemAlignedPosition(ElementReference wrapper, ElementReference content, ElementReference viewport, ElementReference trigger,
        ElementReference valueNode, ElementReference selectedItem, ElementReference selectedItemText, string dir, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("updateSelectItemAlignedPosition", cancellationToken, wrapper, content, viewport, trigger, valueNode, selectedItem,
            selectedItemText, dir);
    }

    public async ValueTask UnregisterSelectItemAlignedPosition(ElementReference wrapper, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("unregisterSelectItemAlignedPosition", cancellationToken, wrapper, wrapper.Id);
    }

    public async ValueTask RegisterSelectViewport(ElementReference viewport, ElementReference content, ElementReference wrapper,
        DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("registerSelectViewport", cancellationToken, viewport, content, wrapper, dotNetReference, viewport.Id);
    }

    public async ValueTask UnregisterSelectViewport(ElementReference viewport, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("unregisterSelectViewport", cancellationToken, viewport, viewport.Id);
    }

    public async ValueTask RegisterSelectContentKeyboard(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("registerSelectContentKeyboard", cancellationToken, content, dotNetReference, content.Id);
    }

    public async ValueTask UnregisterSelectContentKeyboard(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("unregisterSelectContentKeyboard", cancellationToken, content, content.Id);
    }

    public async ValueTask ScrollSelectViewportByItem(ElementReference viewport, ElementReference item, bool upward, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("scrollSelectViewportByItem", cancellationToken, viewport, item, upward);
    }

    public async ValueTask StartSelectAutoScroll(ElementReference viewport, ElementReference item, bool upward, long generation, CancellationToken cancellationToken = default)
    {
        var module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("startSelectAutoScroll", cancellationToken, viewport, item, upward, generation);
    }

    public async ValueTask StopSelectAutoScroll(ElementReference viewport, long generation)
    {
        var module = await _moduleImportUtil.GetContentModuleReference(_modulePath);
        await module.InvokeVoidWithContextAsync("stopSelectAutoScroll", viewport, generation);
    }

    public async ValueTask RegisterSelectContentPointerTracker(ElementReference content, DotNetObjectReference<object> dotNetReference, double pageX, double pageY,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("registerSelectContentPointerTracker", cancellationToken, content, dotNetReference, pageX, pageY, content.Id);
    }

    public async ValueTask UnregisterSelectContentPointerTracker(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("unregisterSelectContentPointerTracker", cancellationToken, content, content.Id);
    }

    public async ValueTask RegisterSelectWindowDismiss(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("registerSelectWindowDismiss", cancellationToken, content, dotNetReference, content.Id);
    }

    public async ValueTask UnregisterSelectWindowDismiss(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidWithContextAsync("unregisterSelectWindowDismiss", cancellationToken, content, content.Id);
    }

    public async ValueTask<string?> GetSelectOptionValueAtPoint(double clientX, double clientY, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        return await module.InvokeWithContextAsync<string?>("getSelectOptionValueAtPoint", cancellationToken, clientX, clientY);
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
