using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the toast type.
/// </summary>
[EnumValue<string>]
public sealed partial class ToastType
{
    /// <summary>
    /// The foreground.
    /// </summary>
    public static readonly ToastType Foreground = new("foreground");
    /// <summary>
    /// The background.
    /// </summary>
    public static readonly ToastType Background = new("background");
}
