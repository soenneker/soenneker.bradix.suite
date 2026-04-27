using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class ScrollAreaType
{
    public static readonly ScrollAreaType Hover = new("hover");
    public static readonly ScrollAreaType Scroll = new("scroll");
    public static readonly ScrollAreaType Auto = new("auto");
    public static readonly ScrollAreaType Always = new("always");
}
