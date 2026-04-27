using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class ToastType
{
    public static readonly ToastType Foreground = new("foreground");
    public static readonly ToastType Background = new("background");
}
