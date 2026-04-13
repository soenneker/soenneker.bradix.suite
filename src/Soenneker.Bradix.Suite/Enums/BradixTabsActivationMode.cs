using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class BradixTabsActivationMode
{
    public static readonly BradixTabsActivationMode Automatic = new("automatic");
    public static readonly BradixTabsActivationMode Manual = new("manual");
}
