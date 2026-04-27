using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class Orientation
{
    public static readonly Orientation Horizontal = new("horizontal");
    public static readonly Orientation Vertical = new("vertical");
}
