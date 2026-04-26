using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Soenneker.Bradix.Suite.Tests;

internal sealed class BradixDirectionProbe : ComponentBase
{
    [CascadingParameter(Name = "BradixDirection")]
    public string? Direction { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "data-direction-probe", Direction);
        builder.CloseElement();
    }
}
