namespace Soenneker.Bradix;

public sealed class BradixFocusOutsideEventArgs : BradixPreventableEventArgs
{
    public BradixFocusOutsideEventArgs(BradixDelegatedFocusEvent originalEvent)
    {
        OriginalEvent = originalEvent;
    }

    public BradixDelegatedFocusEvent OriginalEvent { get; }
}
