using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ModuleImport;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;

namespace Soenneker.Bradix;

///<inheritdoc cref="IBradixSuiteInterop"/>
public sealed class BradixSuiteInterop : IBradixSuiteInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;

    private const string _modulePath = "./_content/Soenneker.Bradix.Suite/js/bradix.js";

    public BradixSuiteInterop(IJSRuntime jsRuntime, IModuleImportUtil? moduleImportUtil = null)
    {
        _moduleImportUtil = moduleImportUtil ?? new ModuleImportUtil(jsRuntime);
    }

    public async ValueTask Initialize(CancellationToken cancellationToken = default)
    {
        _ = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask ObserveCollapsibleContent(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("observeCollapsibleContent", cancellationToken, element);
    }

    public async ValueTask UnobserveCollapsibleContent(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unobserveCollapsibleContent", cancellationToken, element);
    }

    public async ValueTask RegisterRovingFocusNavigationKeys(ElementReference element, DotNetObjectReference<object>? dotNetReference = null,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerRovingFocusNavigationKeys", cancellationToken, element, dotNetReference);
    }

    public async ValueTask UnregisterRovingFocusNavigationKeys(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterRovingFocusNavigationKeys", cancellationToken, element);
    }

    public async ValueTask RegisterRadioGroupItemKeys(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerRadioGroupItemKeys", cancellationToken, element);
    }

    public async ValueTask UnregisterRadioGroupItemKeys(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterRadioGroupItemKeys", cancellationToken, element);
    }

    public async ValueTask RegisterCheckboxRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerCheckboxRoot", cancellationToken, element, dotNetReference);
    }

    public async ValueTask RegisterDelegatedInteraction(ElementReference element, DotNetObjectReference<object> dotNetReference, object options,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerDelegatedInteraction", cancellationToken, element, dotNetReference, options);
    }

    public async ValueTask UnregisterDelegatedInteraction(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterDelegatedInteraction", cancellationToken, element);
    }

    public async ValueTask RegisterTooltipTrigger(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerTooltipTrigger", cancellationToken, element, dotNetReference);
    }

    public async ValueTask UnregisterTooltipTrigger(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterTooltipTrigger", cancellationToken, element);
    }

    public async ValueTask RegisterTooltipContent(ElementReference content, ElementReference trigger, DotNetObjectReference<object> dotNetReference, string contentId,
        bool hoverableContent, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerTooltipContent", cancellationToken, content, trigger, dotNetReference, contentId, hoverableContent);
    }

    public async ValueTask UnregisterTooltipContent(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterTooltipContent", cancellationToken, content);
    }

    public async ValueTask DispatchTooltipOpen(string contentId, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("dispatchTooltipOpen", cancellationToken, contentId);
    }

    public async ValueTask RegisterFormRoot(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerFormRoot", cancellationToken, element);
    }

    public async ValueTask UnregisterFormRoot(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterFormRoot", cancellationToken, element);
    }

    public async ValueTask<BradixFormValiditySnapshot> GetFormControlValidity(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        BradixFormValiditySnapshot? snapshot = await module.InvokeAsync<BradixFormValiditySnapshot>("getFormControlValidity", cancellationToken, element)
            ;
        return snapshot ?? new BradixFormValiditySnapshot();
    }

    public async ValueTask<BradixFormControlSnapshot> GetFormControlState(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        BradixFormControlSnapshot? snapshot = await module.InvokeAsync<BradixFormControlSnapshot>("getFormControlState", cancellationToken, element)
            ;
        return snapshot ?? new BradixFormControlSnapshot();
    }

    public async ValueTask SetFormControlCustomValidity(ElementReference element, string? validationMessage, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("setFormControlCustomValidity", cancellationToken, element, validationMessage);
    }

    public async ValueTask ClearFormCustomValidity(ElementReference formElement, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("clearFormCustomValidity", cancellationToken, formElement);
    }

    public async ValueTask<bool> FocusServerInvalidFormControl(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        return await module.InvokeAsync<bool>("focusServerInvalidFormControl", cancellationToken, element);
    }

    public async ValueTask UnregisterCheckboxRoot(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterCheckboxRoot", cancellationToken, element);
    }

    public async ValueTask<bool> IsFormControl(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        return await module.InvokeAsync<bool>("isFormControl", cancellationToken, element);
    }

    public async ValueTask SyncCheckboxBubbleInputState(ElementReference element, bool isChecked, bool isIndeterminate, bool dispatchEvent, bool bubbles = true,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("syncCheckboxBubbleInputState", cancellationToken, element, isChecked, isIndeterminate, dispatchEvent, bubbles)
            ;
    }

    public async ValueTask ClickElement(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("clickElement", cancellationToken, element);
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

    public async ValueTask MountPortal(ElementReference element, string? containerSelector = null, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("mountPortal", cancellationToken, element, containerSelector);
    }

    public async ValueTask UnmountPortal(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unmountPortal", cancellationToken, element);
    }

    public async ValueTask RegisterToastViewport(ElementReference wrapper, ElementReference viewport, ElementReference headProxy, ElementReference tailProxy,
        IReadOnlyList<string> hotkey, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerToastViewport", cancellationToken, wrapper, viewport, headProxy, tailProxy, hotkey, dotNetReference)
            ;
    }

    public async ValueTask UnregisterToastViewport(ElementReference viewport, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterToastViewport", cancellationToken, viewport);
    }

    public async ValueTask<bool> IsToastFocused(ElementReference toast, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        return await module.InvokeAsync<bool>("isToastFocused", cancellationToken, toast);
    }

    public async ValueTask FocusElementById(string? elementId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(elementId))
            return;

        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("focusElementById", cancellationToken, elementId);
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

    public async ValueTask RequestFormSubmit(ElementReference associatedElement, string? formId = null, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("requestFormSubmit", cancellationToken, associatedElement, formId);
    }

    public async ValueTask RegisterDismissableLayer(ElementReference element, DotNetObjectReference<object> dotNetReference, bool disableOutsidePointerEvents, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerDismissableLayer", cancellationToken, element, dotNetReference, disableOutsidePointerEvents);
    }

    public async ValueTask UpdateDismissableLayer(ElementReference element, bool disableOutsidePointerEvents, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("updateDismissableLayer", cancellationToken, element, disableOutsidePointerEvents);
    }

    public async ValueTask UnregisterDismissableLayer(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterDismissableLayer", cancellationToken, element);
    }

    public async ValueTask RegisterDismissableLayerBranch(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerDismissableLayerBranch", cancellationToken, element);
    }

    public async ValueTask UnregisterDismissableLayerBranch(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterDismissableLayerBranch", cancellationToken, element);
    }

    public async ValueTask RegisterFocusScope(ElementReference element, DotNetObjectReference<object> dotNetReference, bool loop, bool trapped,
        bool preventMountAutoFocus, bool preventUnmountAutoFocus, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerFocusScope", cancellationToken, element, dotNetReference, loop, trapped, preventMountAutoFocus, preventUnmountAutoFocus)
            ;
    }

    public async ValueTask UpdateFocusScope(ElementReference element, bool loop, bool trapped, bool preventMountAutoFocus, bool preventUnmountAutoFocus,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("updateFocusScope", cancellationToken, element, loop, trapped, preventMountAutoFocus, preventUnmountAutoFocus)
            ;
    }

    public async ValueTask UnregisterFocusScope(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterFocusScope", cancellationToken, element);
    }

    public async ValueTask RegisterPopperContent(ElementReference anchor, ElementReference content, ElementReference arrow, DotNetObjectReference<object> dotNetReference, object options, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerPopperContent", cancellationToken, anchor, content, arrow, dotNetReference, options);
    }

    public async ValueTask RegisterVirtualPopperContent(ElementReference content, ElementReference arrow, DotNetObjectReference<object> dotNetReference, double x, double y, object options, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerVirtualPopperContent", cancellationToken, content, arrow, dotNetReference, x, y, options);
    }

    public async ValueTask UpdatePopperContent(ElementReference content, ElementReference arrow, object options, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("updatePopperContent", cancellationToken, content, arrow, options);
    }

    public async ValueTask UpdateVirtualPopperContent(ElementReference content, ElementReference arrow, double x, double y, object options, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("updateVirtualPopperContent", cancellationToken, content, arrow, x, y, options);
    }

    public async ValueTask UnregisterPopperContent(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterPopperContent", cancellationToken, content);
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

    public async ValueTask DisableHoverCardContentTabNavigation(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("disableHoverCardContentTabNavigation", cancellationToken, content);
    }

    public async ValueTask RegisterHoverCardSelectionContainment(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerHoverCardSelectionContainment", cancellationToken, content, dotNetReference);
    }

    public async ValueTask BeginHoverCardSelectionContainment(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("beginHoverCardSelectionContainment", cancellationToken, content);
    }

    public async ValueTask UnregisterHoverCardSelectionContainment(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterHoverCardSelectionContainment", cancellationToken, content);
    }

    public async ValueTask RegisterAvatarImageLoadingStatus(string? src, string? crossOrigin, string? referrerPolicy,
        DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerAvatarImageLoadingStatus", cancellationToken, src, crossOrigin, referrerPolicy, dotNetReference)
            ;
    }

    public async ValueTask UnregisterAvatarImageLoadingStatus(DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterAvatarImageLoadingStatus", cancellationToken, dotNetReference);
    }

    public async ValueTask RegisterNavigationMenuIndicator(ElementReference indicator, ElementReference activeTrigger, ElementReference root,
        DotNetObjectReference<object> dotNetReference, string orientation, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerNavigationMenuIndicator", cancellationToken, indicator, activeTrigger, root, dotNetReference, orientation)
            ;
    }

    public async ValueTask UpdateNavigationMenuIndicator(ElementReference indicator, ElementReference activeTrigger, ElementReference root,
        string orientation, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("updateNavigationMenuIndicator", cancellationToken, indicator, activeTrigger, root, orientation);
    }

    public async ValueTask UnregisterNavigationMenuIndicator(ElementReference indicator, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterNavigationMenuIndicator", cancellationToken, indicator);
    }

    public async ValueTask RegisterNavigationMenuContentFocusBridge(ElementReference content, ElementReference trigger, ElementReference startProxy,
        ElementReference endProxy, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerNavigationMenuContentFocusBridge", cancellationToken, content, trigger, startProxy, endProxy);
    }

    public async ValueTask UpdateNavigationMenuContentFocusBridge(ElementReference content, ElementReference trigger, ElementReference startProxy,
        ElementReference endProxy, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("updateNavigationMenuContentFocusBridge", cancellationToken, content, trigger, startProxy, endProxy);
    }

    public async ValueTask UnregisterNavigationMenuContentFocusBridge(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterNavigationMenuContentFocusBridge", cancellationToken, content);
    }

    public async ValueTask RegisterNavigationMenuViewport(ElementReference viewport, ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerNavigationMenuViewport", cancellationToken, viewport, content, dotNetReference);
    }

    public async ValueTask UpdateNavigationMenuViewport(ElementReference viewport, ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("updateNavigationMenuViewport", cancellationToken, viewport, content);
    }

    public async ValueTask UnregisterNavigationMenuViewport(ElementReference viewport, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterNavigationMenuViewport", cancellationToken, viewport);
    }

    public async ValueTask RegisterPresence(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerPresence", cancellationToken, element, dotNetReference);
    }

    public async ValueTask<BradixPresenceSnapshot> GetPresenceState(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        BradixPresenceSnapshot? snapshot = await module.InvokeAsync<BradixPresenceSnapshot>("getPresenceState", cancellationToken, element);
        return snapshot ?? new BradixPresenceSnapshot();
    }

    public async ValueTask UnregisterPresence(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterPresence", cancellationToken, element);
    }

    public async ValueTask RegisterFocusGuards(CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerFocusGuards", cancellationToken);
    }

    public async ValueTask UnregisterFocusGuards(CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterFocusGuards", cancellationToken);
    }

    public async ValueTask RegisterHideOthers(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerHideOthers", cancellationToken, element);
    }

    public async ValueTask UnregisterHideOthers(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterHideOthers", cancellationToken, element);
    }

    public async ValueTask RegisterRemoveScroll(bool allowPinchZoom = false, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerRemoveScroll", cancellationToken, allowPinchZoom);
    }

    public async ValueTask UnregisterRemoveScroll(CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterRemoveScroll", cancellationToken);
    }

    public async ValueTask RegisterLabelTextSelectionGuard(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerLabelTextSelectionGuard", cancellationToken, element);
    }

    public async ValueTask UnregisterLabelTextSelectionGuard(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterLabelTextSelectionGuard", cancellationToken, element);
    }

    public async ValueTask<string> GetTextContent(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        string? textContent = await module.InvokeAsync<string>("getTextContent", cancellationToken, element);
        return textContent ?? string.Empty;
    }

    public async ValueTask<string[]> GetToastAnnounceText(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        string[]? textContent = await module.InvokeAsync<string[]>("getToastAnnounceText", cancellationToken, element);
        return textContent ?? [];
    }

    public async ValueTask<string> GetActiveElementId(CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        string? id = await module.InvokeAsync<string>("getActiveElementId", cancellationToken);
        return id ?? string.Empty;
    }

    public async ValueTask<BradixMenubarActiveElementState> GetMenubarActiveElementState(string currentContentId, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        BradixMenubarActiveElementState? state = await module.InvokeAsync<BradixMenubarActiveElementState>("getMenubarActiveElementState", cancellationToken, currentContentId);
        return state ?? new BradixMenubarActiveElementState();
    }

    public async ValueTask DisposeAsync()
    {
        await _moduleImportUtil.DisposeContentModule(_modulePath);
    }

}
