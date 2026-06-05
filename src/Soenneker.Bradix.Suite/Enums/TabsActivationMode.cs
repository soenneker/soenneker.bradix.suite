using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the tabs activation mode.
/// </summary>
[EnumValue<string>]
public sealed partial class TabsActivationMode
{
    /// <summary>
    /// The automatic.
    /// </summary>
    public static readonly TabsActivationMode Automatic = new("automatic");
    /// <summary>
    /// The manual.
    /// </summary>
    public static readonly TabsActivationMode Manual = new("manual");
}
