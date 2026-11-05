using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>Allows a trigger's activation to be canceled before its state changes.</summary>
public sealed class BradixActivationEventArgs : BradixPreventableEventArgs
{
    /// <summary>The click that requested activation, including keyboard-generated clicks.</summary>
    public MouseEventArgs OriginalEvent { get; }

    /// <summary>Creates a cancelable activation for the supplied click.</summary>
    public BradixActivationEventArgs(MouseEventArgs originalEvent) => OriginalEvent = originalEvent;
}
