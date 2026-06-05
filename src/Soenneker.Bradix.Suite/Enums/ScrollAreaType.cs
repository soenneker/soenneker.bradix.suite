using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the scroll area type.
/// </summary>
[EnumValue<string>]
public sealed partial class ScrollAreaType
{
    /// <summary>
    /// The hover.
    /// </summary>
    public static readonly ScrollAreaType Hover = new("hover");
    /// <summary>
    /// The scroll.
    /// </summary>
    public static readonly ScrollAreaType Scroll = new("scroll");
    /// <summary>
    /// The auto.
    /// </summary>
    public static readonly ScrollAreaType Auto = new("auto");
    /// <summary>
    /// The always.
    /// </summary>
    public static readonly ScrollAreaType Always = new("always");
}
