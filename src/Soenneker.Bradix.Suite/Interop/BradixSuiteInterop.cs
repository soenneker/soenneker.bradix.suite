using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Bradix;

/// <inheritdoc cref="IBradixSuiteInterop"/>
public sealed class BradixSuiteInterop : IBradixSuiteInterop
{
    private readonly ICollapsibleInterop _collapsibleInterop;
    private readonly IControlsInterop _controlsInterop;
    private readonly IDelegatedInteractionInterop _delegatedInteractionInterop;
    private readonly IDismissableLayerInterop _dismissableLayerInterop;
    private readonly IDomInterop _domInterop;
    private readonly IFocusScopeInterop _focusScopeInterop;
    private readonly IFormInterop _formInterop;
    private readonly IHoverCardAvatarInterop _hoverCardAvatarInterop;
    private readonly IKeyboardModeInterop _keyboardModeInterop;
    private readonly ILabelInterop _labelInterop;
    private readonly IMenuInterop _menuInterop;
    private readonly IMenubarInterop _menubarInterop;
    private readonly INavigationMenuInterop _navigationMenuInterop;
    private readonly IPopperInterop _popperInterop;
    private readonly IPortalInterop _portalInterop;
    private readonly IPresenceOverlayInterop _presenceOverlayInterop;
    private readonly IRadioGroupInterop _radioGroupInterop;
    private readonly IRovingFocusInterop _rovingFocusInterop;
    private readonly IScrollAreaInterop _scrollAreaInterop;
    private readonly ISelectInterop _selectInterop;
    private readonly IToastInterop _toastInterop;
    private readonly ITooltipInterop _tooltipInterop;

    public BradixSuiteInterop(ICollapsibleInterop collapsibleInterop, IControlsInterop controlsInterop, IDelegatedInteractionInterop delegatedInteractionInterop, IDismissableLayerInterop dismissableLayerInterop, IDomInterop domInterop, IFocusScopeInterop focusScopeInterop, IFormInterop formInterop, IHoverCardAvatarInterop hoverCardAvatarInterop, IKeyboardModeInterop keyboardModeInterop, ILabelInterop labelInterop, IMenuInterop menuInterop, IMenubarInterop menubarInterop, INavigationMenuInterop navigationMenuInterop, IPopperInterop popperInterop, IPortalInterop portalInterop, IPresenceOverlayInterop presenceOverlayInterop, IRadioGroupInterop radioGroupInterop, IRovingFocusInterop rovingFocusInterop, IScrollAreaInterop scrollAreaInterop, ISelectInterop selectInterop, IToastInterop toastInterop, ITooltipInterop tooltipInterop)
    {
        _collapsibleInterop = collapsibleInterop;
        _controlsInterop = controlsInterop;
        _delegatedInteractionInterop = delegatedInteractionInterop;
        _dismissableLayerInterop = dismissableLayerInterop;
        _domInterop = domInterop;
        _focusScopeInterop = focusScopeInterop;
        _formInterop = formInterop;
        _hoverCardAvatarInterop = hoverCardAvatarInterop;
        _keyboardModeInterop = keyboardModeInterop;
        _labelInterop = labelInterop;
        _menuInterop = menuInterop;
        _menubarInterop = menubarInterop;
        _navigationMenuInterop = navigationMenuInterop;
        _popperInterop = popperInterop;
        _portalInterop = portalInterop;
        _presenceOverlayInterop = presenceOverlayInterop;
        _radioGroupInterop = radioGroupInterop;
        _rovingFocusInterop = rovingFocusInterop;
        _scrollAreaInterop = scrollAreaInterop;
        _selectInterop = selectInterop;
        _toastInterop = toastInterop;
        _tooltipInterop = tooltipInterop;
    }

    public ValueTask Initialize(CancellationToken cancellationToken = default)
    {
        List<Task>? pending = null;
        AddInitialization(ref pending, _collapsibleInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _controlsInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _delegatedInteractionInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _dismissableLayerInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _domInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _focusScopeInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _formInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _hoverCardAvatarInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _keyboardModeInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _labelInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _menuInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _menubarInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _navigationMenuInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _popperInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _portalInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _presenceOverlayInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _radioGroupInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _rovingFocusInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _scrollAreaInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _selectInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _toastInterop.Initialize(cancellationToken));
        AddInitialization(ref pending, _tooltipInterop.Initialize(cancellationToken));
        return pending is null ? ValueTask.CompletedTask : new ValueTask(Task.WhenAll(pending));
    }

    private static void AddInitialization(ref List<Task>? pending, ValueTask<IJSObjectReference> operation)
    {
        if (operation.IsCompletedSuccessfully)
        {
            _ = operation.Result;
            return;
        }
        (pending ??= new List<Task>(22)).Add(operation.AsTask());
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
