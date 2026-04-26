namespace Soenneker.Bradix;

public sealed class BradixPointerDownOutsideEventArgs : BradixPreventableEventArgs
{
    public BradixPointerDownOutsideEventArgs(BradixDelegatedMouseEvent originalEvent)
    {
        OriginalEvent = originalEvent;
    }

    public BradixDelegatedMouseEvent OriginalEvent { get; }
}
