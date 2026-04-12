using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class BradixAlignment
{
    public static readonly BradixAlignment Start = new("start");
    public static readonly BradixAlignment Center = new("center");
    public static readonly BradixAlignment End = new("end");
}
