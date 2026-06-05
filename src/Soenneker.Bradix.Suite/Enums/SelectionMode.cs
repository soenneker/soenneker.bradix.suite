using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the selection mode.
/// </summary>
[EnumValue<string>]
public sealed partial class SelectionMode
{
    /// <summary>
    /// The single.
    /// </summary>
    public static readonly SelectionMode Single = new("single");
    /// <summary>
    /// The multiple.
    /// </summary>
    public static readonly SelectionMode Multiple = new("multiple");
}
