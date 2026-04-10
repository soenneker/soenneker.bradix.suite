using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Bradix.Suite.DismissableLayer;
using Soenneker.Bradix.Suite.Interop;
using System.Threading.Tasks;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixDismissableLayerRenderTests : Bunit.BunitContext
{
    public BradixDismissableLayerRenderTests()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerDismissableLayer", _ => true);
        module.SetupVoid("updateDismissableLayer", _ => true);
        module.SetupVoid("unregisterDismissableLayer", _ => true);
        module.SetupVoid("registerDismissableLayerBranch", _ => true);
        module.SetupVoid("unregisterDismissableLayerBranch", _ => true);

        Services.AddScoped<BradixSuiteInterop>();
    }

    [Fact]
    public void Dismissable_layer_renders_child_content()
    {
        var cut = Render(CreateLayer());

        Assert.Contains("Layer content", cut.Markup);
    }

    [Fact]
    public async Task Pointer_down_outside_triggers_dismiss()
    {
        var cut = Render(CreateLayer(EventCallback.Factory.Create(this, () => _dismissed = true)));
        var layer = cut.FindComponent<BradixDismissableLayer>();

        await layer.Instance.HandlePointerDownOutsideAsync();

        Assert.True(_dismissed);
    }

    [Fact]
    public async Task Escape_can_be_ignored_when_dismiss_disabled()
    {
        var cut = Render(CreateLayer(EventCallback.Factory.Create(this, () => _dismissed = true), dismissOnEscapeKeyDown: false));
        var layer = cut.FindComponent<BradixDismissableLayer>();

        await layer.Instance.HandleEscapeKeyDownAsync();

        Assert.False(_dismissed);
    }

    [Fact]
    public void Branch_renders_child_content()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixDismissableLayerBranch>(0);
            builder.AddAttribute(1, nameof(BradixDismissableLayerBranch.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.AddContent(0, "Branch");
            }));
            builder.CloseComponent();
        });

        Assert.Contains("Branch", cut.Markup);
    }

    private bool _dismissed;

    private static RenderFragment CreateLayer(EventCallback onDismiss = default, bool dismissOnEscapeKeyDown = true)
    {
        return builder =>
        {
            builder.OpenComponent<BradixDismissableLayer>(0);
            builder.AddAttribute(1, nameof(BradixDismissableLayer.DismissOnEscapeKeyDown), dismissOnEscapeKeyDown);
            if (onDismiss.HasDelegate)
                builder.AddAttribute(2, nameof(BradixDismissableLayer.OnDismiss), onDismiss);
            builder.AddAttribute(3, nameof(BradixDismissableLayer.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.AddContent(0, "Layer content");
            }));
            builder.CloseComponent();
        };
    }
}
