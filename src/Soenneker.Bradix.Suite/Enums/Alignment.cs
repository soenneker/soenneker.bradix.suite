using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class Alignment
{
    public static readonly Alignment Start = new("start");
    public static readonly Alignment Center = new("center");
    public static readonly Alignment End = new("end");
}