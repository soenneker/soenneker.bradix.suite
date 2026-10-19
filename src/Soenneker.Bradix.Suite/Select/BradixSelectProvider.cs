namespace Soenneker.Bradix;

public partial class BradixSelectProvider : BradixSelect, IBradixSelectProvider
{
    protected override bool RenderDefaultParts => false;
}
