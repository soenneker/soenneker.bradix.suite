using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Threading.Tasks;
using Bunit.Rendering;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixDirectionProviderRenderTests : BunitContext
{
    [Test]
    public async Task Direction_provider_cascades_rtl_value()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, nameof(BradixDirectionProvider.Dir), "rtl");
            builder.AddAttribute(2, nameof(BradixDirectionProvider.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixDirectionProbe>(0);
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });

        await Assert.That(cut.Find("[data-direction-probe]").GetAttribute("data-direction-probe")).IsEqualTo("rtl");
    }

    [Test]
    public async Task Direction_provider_normalizes_invalid_values_to_ltr()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, nameof(BradixDirectionProvider.Dir), "invalid");
            builder.AddAttribute(2, nameof(BradixDirectionProvider.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixDirectionProbe>(0);
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });

        await Assert.That(cut.Find("[data-direction-probe]").GetAttribute("data-direction-probe")).IsEqualTo("ltr");
    }

}
