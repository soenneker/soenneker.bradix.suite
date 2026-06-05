using Soenneker.Gen.EnumValues;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix checkbox checked state.
/// </summary>
[EnumValue<string>]
public sealed partial class BradixCheckboxCheckedState
{
    /// <summary>
    /// The unchecked.
    /// </summary>
    public static readonly BradixCheckboxCheckedState Unchecked = new("unchecked");
    /// <summary>
    /// The checked.
    /// </summary>
    public static readonly BradixCheckboxCheckedState Checked = new("checked");
    /// <summary>
    /// The indeterminate.
    /// </summary>
    public static readonly BradixCheckboxCheckedState Indeterminate = new("indeterminate");
}
