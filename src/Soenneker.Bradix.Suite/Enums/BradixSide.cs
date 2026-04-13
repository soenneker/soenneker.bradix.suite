using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class BradixSide
{
    public static readonly BradixSide Top = new("top");
    public static readonly BradixSide Right = new("right");
    public static readonly BradixSide Bottom = new("bottom");
    public static readonly BradixSide Left = new("left");
}
