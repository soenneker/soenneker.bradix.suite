using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;

namespace Soenneker.Bradix;

/// <inheritdoc cref="IControlsInterop"/>
public sealed class ControlsInterop : IControlsInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;

    private const string _modulePath = "./_content/Soenneker.Bradix.Suite/js/bradix/controls.js";

    public ControlsInterop(IModuleImportUtil moduleImportUtil)
    {
        _moduleImportUtil = moduleImportUtil;
    }

    public async ValueTask SyncCheckboxBubbleInputState(ElementReference element, bool isChecked, bool isIndeterminate, bool dispatchEvent, bool bubbles = true,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("syncCheckboxBubbleInputState", cancellationToken, element, isChecked, isIndeterminate, dispatchEvent, bubbles)
            ;
    }

    public ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default)
    {
        return _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask ClickElement(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("clickElement", cancellationToken, element);
    }

    public async ValueTask FocusElementDeferred(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("focusElementDeferred", cancellationToken, element);
    }

    public async ValueTask SelectInputText(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("selectInputText", cancellationToken, element);
    }

    public async ValueTask SyncInputValue(ElementReference element, string? value, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("syncInputValue", cancellationToken, element, value);
    }

    public async ValueTask<bool> IsDirectionRtl(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        return await module.InvokeAsync<bool>("isDirectionRtl", cancellationToken, element);
    }

    public async ValueTask RegisterSliderPointerBridge(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerSliderPointerBridge", cancellationToken, element, dotNetReference);
    }

    public async ValueTask UnregisterSliderPointerBridge(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterSliderPointerBridge", cancellationToken, element);
    }

    public async ValueTask SyncSliderBubbleInputValue(ElementReference element, double value, bool dispatchEvent, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("syncSliderBubbleInputValue", cancellationToken, element, value, dispatchEvent);
    }

    public async ValueTask RegisterSelectBubbleInput(ElementReference element, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerSelectBubbleInput", cancellationToken, element, dotNetReference);
    }

    public async ValueTask UnregisterSelectBubbleInput(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterSelectBubbleInput", cancellationToken, element);
    }

    public async ValueTask SyncSelectBubbleInputValue(ElementReference element, string? value, bool dispatchEvent, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("syncSelectBubbleInputValue", cancellationToken, element, value, dispatchEvent);
    }

    public async ValueTask CapturePointer(ElementReference element, long pointerId, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("capturePointer", cancellationToken, element, pointerId);
    }

    public async ValueTask ReleasePointer(ElementReference element, long pointerId, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("releasePointer", cancellationToken, element, pointerId);
    }

    public async ValueTask SuppressNextClick(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("suppressNextClick", cancellationToken, element);
    }

    public async ValueTask FocusElementById(string? elementId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(elementId))
            return;

        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("focusElementById", cancellationToken, elementId);
    }

    public async ValueTask FocusElementByIdDeferred(string? elementId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(elementId))
            return;

        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("focusElementByIdDeferred", cancellationToken, elementId);
    }

    public ValueTask FocusElementPreventScroll(ElementReference element, CancellationToken cancellationToken = default)
    {
        return element.FocusAsync(preventScroll: true);
    }

    public async ValueTask<bool> FocusFirstMatchingDescendant(ElementReference element, string selector, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        return await module.InvokeAsync<bool>("focusFirstMatchingDescendant", cancellationToken, element, selector);
    }

    public async ValueTask ScrollElementIntoViewNearest(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("scrollElementIntoViewNearest", cancellationToken, element);
    }

    public async ValueTask RegisterOneTimePasswordInput(ElementReference element, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerOneTimePasswordInput", cancellationToken, element, dotNetReference);
    }

    public async ValueTask UnregisterOneTimePasswordInput(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterOneTimePasswordInput", cancellationToken, element);
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
