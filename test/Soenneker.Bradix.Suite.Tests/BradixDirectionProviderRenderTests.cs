using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixDirectionProviderRenderTests : BunitContext
{
    [Fact]
    public void Direction_provider_cascades_rtl_value()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, nameof(BradixDirectionProvider.Dir), "rtl");
            builder.AddAttribute(2, nameof(BradixDirectionProvider.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<DirectionProbe>(0);
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Equal("rtl", cut.Find("[data-direction-probe]").GetAttribute("data-direction-probe"));
    }

    [Fact]
    public void Direction_provider_normalizes_invalid_values_to_ltr()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, nameof(BradixDirectionProvider.Dir), "invalid");
            builder.AddAttribute(2, nameof(BradixDirectionProvider.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<DirectionProbe>(0);
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });

        Assert.Equal("ltr", cut.Find("[data-direction-probe]").GetAttribute("data-direction-probe"));
    }

    private sealed class DirectionProbe : ComponentBase
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
}
