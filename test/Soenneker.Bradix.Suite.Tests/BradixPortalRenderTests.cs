using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixPortalRenderTests : BunitContext
{
    public BradixPortalRenderTests()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("mountPortal", _ => true);
        module.SetupVoid("unmountPortal", _ => true);

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
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

    [Fact]
    public void Portal_does_not_remount_when_rerendered_without_container_change()
    {
        var cut = Render<PortalHost>();

        Assert.Single(JSInterop.Invocations, invocation => invocation.Identifier == "mountPortal");

        cut.Find("button").Click();

        Assert.Single(JSInterop.Invocations, invocation => invocation.Identifier == "mountPortal");
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

    private sealed class PortalHost : ComponentBase
    {
        private int _version;

        private void HandleClick()
        {
            _version++;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "button");
            builder.AddAttribute(1, "type", "button");
            builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, HandleClick));
            builder.AddContent(3, "Rerender");
            builder.CloseElement();

            builder.OpenComponent<BradixPortal>(4);
            builder.AddAttribute(5, nameof(BradixPortal.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.OpenElement(0, "div");
                contentBuilder.AddContent(1, $"Portaled content {_version}");
                contentBuilder.CloseElement();
            }));
            builder.CloseComponent();
        }
    }
}
