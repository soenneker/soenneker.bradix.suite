namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAspectRatio"/>.
/// </summary>
public interface IBradixAspectRatio
{
    /// <summary>
    /// Gets or sets the width-to-height ratio of the content box.
    /// </summary>
    double Ratio { get; set; }
}
