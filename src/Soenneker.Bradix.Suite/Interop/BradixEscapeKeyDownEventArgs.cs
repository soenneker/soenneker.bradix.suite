namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix escape key down event args.
/// </summary>
public sealed class BradixEscapeKeyDownEventArgs : BradixPreventableEventArgs
{
    public BradixEscapeKeyDownEventArgs(BradixDelegatedKeyboardEvent originalEvent)
    {
        OriginalEvent = originalEvent;
    }

    /// <summary>
    /// Gets original event.
    /// </summary>
    public BradixDelegatedKeyboardEvent OriginalEvent { get; }
}
