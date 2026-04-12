namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixCheckboxIndicator"/>.
/// </summary>
public interface IBradixCheckboxIndicator
{
    /// <summary>
    /// Gets or sets whether the indicator stays mounted while unchecked.
    /// </summary>
    bool ForceMount { get; set; }
}
