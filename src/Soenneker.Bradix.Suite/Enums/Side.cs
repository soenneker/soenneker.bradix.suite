using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class Side
{
    public static readonly Side Top = new("top");
    public static readonly Side Right = new("right");
    public static readonly Side Bottom = new("bottom");
    public static readonly Side Left = new("left");
}
