namespace Soenneker.Bradix;

public partial class BradixCheckboxProvider : BradixCheckbox, IBradixCheckboxProvider
{
    protected override bool RenderDefaultParts => false;
}
