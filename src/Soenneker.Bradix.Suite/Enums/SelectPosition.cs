using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class SelectPosition
{
    public static readonly SelectPosition ItemAligned = new("item-aligned");
    public static readonly SelectPosition Popper = new("popper");
}
