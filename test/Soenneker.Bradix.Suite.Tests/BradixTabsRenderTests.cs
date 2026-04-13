using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixTabsRenderTests : BunitContext
{
    public BradixTabsRenderTests()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
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

    [Fact]
    public void Tabs_render_active_trigger_and_panel_relationships()
    {
        var cut = Render(CreateTabs(defaultValue: "tab1"));

        var buttons = cut.FindAll("button");
        var tabList = cut.Find("[role='tablist']");
        var panel = cut.Find("[role='tabpanel']");

        Assert.Equal("horizontal", tabList.GetAttribute("aria-orientation"));
        Assert.Equal("true", buttons[0].GetAttribute("aria-selected"));
        Assert.Equal("active", buttons[0].GetAttribute("data-state"));
        Assert.Equal(buttons[0].GetAttribute("aria-controls"), panel.Id);
        Assert.Equal(buttons[0].Id, panel.GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void Manual_activation_does_not_switch_on_focus_but_does_on_enter()
    {
        string? requestedValue = null;

        var cut = Render(CreateTabs(
            defaultValue: "tab1",
            activationMode: BradixTabsActivationMode.Manual,
            onValueChange: EventCallback.Factory.Create<string?>(this, value => requestedValue = value)));

        var buttons = cut.FindAll("button");
        buttons[2].Focus();

        Assert.Contains("Content 1", cut.Markup);
        Assert.DoesNotContain("Content 3", cut.Markup);

        buttons[2].KeyDown(new KeyboardEventArgs { Key = "Enter" });

        Assert.Equal("tab3", requestedValue);
    }

    [Fact]
    public void Automatic_activation_switches_on_focus()
    {
        string? requestedValue = null;

        var cut = Render(CreateTabs(
            defaultValue: "tab1",
            onValueChange: EventCallback.Factory.Create<string?>(this, value => requestedValue = value)));

        var buttons = cut.FindAll("button");
        buttons[2].Focus();

        Assert.Equal("tab3", requestedValue);
    }

    [Fact]
    public void Force_mount_keeps_inactive_panel_present_without_hidden_attribute()
    {
        var cut = Render(CreateTabs(defaultValue: "tab1", forceMount: true));

        var tab2Panel = cut.Find("#" + cut.FindAll("button")[1].GetAttribute("aria-controls"));

        Assert.False(tab2Panel.HasAttribute("hidden"));
        Assert.Equal("0", tab2Panel.GetAttribute("tabindex"));
        Assert.Contains("Content 2", tab2Panel.TextContent);
    }

    [Fact]
    public void Inherited_direction_flips_horizontal_roving_focus_intent()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, nameof(BradixDirectionProvider.Dir), "rtl");
            builder.AddAttribute(2, nameof(BradixDirectionProvider.ChildContent), (RenderFragment) (contentBuilder =>
            {
                CreateTabs(defaultValue: "tab1")(contentBuilder);
            }));
            builder.CloseComponent();
        });

        var buttons = cut.FindAll("button");
        buttons[0].KeyDown(new KeyboardEventArgs { Key = "ArrowLeft" });
        buttons = cut.FindAll("button");

        Assert.Equal("-1", buttons[0].GetAttribute("tabindex"));
        Assert.Equal("0", buttons[2].GetAttribute("tabindex"));
    }

    [Fact]
    public void Vertical_tabs_list_exposes_vertical_aria_orientation()
    {
        var cut = Render(CreateTabs(defaultValue: "tab1", orientation: BradixOrientation.Vertical));

        Assert.Equal("vertical", cut.Find("[role='tablist']").GetAttribute("aria-orientation"));
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
