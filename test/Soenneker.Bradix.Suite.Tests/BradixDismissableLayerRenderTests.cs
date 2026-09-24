using System.Threading.Tasks;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixDismissableLayerRenderTests : BunitContext
{
    public BradixDismissableLayerRenderTests()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerDismissableLayer", _ => true);
        module.SetupVoid("updateDismissableLayer", _ => true);
        module.SetupVoid("unregisterDismissableLayer", _ => true);
        module.SetupVoid("registerDismissableLayerBranch", _ => true);
        module.SetupVoid("unregisterDismissableLayerBranch", _ => true);

        Services.AddBradixTestInterops();
    }

    [Test]
    public async Task Dismissable_layer_renders_child_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateLayer());

        await Assert.That(cut.Markup).Contains("Layer content");
    }

    [Test]
    public async Task Pointer_down_outside_triggers_dismiss()
    {
        var dismissed = false;

        IRenderedComponent<ContainerFragment> cut = Render(CreateLayer(EventCallback.Factory.Create(this, () => dismissed = true)));
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();

        await layer.Instance.HandlePointerDownOutside();

        await Assert.That(dismissed).IsTrue();
    }

    [Test]
    public async Task Escape_can_be_ignored_when_dismiss_disabled()
    {
        var dismissed = false;

        IRenderedComponent<ContainerFragment> cut = Render(CreateLayer(EventCallback.Factory.Create(this, () => dismissed = true), dismissOnEscapeKeyDown: false));
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();

        await layer.Instance.HandleEscapeKeyDown();

        await Assert.That(dismissed).IsFalse();
    }

    [Test]
    public async Task Escape_reports_prevent_default_when_it_dismisses()
    {
        var dismissed = false;

        IRenderedComponent<ContainerFragment> cut = Render(CreateLayer(EventCallback.Factory.Create(this, () => dismissed = true)));
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();

        bool shouldPreventDefault = await layer.Instance.HandleEscapeKeyDown();

        await Assert.That(dismissed).IsTrue();
        await Assert.That(shouldPreventDefault).IsTrue();
    }

    [Test]
    public async Task Escape_does_not_report_prevent_default_when_handler_prevents_dismiss()
    {
        var dismissed = false;

        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixDismissableLayer>(0);
            builder.AddAttribute(1, nameof(BradixDismissableLayer.OnEscapeKeyDownDetailed), EventCallback.Factory.Create<BradixEscapeKeyDownEventArgs>(this, args => args.PreventDefault()));
            builder.AddAttribute(2, nameof(BradixDismissableLayer.OnDismiss), EventCallback.Factory.Create(this, () => dismissed = true));
            builder.AddAttribute(3, nameof(BradixDismissableLayer.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.AddContent(0, "Layer content");
            }));
            builder.CloseComponent();
        });

        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();
        bool shouldPreventDefault = await layer.Instance.HandleEscapeKeyDown();

        await Assert.That(dismissed).IsFalse();
        await Assert.That(shouldPreventDefault).IsFalse();
    }

    [Test]
    public async Task Focus_outside_triggers_interact_and_dismiss_callbacks()
    {
        var dismissed = false;

        var focusOutside = false;
        var interactOutside = false;

        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixDismissableLayer>(0);
            builder.AddAttribute(1, nameof(BradixDismissableLayer.OnFocusOutside), EventCallback.Factory.Create(this, () => focusOutside = true));
            builder.AddAttribute(2, nameof(BradixDismissableLayer.OnInteractOutside), EventCallback.Factory.Create(this, () => interactOutside = true));
            builder.AddAttribute(3, nameof(BradixDismissableLayer.OnDismiss), EventCallback.Factory.Create(this, () => dismissed = true));
            builder.AddAttribute(4, nameof(BradixDismissableLayer.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.AddContent(0, "Layer content");
            }));
            builder.CloseComponent();
        });

        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();
        await layer.Instance.HandleFocusOutside();

        await Assert.That(focusOutside).IsTrue();
        await Assert.That(interactOutside).IsTrue();
        await Assert.That(dismissed).IsTrue();
    }

    [Test]
    public async Task Pointer_down_outside_prevent_default_prevents_dismiss()
    {
        var dismissed = false;

        var interactOutside = false;

        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixDismissableLayer>(0);
            builder.AddAttribute(1, nameof(BradixDismissableLayer.OnPointerDownOutsideDetailed),
                EventCallback.Factory.Create<BradixPointerDownOutsideEventArgs>(this, args => args.PreventDefault()));
            builder.AddAttribute(2, nameof(BradixDismissableLayer.OnInteractOutside), EventCallback.Factory.Create(this, () => interactOutside = true));
            builder.AddAttribute(3, nameof(BradixDismissableLayer.OnDismiss), EventCallback.Factory.Create(this, () => dismissed = true));
            builder.AddAttribute(4, nameof(BradixDismissableLayer.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.AddContent(0, "Layer content");
            }));
            builder.CloseComponent();
        });

        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();
        await layer.Instance.HandlePointerDownOutside();

        await Assert.That(interactOutside).IsTrue();
        await Assert.That(dismissed).IsFalse();
    }

    [Test]
    public async Task Interact_outside_prevent_default_prevents_focus_dismiss()
    {
        var dismissed = false;

        var focusOutside = false;

        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixDismissableLayer>(0);
            builder.AddAttribute(1, nameof(BradixDismissableLayer.OnFocusOutside), EventCallback.Factory.Create(this, () => focusOutside = true));
            builder.AddAttribute(2, nameof(BradixDismissableLayer.OnInteractOutsideDetailed),
                EventCallback.Factory.Create<BradixInteractOutsideEventArgs>(this, args => args.PreventDefault()));
            builder.AddAttribute(3, nameof(BradixDismissableLayer.OnDismiss), EventCallback.Factory.Create(this, () => dismissed = true));
            builder.AddAttribute(4, nameof(BradixDismissableLayer.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.AddContent(0, "Layer content");
            }));
            builder.CloseComponent();
        });

        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();
        await layer.Instance.HandleFocusOutside();

        await Assert.That(focusOutside).IsTrue();
        await Assert.That(dismissed).IsFalse();
    }

    [Test]
    public async Task Branch_renders_child_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixDismissableLayerBranch>(0);
            builder.AddAttribute(1, nameof(BradixDismissableLayerBranch.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.AddContent(0, "Branch");
            }));
            builder.CloseComponent();
        });

        await Assert.That(cut.Markup).Contains("Branch");
    }

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
