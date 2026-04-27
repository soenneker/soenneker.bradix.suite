using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class SwipeDirection
{
    public static readonly SwipeDirection Up = new("up");
    public static readonly SwipeDirection Down = new("down");
    public static readonly SwipeDirection Left = new("left");
    public static readonly SwipeDirection Right = new("right");
}
