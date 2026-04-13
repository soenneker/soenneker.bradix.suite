using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class BradixSelectionMode
{
    public static readonly BradixSelectionMode Single = new("single");
    public static readonly BradixSelectionMode Multiple = new("multiple");
}
