using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Bradix.Suite.Interop;
using Soenneker.Bradix.Suite.Portal;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixPortalRenderTests : Bunit.BunitContext
{
    public BradixPortalRenderTests()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("mountPortal", _ => true);
        module.SetupVoid("unmountPortal", _ => true);

        Services.AddScoped<BradixSuiteInterop>();
    }

    [Fact]
    public void Portal_renders_child_content_inside_portal_host()
    {
        var cut = Render(CreatePortal());

        Assert.Contains("Portaled content", cut.Markup);
        Assert.Contains("data-bradix-portal", cut.Markup);
    }

    [Fact]
    public void Portal_supports_custom_container_selector_parameter()
    {
        var cut = Render(CreatePortal("#custom-container"));

        Assert.Contains("Portaled content", cut.Markup);
        Assert.Contains("data-bradix-portal", cut.Markup);
    }

    private static RenderFragment CreatePortal(string? containerSelector = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixPortal>(0);

            if (containerSelector is not null)
                builder.AddAttribute(1, nameof(BradixPortal.ContainerSelector), containerSelector);

            builder.AddAttribute(2, nameof(BradixPortal.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenElement(0, "div");
                contentBuilder.AddContent(1, "Portaled content");
                contentBuilder.CloseElement();
            }));
            builder.CloseComponent();
        };
    }
}
