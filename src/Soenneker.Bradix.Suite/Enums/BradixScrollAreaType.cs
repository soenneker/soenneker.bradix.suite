using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class BradixScrollAreaType
{
    public static readonly BradixScrollAreaType Hover = new("hover");
    public static readonly BradixScrollAreaType Scroll = new("scroll");
    public static readonly BradixScrollAreaType Auto = new("auto");
    public static readonly BradixScrollAreaType Always = new("always");
}
