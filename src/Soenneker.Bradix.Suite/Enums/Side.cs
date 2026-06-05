using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the side.
/// </summary>
[EnumValue<string>]
public sealed partial class Side
{
    /// <summary>
    /// The top.
    /// </summary>
    public static readonly Side Top = new("top");
    /// <summary>
    /// The right.
    /// </summary>
    public static readonly Side Right = new("right");
    /// <summary>
    /// The bottom.
    /// </summary>
    public static readonly Side Bottom = new("bottom");
    /// <summary>
    /// The left.
    /// </summary>
    public static readonly Side Left = new("left");
}
