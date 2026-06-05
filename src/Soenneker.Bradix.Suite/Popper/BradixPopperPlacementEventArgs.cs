namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix popper placement event args.
/// </summary>
public sealed class BradixPopperPlacementEventArgs
{
    /// <summary>
    /// Gets or sets side.
    /// </summary>
    public required string Side { get; init; }

    /// <summary>
    /// Gets or sets align.
    /// </summary>
    public required string Align { get; init; }
}
