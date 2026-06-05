using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the swipe direction.
/// </summary>
[EnumValue<string>]
public sealed partial class SwipeDirection
{
    /// <summary>
    /// The up.
    /// </summary>
    public static readonly SwipeDirection Up = new("up");
    /// <summary>
    /// The down.
    /// </summary>
    public static readonly SwipeDirection Down = new("down");
    /// <summary>
    /// The left.
    /// </summary>
    public static readonly SwipeDirection Left = new("left");
    /// <summary>
    /// The right.
    /// </summary>
    public static readonly SwipeDirection Right = new("right");
}
