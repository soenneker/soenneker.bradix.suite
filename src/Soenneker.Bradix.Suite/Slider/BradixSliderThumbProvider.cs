using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

public partial class BradixSliderThumbProvider : BradixSliderThumb, IBradixSliderThumbProvider
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    protected override bool RenderDefaultParts => false;
    protected override RenderFragment? ComposedContent => ChildContent;
}
