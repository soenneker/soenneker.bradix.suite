namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix interact outside event args.
/// </summary>
public sealed class BradixInteractOutsideEventArgs : BradixPreventableEventArgs
{
    public BradixInteractOutsideEventArgs(string originalEventType, BradixDelegatedMouseEvent? pointerDownOutsideEvent = null,
        BradixDelegatedFocusEvent? focusOutsideEvent = null)
    {
        OriginalEventType = originalEventType;
        PointerDownOutsideEvent = pointerDownOutsideEvent;
        FocusOutsideEvent = focusOutsideEvent;
    }

    /// <summary>
    /// Gets original event type.
    /// </summary>
    public string OriginalEventType { get; }
    /// <summary>
    /// Gets pointer down outside event.
    /// </summary>
    public BradixDelegatedMouseEvent? PointerDownOutsideEvent { get; }
    /// <summary>
    /// Gets focus outside event.
    /// </summary>
    public BradixDelegatedFocusEvent? FocusOutsideEvent { get; }
}
