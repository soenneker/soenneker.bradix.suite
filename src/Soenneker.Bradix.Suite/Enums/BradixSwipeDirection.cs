using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class BradixSwipeDirection
{
    public static readonly BradixSwipeDirection Up = new("up");
    public static readonly BradixSwipeDirection Down = new("down");
    public static readonly BradixSwipeDirection Left = new("left");
    public static readonly BradixSwipeDirection Right = new("right");
}
