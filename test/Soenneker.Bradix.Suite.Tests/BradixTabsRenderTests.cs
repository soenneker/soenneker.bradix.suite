using System.Collections.Generic;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit.Rendering;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixTabsRenderTests : BunitContext
{
    public BradixTabsRenderTests()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerRovingFocusNavigationKeys", _ => true).SetVoidResult();
        module.SetupVoid("unregisterRovingFocusNavigationKeys", _ => true).SetVoidResult();
        module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        module.SetupVoid("unregisterDelegatedInteraction", _ => true).SetVoidResult();
        module.SetupVoid("registerPresence", _ => true).SetVoidResult();
        module.SetupVoid("unregisterPresence", _ => true).SetVoidResult();
        module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "none", Display = "block" });
        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Tabs_render_active_trigger_and_panel_relationships()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateTabs(defaultValue: "tab1"));

        IReadOnlyList<IElement> buttons = cut.FindAll("button");
        IElement tabList = cut.Find("[role='tablist']");
        IElement panel = cut.Find("[role='tabpanel']");

        await Assert.That(tabList.GetAttribute("aria-orientation")).IsEqualTo("horizontal");
        await Assert.That(buttons[0].GetAttribute("aria-selected")).IsEqualTo("true");
        await Assert.That(buttons[0].GetAttribute("data-state")).IsEqualTo("active");
        await Assert.That(panel.Id).IsEqualTo(buttons[0].GetAttribute("aria-controls"));
        await Assert.That(panel.GetAttribute("aria-labelledby")).IsEqualTo(buttons[0].Id);
    }

    [Test]
    public async Task Manual_activation_does_not_switch_on_focus_but_does_on_enter()
    {
        string? requestedValue = null;

        IRenderedComponent<ContainerFragment> cut = Render(CreateTabs(
            defaultValue: "tab1",
            activationMode: BradixTabsActivationMode.Manual,
            onValueChange: EventCallback.Factory.Create<string?>(this, value => requestedValue = value)));

        IReadOnlyList<IElement> buttons = cut.FindAll("button");
        await buttons[2].FocusAsync();

        await Assert.That(cut.Markup).Contains("Content 1");
        await Assert.That(cut.Markup).DoesNotContain("Content 3");

        await buttons[2].KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        await Assert.That(requestedValue).IsEqualTo("tab3");
    }

    [Test]
    public async Task Automatic_activation_switches_on_focus()
    {
        string? requestedValue = null;

        IRenderedComponent<ContainerFragment> cut = Render(CreateTabs(
            defaultValue: "tab1",
            onValueChange: EventCallback.Factory.Create<string?>(this, value => requestedValue = value)));

        IReadOnlyList<IElement> buttons = cut.FindAll("button");
        await buttons[2].FocusAsync();

        await Assert.That(requestedValue).IsEqualTo("tab3");
    }

    [Test]
    public async Task Force_mount_keeps_inactive_panel_present_without_hidden_attribute()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateTabs(defaultValue: "tab1", forceMount: true));

        IElement tab2Panel = cut.Find("#" + cut.FindAll("button")[1].GetAttribute("aria-controls"));

        await Assert.That(tab2Panel.HasAttribute("hidden")).IsFalse();
        await Assert.That(tab2Panel.GetAttribute("tabindex")).IsEqualTo("0");
        await Assert.That(tab2Panel.TextContent).Contains("Content 2");
    }

    [Test]
    public async Task Inherited_direction_flips_horizontal_roving_focus_intent()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, nameof(BradixDirectionProvider.Dir), "rtl");
            builder.AddAttribute(2, nameof(BradixDirectionProvider.ChildContent), (RenderFragment) (contentBuilder =>
            {
                CreateTabs(defaultValue: "tab1")(contentBuilder);
            }));
            builder.CloseComponent();
        });

        IReadOnlyList<IElement> buttons = cut.FindAll("button");
        await buttons[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });
        buttons = cut.FindAll("button");

        await Assert.That(buttons[0].GetAttribute("tabindex")).IsEqualTo("-1");
        await Assert.That(buttons[2].GetAttribute("tabindex")).IsEqualTo("0");
    }

    [Test]
    public async Task Vertical_tabs_list_exposes_vertical_aria_orientation()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateTabs(defaultValue: "tab1", orientation: BradixOrientation.Vertical));

        await Assert.That(cut.Find("[role='tablist']").GetAttribute("aria-orientation")).IsEqualTo("vertical");
    }

    private static RenderFragment CreateTabs(string? defaultValue = null, BradixTabsActivationMode? activationMode = null, EventCallback<string?> onValueChange = default,
        bool forceMount = false, BradixOrientation? orientation = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixTabs>(0);

            if (defaultValue is not null)
                builder.AddAttribute(1, nameof(BradixTabs.DefaultValue), defaultValue);

            builder.AddAttribute(2, nameof(BradixTabs.ActivationMode), (object) (activationMode ?? BradixTabsActivationMode.Automatic));
            builder.AddAttribute(6, nameof(BradixTabs.Orientation), (object) (orientation ?? BradixOrientation.Horizontal));

            if (onValueChange.HasDelegate)
                builder.AddAttribute(3, nameof(BradixTabs.OnValueChange), onValueChange);

            builder.AddAttribute(4, nameof(BradixTabs.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixTabsList>(0);
                contentBuilder.AddAttribute(1, nameof(BradixTabsList.ChildContent), (RenderFragment) (listBuilder =>
                {
                    RenderTrigger(listBuilder, 0, "tab1");
                    RenderTrigger(listBuilder, 10, "tab2", disabled: true);
                    RenderTrigger(listBuilder, 20, "tab3");
                }));
                contentBuilder.CloseComponent();

                RenderContent(contentBuilder, 30, "tab1", forceMount);
                RenderContent(contentBuilder, 40, "tab2", forceMount);
                RenderContent(contentBuilder, 50, "tab3", forceMount);
            }));
            builder.CloseComponent();
        };
    }

    private static void RenderTrigger(RenderTreeBuilder builder, int sequence, string value, bool disabled = false)
    {
        builder.OpenComponent<BradixTabsTrigger>(sequence);
        builder.AddAttribute(sequence + 1, nameof(BradixTabsTrigger.Value), value);
        builder.AddAttribute(sequence + 2, nameof(BradixTabsTrigger.Disabled), disabled);
        builder.AddAttribute(sequence + 3, nameof(BradixTabsTrigger.ChildContent), (RenderFragment) (contentBuilder =>
        {
            contentBuilder.AddContent(0, value);
        }));
        builder.CloseComponent();
    }

    private static void RenderContent(RenderTreeBuilder builder, int sequence, string value, bool forceMount)
    {
        builder.OpenComponent<BradixTabsContent>(sequence);
        builder.AddAttribute(sequence + 1, nameof(BradixTabsContent.Value), value);
        builder.AddAttribute(sequence + 2, nameof(BradixTabsContent.ForceMount), forceMount);
        builder.AddAttribute(sequence + 3, nameof(BradixTabsContent.ChildContent), (RenderFragment) (contentBuilder =>
        {
            contentBuilder.AddContent(0, $"Content {value[^1]}");
        }));
        builder.CloseComponent();
    }
}