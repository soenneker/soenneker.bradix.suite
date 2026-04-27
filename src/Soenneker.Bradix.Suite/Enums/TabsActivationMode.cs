using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class TabsActivationMode
{
    public static readonly TabsActivationMode Automatic = new("automatic");
    public static readonly TabsActivationMode Manual = new("manual");
}
