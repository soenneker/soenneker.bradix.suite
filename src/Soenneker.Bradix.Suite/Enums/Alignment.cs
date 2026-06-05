using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the alignment.
/// </summary>
[EnumValue<string>]
public sealed partial class Alignment
{
    /// <summary>
    /// The start.
    /// </summary>
    public static readonly Alignment Start = new("start");
    /// <summary>
    /// The center.
    /// </summary>
    public static readonly Alignment Center = new("center");
    /// <summary>
    /// The end.
    /// </summary>
    public static readonly Alignment End = new("end");
}