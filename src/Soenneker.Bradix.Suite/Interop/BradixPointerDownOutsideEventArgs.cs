namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix pointer down outside event args.
/// </summary>
public sealed class BradixPointerDownOutsideEventArgs : BradixPreventableEventArgs
{
    public BradixPointerDownOutsideEventArgs(BradixDelegatedMouseEvent originalEvent)
    {
        OriginalEvent = originalEvent;
    }

    /// <summary>
    /// Gets original event.
    /// </summary>
    public BradixDelegatedMouseEvent OriginalEvent { get; }
}
