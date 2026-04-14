using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixCollapsibleRenderTests : BunitContext
{
    public BradixCollapsibleRenderTests()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("observeCollapsibleContent", _ => true).SetVoidResult();
        module.SetupVoid("unobserveCollapsibleContent", _ => true).SetVoidResult();
        module.SetupVoid("registerPresence", _ => true).SetVoidResult();
        module.SetupVoid("unregisterPresence", _ => true).SetVoidResult();
        module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        module.SetupVoid("unregisterDelegatedInteraction", _ => true).SetVoidResult();
        module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "none", Display = "block" });
        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Uncontrolled_toggle_updates_state_and_aria()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateCollapsible());

        IElement trigger = cut.Find("button");
        string contentId = trigger.GetAttribute("aria-controls")!;

        await Assert.That(trigger.GetAttribute("aria-expanded")).IsEqualTo("false");
        await Assert.That(trigger.GetAttribute("data-state")).IsEqualTo("closed");
        await Assert.That(cut.FindAll($"#{contentId}")).IsEmpty();
        await Assert.That(cut.Markup).DoesNotContain("Content");

        await trigger.ClickAsync();

        IElement content = cut.Find($"#{contentId}");
        await Assert.That(trigger.GetAttribute("aria-expanded")).IsEqualTo("true");
        await Assert.That(trigger.GetAttribute("data-state")).IsEqualTo("open");
        await Assert.That(content.GetAttribute("role")).IsEqualTo("region");
        await Assert.That(content.GetAttribute("aria-labelledby")).IsEqualTo(trigger.Id);
        await Assert.That(content.HasAttribute("hidden")).IsFalse();
        await Assert.That(cut.Markup).Contains("Content");
    }

    [Test]
    public async Task Trigger_uses_root_id_prefix_for_aria_wiring()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateCollapsible(rootId: "settings-collapse", open: true));

        IElement trigger = cut.Find("button");
        IElement content = cut.Find("#settings-collapse-content");

        await Assert.That(trigger.Id).IsEqualTo("settings-collapse-trigger");
        await Assert.That(trigger.GetAttribute("aria-controls")).IsEqualTo("settings-collapse-content");
        await Assert.That(content.GetAttribute("aria-labelledby")).IsEqualTo("settings-collapse-trigger");
    }

    [Test]
    public async Task Disabled_trigger_does_not_toggle_uncontrolled_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateCollapsible(triggerDisabled: true));

        IElement trigger = cut.Find("button");
        await trigger.ClickAsync();

        await Assert.That(trigger.HasAttribute("disabled")).IsTrue();
        await Assert.That(trigger.GetAttribute("data-state")).IsEqualTo("closed");
        await Assert.That(cut.Markup).DoesNotContain("Content");
    }

    [Test]
    public async Task Controlled_toggle_notifies_parent_without_closing_content()
    {
        bool? requestedOpen = null;

        IRenderedComponent<ContainerFragment> cut = Render(CreateCollapsible(
            open: true,
            onOpenChange: EventCallback.Factory.Create<bool>(this, open => requestedOpen = open)));

        IElement trigger = cut.Find("button");
        string contentId = trigger.GetAttribute("aria-controls")!;
        IElement content = cut.Find($"#{contentId}");

        await trigger.ClickAsync();

        await Assert.That(requestedOpen).IsEqualTo(false);
        await Assert.That(trigger.GetAttribute("aria-expanded")).IsEqualTo("true");
        await Assert.That(content.HasAttribute("hidden")).IsFalse();
        await Assert.That(cut.Markup).Contains("Content");
    }

    private static RenderFragment CreateCollapsible(bool? open = null, bool defaultOpen = false, EventCallback<bool> onOpenChange = default, string? rootId = null, bool triggerDisabled = false)
    {
        return builder =>
        {
            builder.OpenComponent<BradixCollapsible>(0);

            if (rootId is not null)
                builder.AddAttribute(10, nameof(BradixCollapsible.Id), rootId);

            if (open.HasValue)
                builder.AddAttribute(1, nameof(BradixCollapsible.Open), open.Value);

            builder.AddAttribute(2, nameof(BradixCollapsible.DefaultOpen), defaultOpen);

            if (onOpenChange.HasDelegate)
                builder.AddAttribute(3, nameof(BradixCollapsible.OnOpenChange), onOpenChange);

            builder.AddAttribute(4, nameof(BradixCollapsible.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixCollapsibleTrigger>(0);
                contentBuilder.AddAttribute(10, nameof(BradixCollapsibleTrigger.Disabled), triggerDisabled);
                contentBuilder.AddAttribute(1, nameof(BradixCollapsibleTrigger.ChildContent), (RenderFragment) (triggerBuilder =>
                {
                    triggerBuilder.AddContent(0, "Trigger");
                }));
                contentBuilder.CloseComponent();

                contentBuilder.OpenComponent<BradixCollapsibleContent>(2);
                contentBuilder.AddAttribute(3, nameof(BradixCollapsibleContent.ChildContent), (RenderFragment) (childBuilder =>
                {
                    childBuilder.AddContent(0, "Content");
                }));
                contentBuilder.CloseComponent();
            }));

            builder.CloseComponent();
        };
    }
}