namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix presence snapshot.
/// </summary>
public sealed class BradixPresenceSnapshot
{
    /// <summary>
    /// Gets or sets animation name.
    /// </summary>
    public string AnimationName { get; set; } = "none";

    /// <summary>
    /// Gets or sets display.
    /// </summary>
    public string Display { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether an animation is currently running or pending. A null value preserves compatibility with callers that only provide the animation name.
    /// </summary>
    public bool? HasActiveAnimation { get; set; }
}
