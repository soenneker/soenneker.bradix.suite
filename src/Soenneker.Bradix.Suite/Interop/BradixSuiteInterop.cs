using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Bradix.Suite.Presence;
using Soenneker.Bradix.Suite.Abstract;
using Soenneker.Bradix.Suite.Menubar;

namespace Soenneker.Bradix.Suite.Interop;

public sealed class BradixSuiteInterop : ISuiteInterop
{
    private readonly IJSRuntime _jsRuntime;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private IJSObjectReference? _module;

    public BradixSuiteInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async ValueTask Initialize(CancellationToken cancellationToken = default)
    {
        _ = await GetModule(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask ObserveCollapsibleContent(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("observeCollapsibleContent", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask UnobserveCollapsibleContent(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unobserveCollapsibleContent", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask RegisterRovingFocusNavigationKeys(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerRovingFocusNavigationKeys", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask UnregisterRovingFocusNavigationKeys(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterRovingFocusNavigationKeys", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask RegisterRadioGroupItemKeys(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerRadioGroupItemKeys", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask UnregisterRadioGroupItemKeys(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterRadioGroupItemKeys", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask RegisterCheckboxRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerCheckboxRoot", cancellationToken, element, dotNetReference).ConfigureAwait(false);
    }

    public async ValueTask UnregisterCheckboxRoot(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterCheckboxRoot", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask SyncCheckboxBubbleInputState(ElementReference element, bool isChecked, bool isIndeterminate, bool dispatchEvent, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("syncCheckboxBubbleInputState", cancellationToken, element, isChecked, isIndeterminate, dispatchEvent).ConfigureAwait(false);
    }

    public async ValueTask ClickElement(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("clickElement", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask RegisterSliderPointerBridge(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerSliderPointerBridge", cancellationToken, element, dotNetReference).ConfigureAwait(false);
    }

    public async ValueTask UnregisterSliderPointerBridge(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterSliderPointerBridge", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask RegisterScrollAreaRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerScrollAreaRoot", cancellationToken, element, dotNetReference).ConfigureAwait(false);
    }

    public async ValueTask UnregisterScrollAreaRoot(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterScrollAreaRoot", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask RegisterScrollAreaViewport(ElementReference viewport, ElementReference content, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerScrollAreaViewport", cancellationToken, viewport, content, dotNetReference).ConfigureAwait(false);
    }

    public async ValueTask UnregisterScrollAreaViewport(ElementReference viewport, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterScrollAreaViewport", cancellationToken, viewport).ConfigureAwait(false);
    }

    public async ValueTask RegisterScrollAreaScrollbar(ElementReference scrollbar, ElementReference thumb, ElementReference viewport, string orientation, string dir, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerScrollAreaScrollbar", cancellationToken, scrollbar, thumb, viewport, orientation, dir, dotNetReference).ConfigureAwait(false);
    }

    public async ValueTask UnregisterScrollAreaScrollbar(ElementReference scrollbar, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterScrollAreaScrollbar", cancellationToken, scrollbar).ConfigureAwait(false);
    }

    public async ValueTask MountPortal(ElementReference element, string? containerSelector = null, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("mountPortal", cancellationToken, element, containerSelector).ConfigureAwait(false);
    }

    public async ValueTask UnmountPortal(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unmountPortal", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask RegisterDismissableLayer(ElementReference element, DotNetObjectReference<object> dotNetReference, bool disableOutsidePointerEvents, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerDismissableLayer", cancellationToken, element, dotNetReference, disableOutsidePointerEvents).ConfigureAwait(false);
    }

    public async ValueTask UpdateDismissableLayer(ElementReference element, bool disableOutsidePointerEvents, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("updateDismissableLayer", cancellationToken, element, disableOutsidePointerEvents).ConfigureAwait(false);
    }

    public async ValueTask UnregisterDismissableLayer(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterDismissableLayer", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask RegisterDismissableLayerBranch(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerDismissableLayerBranch", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask UnregisterDismissableLayerBranch(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterDismissableLayerBranch", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask RegisterFocusScope(ElementReference element, DotNetObjectReference<object> dotNetReference, bool loop, bool trapped, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerFocusScope", cancellationToken, element, dotNetReference, loop, trapped).ConfigureAwait(false);
    }

    public async ValueTask UpdateFocusScope(ElementReference element, bool loop, bool trapped, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("updateFocusScope", cancellationToken, element, loop, trapped).ConfigureAwait(false);
    }

    public async ValueTask UnregisterFocusScope(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterFocusScope", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask RegisterPopperContent(ElementReference anchor, ElementReference content, ElementReference arrow, DotNetObjectReference<object> dotNetReference, object options, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerPopperContent", cancellationToken, anchor, content, arrow, dotNetReference, options).ConfigureAwait(false);
    }

    public async ValueTask RegisterVirtualPopperContent(ElementReference content, ElementReference arrow, DotNetObjectReference<object> dotNetReference, double x, double y, object options, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerVirtualPopperContent", cancellationToken, content, arrow, dotNetReference, x, y, options).ConfigureAwait(false);
    }

    public async ValueTask UpdatePopperContent(ElementReference content, ElementReference arrow, object options, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("updatePopperContent", cancellationToken, content, arrow, options).ConfigureAwait(false);
    }

    public async ValueTask UpdateVirtualPopperContent(ElementReference content, ElementReference arrow, double x, double y, object options, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("updateVirtualPopperContent", cancellationToken, content, arrow, x, y, options).ConfigureAwait(false);
    }

    public async ValueTask UnregisterPopperContent(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterPopperContent", cancellationToken, content).ConfigureAwait(false);
    }

    public async ValueTask RegisterSelectItemAlignedPosition(ElementReference wrapper, ElementReference content, ElementReference viewport, ElementReference trigger,
        ElementReference valueNode, ElementReference selectedItem, ElementReference selectedItemText, string dir, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerSelectItemAlignedPosition", cancellationToken, wrapper, content, viewport, trigger, valueNode, selectedItem,
            selectedItemText, dir).ConfigureAwait(false);
    }

    public async ValueTask UpdateSelectItemAlignedPosition(ElementReference wrapper, ElementReference content, ElementReference viewport, ElementReference trigger,
        ElementReference valueNode, ElementReference selectedItem, ElementReference selectedItemText, string dir, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("updateSelectItemAlignedPosition", cancellationToken, wrapper, content, viewport, trigger, valueNode, selectedItem,
            selectedItemText, dir).ConfigureAwait(false);
    }

    public async ValueTask UnregisterSelectItemAlignedPosition(ElementReference wrapper, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterSelectItemAlignedPosition", cancellationToken, wrapper).ConfigureAwait(false);
    }

    public async ValueTask RegisterSelectViewport(ElementReference viewport, ElementReference content, ElementReference wrapper,
        DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerSelectViewport", cancellationToken, viewport, content, wrapper, dotNetReference).ConfigureAwait(false);
    }

    public async ValueTask UnregisterSelectViewport(ElementReference viewport, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterSelectViewport", cancellationToken, viewport).ConfigureAwait(false);
    }

    public async ValueTask ScrollSelectViewportByItem(ElementReference viewport, ElementReference item, bool upward, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("scrollSelectViewportByItem", cancellationToken, viewport, item, upward).ConfigureAwait(false);
    }

    public async ValueTask RegisterSelectContentPointerTracker(ElementReference content, DotNetObjectReference<object> dotNetReference, double pageX, double pageY,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerSelectContentPointerTracker", cancellationToken, content, dotNetReference, pageX, pageY).ConfigureAwait(false);
    }

    public async ValueTask UnregisterSelectContentPointerTracker(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterSelectContentPointerTracker", cancellationToken, content).ConfigureAwait(false);
    }

    public async ValueTask RegisterSelectWindowDismiss(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerSelectWindowDismiss", cancellationToken, content, dotNetReference).ConfigureAwait(false);
    }

    public async ValueTask UnregisterSelectWindowDismiss(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterSelectWindowDismiss", cancellationToken, content).ConfigureAwait(false);
    }

    public async ValueTask DisableHoverCardContentTabNavigation(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("disableHoverCardContentTabNavigation", cancellationToken, content).ConfigureAwait(false);
    }

    public async ValueTask RegisterHoverCardSelectionContainment(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerHoverCardSelectionContainment", cancellationToken, content, dotNetReference).ConfigureAwait(false);
    }

    public async ValueTask BeginHoverCardSelectionContainment(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("beginHoverCardSelectionContainment", cancellationToken, content).ConfigureAwait(false);
    }

    public async ValueTask UnregisterHoverCardSelectionContainment(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterHoverCardSelectionContainment", cancellationToken, content).ConfigureAwait(false);
    }

    public async ValueTask RegisterAvatarImageLoadingStatus(string? src, string? crossOrigin, string? referrerPolicy,
        DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerAvatarImageLoadingStatus", cancellationToken, src, crossOrigin, referrerPolicy, dotNetReference)
            .ConfigureAwait(false);
    }

    public async ValueTask UnregisterAvatarImageLoadingStatus(DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterAvatarImageLoadingStatus", cancellationToken, dotNetReference).ConfigureAwait(false);
    }

    public async ValueTask RegisterNavigationMenuIndicator(ElementReference indicator, ElementReference activeTrigger, ElementReference root,
        DotNetObjectReference<object> dotNetReference, string orientation, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerNavigationMenuIndicator", cancellationToken, indicator, activeTrigger, root, dotNetReference, orientation)
            .ConfigureAwait(false);
    }

    public async ValueTask UpdateNavigationMenuIndicator(ElementReference indicator, ElementReference activeTrigger, ElementReference root,
        string orientation, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("updateNavigationMenuIndicator", cancellationToken, indicator, activeTrigger, root, orientation).ConfigureAwait(false);
    }

    public async ValueTask UnregisterNavigationMenuIndicator(ElementReference indicator, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterNavigationMenuIndicator", cancellationToken, indicator).ConfigureAwait(false);
    }

    public async ValueTask RegisterNavigationMenuViewport(ElementReference viewport, ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerNavigationMenuViewport", cancellationToken, viewport, content, dotNetReference).ConfigureAwait(false);
    }

    public async ValueTask UpdateNavigationMenuViewport(ElementReference viewport, ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("updateNavigationMenuViewport", cancellationToken, viewport, content).ConfigureAwait(false);
    }

    public async ValueTask UnregisterNavigationMenuViewport(ElementReference viewport, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterNavigationMenuViewport", cancellationToken, viewport).ConfigureAwait(false);
    }

    public async ValueTask RegisterPresence(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerPresence", cancellationToken, element, dotNetReference).ConfigureAwait(false);
    }

    public async ValueTask<BradixPresenceSnapshot> GetPresenceState(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        BradixPresenceSnapshot? snapshot = await module.InvokeAsync<BradixPresenceSnapshot>("getPresenceState", cancellationToken, element).ConfigureAwait(false);
        return snapshot ?? new BradixPresenceSnapshot();
    }

    public async ValueTask UnregisterPresence(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterPresence", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask RegisterFocusGuards(CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerFocusGuards", cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask UnregisterFocusGuards(CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterFocusGuards", cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask RegisterHideOthers(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerHideOthers", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask UnregisterHideOthers(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterHideOthers", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask RegisterRemoveScroll(CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerRemoveScroll", cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask UnregisterRemoveScroll(CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterRemoveScroll", cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask RegisterLabelTextSelectionGuard(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerLabelTextSelectionGuard", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask UnregisterLabelTextSelectionGuard(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterLabelTextSelectionGuard", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask<string> GetTextContent(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        string? textContent = await module.InvokeAsync<string>("getTextContent", cancellationToken, element).ConfigureAwait(false);
        return textContent ?? string.Empty;
    }

    public async ValueTask<string> GetActiveElementId(CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        string? id = await module.InvokeAsync<string>("getActiveElementId", cancellationToken).ConfigureAwait(false);
        return id ?? string.Empty;
    }

    public async ValueTask<BradixMenubarActiveElementState> GetMenubarActiveElementState(string currentContentId, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        BradixMenubarActiveElementState? state = await module.InvokeAsync<BradixMenubarActiveElementState>("getMenubarActiveElementState", cancellationToken, currentContentId).ConfigureAwait(false);
        return state ?? new BradixMenubarActiveElementState();
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            try
            {
                await _module.DisposeAsync().ConfigureAwait(false);
            }
            catch (JSDisconnectedException)
            {
            }
        }

        _semaphore.Dispose();
    }

    private async ValueTask<IJSObjectReference> GetModule(CancellationToken cancellationToken)
    {
        if (_module is not null)
            return _module;

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (_module is null)
            {
                _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    cancellationToken,
                    "./_content/Soenneker.Bradix.Suite/js/bradix.js").ConfigureAwait(false);
            }

            return _module;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
