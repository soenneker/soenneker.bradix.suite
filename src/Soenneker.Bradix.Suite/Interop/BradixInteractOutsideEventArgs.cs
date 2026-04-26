namespace Soenneker.Bradix;

public sealed class BradixInteractOutsideEventArgs : BradixPreventableEventArgs
{
    public BradixInteractOutsideEventArgs(string originalEventType, BradixDelegatedMouseEvent? pointerDownOutsideEvent = null,
        BradixDelegatedFocusEvent? focusOutsideEvent = null)
    {
        OriginalEventType = originalEventType;
        PointerDownOutsideEvent = pointerDownOutsideEvent;
        FocusOutsideEvent = focusOutsideEvent;
    }

    public string OriginalEventType { get; }
    public BradixDelegatedMouseEvent? PointerDownOutsideEvent { get; }
    public BradixDelegatedFocusEvent? FocusOutsideEvent { get; }
}
