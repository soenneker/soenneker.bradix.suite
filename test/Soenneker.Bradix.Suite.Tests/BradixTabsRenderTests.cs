using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Bradix.Suite.Direction;
using Soenneker.Bradix.Suite.Id;
using Soenneker.Bradix.Suite.Interop;
using Soenneker.Bradix.Suite.Tabs;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixTabsRenderTests : Bunit.BunitContext
{
    public BradixTabsRenderTests()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerRovingFocusNavigationKeys", _ => true);
        module.SetupVoid("unregisterRovingFocusNavigationKeys", _ => true);

        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
        Services.AddScoped<BradixSuiteInterop>();
    }

    [Fact]
    public void Tabs_render_active_trigger_and_panel_relationships()
    {
        var cut = Render(CreateTabs(defaultValue: "tab1"));

        var buttons = cut.FindAll("button");
        var panel = cut.Find("[role='tabpanel']");

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
            activationMode: "manual",
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
    public void Force_mount_keeps_inactive_panel_in_dom_hidden()
    {
        var cut = Render(CreateTabs(defaultValue: "tab1", forceMount: true));

        var tab2Panel = cut.Find("#" + cut.FindAll("button")[1].GetAttribute("aria-controls"));

        Assert.True(tab2Panel.HasAttribute("hidden"));
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

    private static RenderFragment CreateTabs(string? defaultValue = null, string activationMode = "automatic", EventCallback<string?> onValueChange = default, bool forceMount = false)
    {
        return builder =>
        {
            builder.OpenComponent<BradixTabs>(0);

            if (defaultValue is not null)
                builder.AddAttribute(1, nameof(BradixTabs.DefaultValue), defaultValue);

            builder.AddAttribute(2, nameof(BradixTabs.ActivationMode), activationMode);

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
