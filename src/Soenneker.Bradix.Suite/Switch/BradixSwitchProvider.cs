namespace Soenneker.Bradix;

public partial class BradixSwitchProvider : BradixSwitch, IBradixSwitchProvider
{
    protected override bool RenderDefaultParts => false;
}
