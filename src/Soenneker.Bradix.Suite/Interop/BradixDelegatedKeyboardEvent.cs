namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix delegated keyboard event.
/// </summary>
public sealed class BradixDelegatedKeyboardEvent
{
    /// <summary>
    /// Gets or sets key.
    /// </summary>
    public string Key { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets code.
    /// </summary>
    public string Code { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets a value indicating whether ctrl key.
    /// </summary>
    public bool CtrlKey { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether shift key.
    /// </summary>
    public bool ShiftKey { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether alt key.
    /// </summary>
    public bool AltKey { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether meta key.
    /// </summary>
    public bool MetaKey { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether repeat.
    /// </summary>
    public bool Repeat { get; set; }
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
    /// Gets or sets closest menubar content id.
    /// </summary>
    public string ClosestMenubarContentId { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets a value indicating whether the instance is menubar sub trigger.
    /// </summary>
    public bool IsMenubarSubTrigger { get; set; }
}
