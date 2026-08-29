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
        return new ValueTask(Task.WhenAll(
            _collapsibleInterop.Initialize(cancellationToken).AsTask(),
            _controlsInterop.Initialize(cancellationToken).AsTask(),
            _delegatedInteractionInterop.Initialize(cancellationToken).AsTask(),
            _dismissableLayerInterop.Initialize(cancellationToken).AsTask(),
            _domInterop.Initialize(cancellationToken).AsTask(),
            _focusScopeInterop.Initialize(cancellationToken).AsTask(),
            _formInterop.Initialize(cancellationToken).AsTask(),
            _hoverCardAvatarInterop.Initialize(cancellationToken).AsTask(),
            _keyboardModeInterop.Initialize(cancellationToken).AsTask(),
            _labelInterop.Initialize(cancellationToken).AsTask(),
            _menuInterop.Initialize(cancellationToken).AsTask(),
            _menubarInterop.Initialize(cancellationToken).AsTask(),
            _navigationMenuInterop.Initialize(cancellationToken).AsTask(),
            _popperInterop.Initialize(cancellationToken).AsTask(),
            _portalInterop.Initialize(cancellationToken).AsTask(),
            _presenceOverlayInterop.Initialize(cancellationToken).AsTask(),
            _radioGroupInterop.Initialize(cancellationToken).AsTask(),
            _rovingFocusInterop.Initialize(cancellationToken).AsTask(),
            _scrollAreaInterop.Initialize(cancellationToken).AsTask(),
            _selectInterop.Initialize(cancellationToken).AsTask(),
            _toastInterop.Initialize(cancellationToken).AsTask(),
            _tooltipInterop.Initialize(cancellationToken).AsTask()));
    }

    /// <summary>
    /// Asynchronously releases resources used by the current instance.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
