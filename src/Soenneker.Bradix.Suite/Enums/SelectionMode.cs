using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class SelectionMode
{
    public static readonly SelectionMode Single = new("single");
    public static readonly SelectionMode Multiple = new("multiple");
}
