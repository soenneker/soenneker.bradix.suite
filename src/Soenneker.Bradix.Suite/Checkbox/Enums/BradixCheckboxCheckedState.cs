using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

[EnumValue<string>]
public sealed partial class BradixCheckboxCheckedState
{
    public static readonly BradixCheckboxCheckedState Unchecked = new("unchecked");
    public static readonly BradixCheckboxCheckedState Checked = new("checked");
    public static readonly BradixCheckboxCheckedState Indeterminate = new("indeterminate");
}
