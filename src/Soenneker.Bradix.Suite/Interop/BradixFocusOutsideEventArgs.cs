namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix focus outside event args.
/// </summary>
public sealed class BradixFocusOutsideEventArgs : BradixPreventableEventArgs
{
    public BradixFocusOutsideEventArgs(BradixDelegatedFocusEvent originalEvent)
    {
        OriginalEvent = originalEvent;
    }

    /// <summary>
    /// Gets original event.
    /// </summary>
    public BradixDelegatedFocusEvent OriginalEvent { get; }
}
