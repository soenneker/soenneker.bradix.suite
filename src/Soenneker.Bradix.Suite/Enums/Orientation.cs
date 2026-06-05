using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the orientation.
/// </summary>
[EnumValue<string>]
public sealed partial class Orientation
{
    /// <summary>
    /// The horizontal.
    /// </summary>
    public static readonly Orientation Horizontal = new("horizontal");
    /// <summary>
    /// The vertical.
    /// </summary>
    public static readonly Orientation Vertical = new("vertical");
}
