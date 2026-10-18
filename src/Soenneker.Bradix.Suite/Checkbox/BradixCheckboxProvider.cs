namespace Soenneker.Bradix;

public class BradixCheckboxProvider : BradixCheckbox, IBradixCheckboxProvider
{
    protected override bool RenderDefaultParts => false;
}
