using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class BradixOrientation
{
    public static readonly BradixOrientation Horizontal = new("horizontal");
    public static readonly BradixOrientation Vertical = new("vertical");
}
