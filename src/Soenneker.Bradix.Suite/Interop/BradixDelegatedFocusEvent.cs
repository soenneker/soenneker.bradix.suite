namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix delegated focus event.
/// </summary>
public sealed class BradixDelegatedFocusEvent
{
    /// <summary>
    /// Gets or sets a value indicating whether default prevented.
    /// </summary>
    public bool DefaultPrevented { get; set; }
    /// <summary>
    /// Gets or sets target id.
    /// </summary>
    public string TargetId { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets ancestor ids.
    /// </summary>
    public string[] AncestorIds { get; set; } = [];
    /// <summary>
    /// Gets or sets related target id.
    /// </summary>
    public string RelatedTargetId { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets related target ancestor ids.
    /// </summary>
    public string[] RelatedTargetAncestorIds { get; set; } = [];
}
