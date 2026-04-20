using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Blazor interop for browser-facing functionality exposed by this package.
/// </summary>
public interface IBradixSuiteInterop : IAsyncDisposable
{
    /// <summary>
    /// Ensures the JavaScript module for this package has been loaded and initialized.
    /// </summary>
    ValueTask Initialize(CancellationToken cancellationToken = default);

    ValueTask ObserveCollapsibleContent(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask UnobserveCollapsibleContent(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterRovingFocusNavigationKeys(ElementReference element, object? dotNetReference = null,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterRovingFocusNavigationKeys(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterRadioGroupItemKeys(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask UnregisterRadioGroupItemKeys(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterCheckboxRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, string? formId = null,
        CancellationToken cancellationToken = default);

    ValueTask RegisterDelegatedInteraction(ElementReference element, object dotNetReference, object options,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterDelegatedInteraction(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterNavigationMenuTriggerInteraction(ElementReference element, object dotNetReference,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterNavigationMenuTriggerInteraction(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterTooltipTrigger(ElementReference element, object dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterTooltipTrigger(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterTooltipContent(ElementReference content, ElementReference trigger, object dotNetReference, string contentId,
        bool hoverableContent, CancellationToken cancellationToken = default);

    ValueTask UnregisterTooltipContent(ElementReference content, CancellationToken cancellationToken = default);

    ValueTask DispatchTooltipOpen(string contentId, CancellationToken cancellationToken = default);

    ValueTask RegisterFormRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterFormRoot(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask<BradixFormValiditySnapshot> GetFormControlValidity(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask<BradixFormControlSnapshot> GetFormControlState(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask SetFormControlCustomValidity(ElementReference element, string? validationMessage, CancellationToken cancellationToken = default);

    ValueTask ClearFormCustomValidity(ElementReference formElement, CancellationToken cancellationToken = default);

    ValueTask<bool> FocusServerInvalidFormControl(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask UnregisterCheckboxRoot(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask<bool> IsFormControl(ElementReference element, string? formId = null, CancellationToken cancellationToken = default);

    ValueTask<bool> IsKeyboardInteractionMode(CancellationToken cancellationToken = default);

    ValueTask SyncCheckboxBubbleInputState(ElementReference element, bool isChecked, bool isIndeterminate, bool dispatchEvent, bool bubbles = true,
        CancellationToken cancellationToken = default);

    ValueTask ClickElement(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask FocusElementDeferred(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask SelectInputText(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask SyncInputValue(ElementReference element, string? value, CancellationToken cancellationToken = default);

    ValueTask<bool> IsDirectionRtl(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterSliderPointerBridge(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterSliderPointerBridge(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask SyncSliderBubbleInputValue(ElementReference element, double value, bool dispatchEvent, CancellationToken cancellationToken = default);

    ValueTask RegisterSelectBubbleInput(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterSelectBubbleInput(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask SyncSelectBubbleInputValue(ElementReference element, string? value, bool dispatchEvent, CancellationToken cancellationToken = default);

    ValueTask RegisterScrollAreaRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterScrollAreaRoot(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterScrollAreaViewport(ElementReference viewport, ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterScrollAreaViewport(ElementReference viewport, CancellationToken cancellationToken = default);

    ValueTask RegisterScrollAreaScrollbar(ElementReference scrollbar, ElementReference thumb, ElementReference viewport, string orientation, string dir,
        DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterScrollAreaScrollbar(ElementReference scrollbar, CancellationToken cancellationToken = default);

    ValueTask MountPortal(ElementReference element, string? containerSelector = null, ElementReference container = default,
        CancellationToken cancellationToken = default);

    ValueTask UnmountPortal(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterToastViewport(ElementReference wrapper, ElementReference viewport, ElementReference headProxy, ElementReference tailProxy,
        IReadOnlyList<string> hotkey, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterToastViewport(ElementReference viewport, CancellationToken cancellationToken = default);

    ValueTask<bool> IsToastFocused(ElementReference toast, CancellationToken cancellationToken = default);

    ValueTask CapturePointer(ElementReference element, long pointerId, CancellationToken cancellationToken = default);

    ValueTask ReleasePointer(ElementReference element, long pointerId, CancellationToken cancellationToken = default);

    ValueTask SuppressNextClick(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask FocusElementById(string? elementId, CancellationToken cancellationToken = default);

    ValueTask FocusElementPreventScroll(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask<bool> FocusFirstMatchingDescendant(ElementReference element, string selector, CancellationToken cancellationToken = default);

    ValueTask ScrollElementIntoViewNearest(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterOneTimePasswordInput(ElementReference element, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterOneTimePasswordInput(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterAssociatedFormReset(ElementReference element, DotNetObjectReference<object> dotNetReference, string? formId = null,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterAssociatedFormReset(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RequestFormSubmit(ElementReference associatedElement, string? formId = null, CancellationToken cancellationToken = default);

    ValueTask RegisterDismissableLayer(ElementReference element, object dotNetReference, bool disableOutsidePointerEvents,
        CancellationToken cancellationToken = default);

    ValueTask UpdateDismissableLayer(ElementReference element, bool disableOutsidePointerEvents, CancellationToken cancellationToken = default);

    ValueTask UnregisterDismissableLayer(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterDismissableLayerBranch(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask UnregisterDismissableLayerBranch(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterMenubarDocumentDismiss(ElementReference element, object dotNetReference, string menubarId,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterMenubarDocumentDismiss(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterFocusScope(ElementReference element, DotNetObjectReference<object> dotNetReference, bool loop, bool trapped,
        bool preventMountAutoFocus, bool preventUnmountAutoFocus, CancellationToken cancellationToken = default);

    ValueTask UpdateFocusScope(ElementReference element, bool loop, bool trapped, bool preventMountAutoFocus, bool preventUnmountAutoFocus,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterFocusScope(ElementReference element, bool unmountAutoFocusPrevented = false, CancellationToken cancellationToken = default);

    ValueTask RegisterPopperContent(ElementReference anchor, ElementReference content, ElementReference arrow, DotNetObjectReference<object> dotNetReference,
        object options, CancellationToken cancellationToken = default);

    ValueTask<bool> BeginMenuSubmenuPointerGrace(ElementReference trigger, ElementReference content, double clientX, double clientY,
        DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask CancelMenuSubmenuPointerGrace(ElementReference trigger, CancellationToken cancellationToken = default);

    ValueTask RegisterVirtualPopperContent(ElementReference content, ElementReference arrow, DotNetObjectReference<object> dotNetReference, double x, double y,
        object options, CancellationToken cancellationToken = default);

    ValueTask UpdatePopperContent(ElementReference content, ElementReference arrow, object options, CancellationToken cancellationToken = default);

    ValueTask UpdateVirtualPopperContent(ElementReference content, ElementReference arrow, double x, double y, object options,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterPopperContent(ElementReference content, CancellationToken cancellationToken = default);

    ValueTask RegisterSelectItemAlignedPosition(ElementReference wrapper, ElementReference content, ElementReference viewport, ElementReference trigger,
        ElementReference valueNode, ElementReference selectedItem, ElementReference selectedItemText, string dir, CancellationToken cancellationToken = default);

    ValueTask UpdateSelectItemAlignedPosition(ElementReference wrapper, ElementReference content, ElementReference viewport, ElementReference trigger,
        ElementReference valueNode, ElementReference selectedItem, ElementReference selectedItemText, string dir, CancellationToken cancellationToken = default);

    ValueTask UnregisterSelectItemAlignedPosition(ElementReference wrapper, CancellationToken cancellationToken = default);

    ValueTask RegisterSelectViewport(ElementReference viewport, ElementReference content, ElementReference wrapper,
        DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterSelectViewport(ElementReference viewport, CancellationToken cancellationToken = default);

    ValueTask ScrollSelectViewportByItem(ElementReference viewport, ElementReference item, bool upward, CancellationToken cancellationToken = default);

    ValueTask RegisterSelectContentPointerTracker(ElementReference content, DotNetObjectReference<object> dotNetReference, double pageX, double pageY,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterSelectContentPointerTracker(ElementReference content, CancellationToken cancellationToken = default);

    ValueTask RegisterSelectWindowDismiss(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterSelectWindowDismiss(ElementReference content, CancellationToken cancellationToken = default);

    ValueTask DisableHoverCardContentTabNavigation(ElementReference content, CancellationToken cancellationToken = default);

    ValueTask RegisterHoverCardSelectionContainment(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    ValueTask BeginHoverCardSelectionContainment(ElementReference content, CancellationToken cancellationToken = default);

    ValueTask UnregisterHoverCardSelectionContainment(ElementReference content, CancellationToken cancellationToken = default);

    ValueTask RegisterAvatarImageLoadingStatus(string? src, string? crossOrigin, string? referrerPolicy,
        DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterAvatarImageLoadingStatus(DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask RegisterNavigationMenuIndicator(ElementReference indicator, ElementReference activeTrigger, ElementReference track,
        object dotNetReference, string orientation, CancellationToken cancellationToken = default);

    ValueTask UpdateNavigationMenuIndicator(ElementReference indicator, ElementReference activeTrigger, ElementReference track, string orientation,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterNavigationMenuIndicator(ElementReference indicator, CancellationToken cancellationToken = default);

    ValueTask RegisterNavigationMenuContentFocusBridge(ElementReference content, ElementReference trigger, ElementReference startProxy,
        ElementReference endProxy, CancellationToken cancellationToken = default);

    ValueTask UpdateNavigationMenuContentFocusBridge(ElementReference content, ElementReference trigger, ElementReference startProxy,
        ElementReference endProxy, CancellationToken cancellationToken = default);

    ValueTask UnregisterNavigationMenuContentFocusBridge(ElementReference content, CancellationToken cancellationToken = default);

    ValueTask RegisterNavigationMenuViewport(ElementReference viewport, ElementReference content, object dotNetReference,
        CancellationToken cancellationToken = default);

    ValueTask UpdateNavigationMenuViewport(ElementReference viewport, ElementReference content, CancellationToken cancellationToken = default);

    ValueTask UnregisterNavigationMenuViewport(ElementReference viewport, CancellationToken cancellationToken = default);


    ValueTask RegisterPresence(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask<BradixPresenceSnapshot> GetPresenceState(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask UnregisterPresence(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterFocusGuards(CancellationToken cancellationToken = default);

    ValueTask UnregisterFocusGuards(CancellationToken cancellationToken = default);

    ValueTask RegisterHideOthers(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask UnregisterHideOthers(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterRemoveScroll(bool allowPinchZoom = false, CancellationToken cancellationToken = default);

    ValueTask UnregisterRemoveScroll(CancellationToken cancellationToken = default);

    ValueTask RegisterLabelTextSelectionGuard(ElementReference element, DotNetObjectReference<object>? dotNetReference = null,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterLabelTextSelectionGuard(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask<string> GetTextContent(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask<string[]> GetToastAnnounceText(ElementReference element, CancellationToken cancellationToken = default);

}
