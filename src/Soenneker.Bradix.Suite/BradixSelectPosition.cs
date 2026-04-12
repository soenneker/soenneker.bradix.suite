using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class BradixSelectPosition
{
    public static readonly BradixSelectPosition ItemAligned = new("item-aligned");
    public static readonly BradixSelectPosition Popper = new("popper");
}
