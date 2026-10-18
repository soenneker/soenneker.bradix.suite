using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

/// <summary>Controls position update scheduling.</summary>
[EnumValue<string>]
public sealed partial class PopperUpdatePositionStrategy
{
    /// <summary>Optimized behavior.</summary>
    public static readonly PopperUpdatePositionStrategy Optimized = new("optimized");
    /// <summary>Always behavior.</summary>
    public static readonly PopperUpdatePositionStrategy Always = new("always");
}
