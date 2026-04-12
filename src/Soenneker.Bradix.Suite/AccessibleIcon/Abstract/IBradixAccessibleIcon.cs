namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAccessibleIcon"/>.
/// </summary>
public interface IBradixAccessibleIcon
{
    /// <summary>
    /// Gets or sets the accessible label announced to assistive technologies while the icon is hidden from the accessibility tree.
    /// </summary>
    string Label { get; set; }
}
