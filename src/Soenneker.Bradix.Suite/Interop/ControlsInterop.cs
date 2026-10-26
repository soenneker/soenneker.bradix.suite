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
        await module.InvokeBradixVoidAsync("syncCheckboxBubbleInputState", cancellationToken, element, (isChecked ? BradixAttributeValues.True : BradixAttributeValues.False), (isIndeterminate ? BradixAttributeValues.True : BradixAttributeValues.False), (dispatchEvent ? BradixAttributeValues.True : BradixAttributeValues.False), (bubbles ? BradixAttributeValues.True : BradixAttributeValues.False))
            ;
    }

    public ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default)
    {
        return _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask ClickElement(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("clickElement", cancellationToken, element);
    }

    public async ValueTask RegisterContextMenuKeyboard(ElementReference element, DotNetObjectReference<object>? dotNetReference = null, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("registerContextMenuKeyboard", cancellationToken, element, dotNetReference);
    }

    public async ValueTask UnregisterContextMenuKeyboard(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("unregisterContextMenuKeyboard", cancellationToken, element);
    }

    public async ValueTask FocusElementDeferred(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("focusElementDeferred", cancellationToken, element);
    }

    public async ValueTask SelectInputText(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("selectInputText", cancellationToken, element);
    }

    public async ValueTask SyncInputValue(ElementReference element, string? value, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("syncInputValue", cancellationToken, element, value);
    }

    public async ValueTask<bool> IsDirectionRtl(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        return await module.InvokeBradixAsync<bool>("isDirectionRtl", cancellationToken, element);
    }

    public async ValueTask RegisterSliderPointerBridge(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("registerSliderPointerBridge", cancellationToken, element, dotNetReference);
    }

    public async ValueTask UnregisterSliderPointerBridge(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("unregisterSliderPointerBridge", cancellationToken, element);
    }

    public async ValueTask SyncSliderBubbleInputValue(ElementReference element, double value, bool dispatchEvent, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("syncSliderBubbleInputValue", cancellationToken, element, value, (dispatchEvent ? BradixAttributeValues.True : BradixAttributeValues.False));
    }

    public async ValueTask RegisterSelectBubbleInput(ElementReference element, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("registerSelectBubbleInput", cancellationToken, element, dotNetReference);
    }

    public async ValueTask UnregisterSelectBubbleInput(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("unregisterSelectBubbleInput", cancellationToken, element);
    }

    public async ValueTask SyncSelectBubbleInputValue(ElementReference element, string? value, bool dispatchEvent, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("syncSelectBubbleInputValue", cancellationToken, element, value, (dispatchEvent ? BradixAttributeValues.True : BradixAttributeValues.False));
    }

    public async ValueTask CapturePointer(ElementReference element, long pointerId, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("capturePointer", cancellationToken, element, pointerId);
    }

    public async ValueTask ReleasePointer(ElementReference element, long pointerId, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("releasePointer", cancellationToken, element, pointerId);
    }

    public async ValueTask SuppressNextClick(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("suppressNextClick", cancellationToken, element);
    }

    public async ValueTask FocusElementById(string? elementId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(elementId))
            return;

        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("focusElementById", cancellationToken, elementId);
    }

    public async ValueTask FocusElementByIdDeferred(string? elementId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(elementId))
            return;

        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("focusElementByIdDeferred", cancellationToken, elementId);
    }

    public async ValueTask FocusElementPreventScroll(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("focusElementPreventScroll", cancellationToken, element);
    }

    public async ValueTask<bool> FocusFirstMatchingDescendant(ElementReference element, string selector, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        return await module.InvokeBradixAsync<bool>("focusFirstMatchingDescendant", cancellationToken, element, selector);
    }

    public async ValueTask ScrollElementIntoViewNearest(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("scrollElementIntoViewNearest", cancellationToken, element);
    }

    public async ValueTask RegisterOneTimePasswordInput(ElementReference element, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("registerOneTimePasswordInput", cancellationToken, element, dotNetReference);
    }

    public async ValueTask UnregisterOneTimePasswordInput(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeBradixVoidAsync("unregisterOneTimePasswordInput", cancellationToken, element);
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
