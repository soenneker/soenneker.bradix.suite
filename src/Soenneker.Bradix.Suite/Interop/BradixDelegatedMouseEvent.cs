namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix delegated mouse event.
/// </summary>
public sealed class BradixDelegatedMouseEvent
{
    /// <summary>
    /// Gets or sets detail.
    /// </summary>
    public long Detail { get; set; }
    /// <summary>
    /// Gets or sets button.
    /// </summary>
    public long Button { get; set; }
    /// <summary>
    /// Gets or sets pointer id.
    /// </summary>
    public long PointerId { get; set; }
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
    /// Gets or sets a value indicating whether default prevented.
    /// </summary>
    public bool DefaultPrevented { get; set; }
    /// <summary>
    /// Gets or sets page x.
    /// </summary>
    public double PageX { get; set; }
    /// <summary>
    /// Gets or sets page y.
    /// </summary>
    public double PageY { get; set; }
    /// <summary>
    /// Gets or sets pointer type.
    /// </summary>
    public string PointerType { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets target id.
    /// </summary>
    public string TargetId { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets ancestor ids.
    /// </summary>
    public string[] AncestorIds { get; set; } = [];
    /// <summary>
    /// Gets or sets a value indicating whether active element inside layer.
    /// </summary>
    public bool ActiveElementInsideLayer { get; set; }
}
