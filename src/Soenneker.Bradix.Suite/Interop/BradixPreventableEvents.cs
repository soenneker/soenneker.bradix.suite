namespace Soenneker.Bradix;

public class BradixPreventableEventArgs
{
    public bool DefaultPrevented { get; private set; }

    public void PreventDefault()
    {
        DefaultPrevented = true;
    }
}

public sealed class BradixAutoFocusEventArgs : BradixPreventableEventArgs
{
}

public sealed class BradixEscapeKeyDownEventArgs : BradixPreventableEventArgs
{
    public BradixEscapeKeyDownEventArgs(BradixDelegatedKeyboardEvent originalEvent)
    {
        OriginalEvent = originalEvent;
    }

    public BradixDelegatedKeyboardEvent OriginalEvent { get; }
}

public sealed class BradixPointerDownOutsideEventArgs : BradixPreventableEventArgs
{
    public BradixPointerDownOutsideEventArgs(BradixDelegatedMouseEvent originalEvent)
    {
        OriginalEvent = originalEvent;
    }

    public BradixDelegatedMouseEvent OriginalEvent { get; }
}

public sealed class BradixFocusOutsideEventArgs : BradixPreventableEventArgs
{
    public BradixFocusOutsideEventArgs(BradixDelegatedFocusEvent originalEvent)
    {
        OriginalEvent = originalEvent;
    }

    public BradixDelegatedFocusEvent OriginalEvent { get; }
}

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
