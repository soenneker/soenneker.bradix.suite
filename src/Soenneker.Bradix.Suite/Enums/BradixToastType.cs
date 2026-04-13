using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class BradixToastType
{
    public static readonly BradixToastType Foreground = new("foreground");
    public static readonly BradixToastType Background = new("background");
}
