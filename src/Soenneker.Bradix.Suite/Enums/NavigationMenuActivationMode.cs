using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

/// <summary>Controls navigation menu activation.</summary>
[EnumValue<string>]
public sealed partial class NavigationMenuActivationMode
{
    /// <summary>Automatic behavior.</summary>
    public static readonly NavigationMenuActivationMode Automatic = new("automatic");
    /// <summary>Manual behavior.</summary>
    public static readonly NavigationMenuActivationMode Manual = new("manual");
}
