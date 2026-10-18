namespace Soenneker.Bradix;

public class BradixSwitchProvider : BradixSwitch, IBradixSwitchProvider
{
    protected override bool RenderDefaultParts => false;
}
