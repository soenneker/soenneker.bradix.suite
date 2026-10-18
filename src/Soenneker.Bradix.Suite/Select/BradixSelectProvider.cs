namespace Soenneker.Bradix;

public class BradixSelectProvider : BradixSelect, IBradixSelectProvider
{
    protected override bool RenderDefaultParts => false;
}
