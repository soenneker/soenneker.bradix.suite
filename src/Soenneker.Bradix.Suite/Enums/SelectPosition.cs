using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the select position.
/// </summary>
[EnumValue<string>]
public sealed partial class SelectPosition
{
    /// <summary>
    /// The item aligned.
    /// </summary>
    public static readonly SelectPosition ItemAligned = new("item-aligned");
    /// <summary>
    /// The popper.
    /// </summary>
    public static readonly SelectPosition Popper = new("popper");
}
