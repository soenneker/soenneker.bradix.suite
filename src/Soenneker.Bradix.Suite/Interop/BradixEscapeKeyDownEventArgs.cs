namespace Soenneker.Bradix;

public sealed class BradixEscapeKeyDownEventArgs : BradixPreventableEventArgs
{
    public BradixEscapeKeyDownEventArgs(BradixDelegatedKeyboardEvent originalEvent)
    {
        OriginalEvent = originalEvent;
    }

    public BradixDelegatedKeyboardEvent OriginalEvent { get; }
}
