using Bunit;
using System.Threading.Tasks;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixPortalRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixPortalRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("mountPortal", _ => true).SetVoidResult();
        _module.SetupVoid("unmountPortal", _ => true).SetVoidResult();

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Portal_renders_child_content_inside_portal_host()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreatePortal());

        await Assert.That(cut.Markup).Contains("Portaled content");
        await Assert.That(cut.Markup).DoesNotContain("data-bradix-portal");
    }

    [Test]
    public async Task Portal_supports_custom_container_selector_parameter()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreatePortal("#custom-container"));

        await Assert.That(cut.Markup).Contains("Portaled content");
        await Assert.That(cut.Markup).DoesNotContain("data-bradix-portal");
    }

    [Test]
    public async Task Portal_does_not_remount_when_rerendered_without_container_change()
    {
        IRenderedComponent<PortalHost> cut = Render<PortalHost>();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(_module.Invocations).HasSingleItem();
        });

        await cut.Find("button").ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(_module.Invocations).HasSingleItem();
        });
    }

    [Test]
    public async Task Portal_unmounts_when_disposed()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreatePortal());

        await Assert.That(_module.Invocations).HasSingleItem();

        await cut.InvokeAsync(() => cut.FindComponent<BradixPortal>().Instance.DisposeAsync().AsTask());

        await Assert.That(_module.Invocations.Count).IsEqualTo(2);
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