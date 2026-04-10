using Bunit;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Bradix.Suite.Direction;
using Soenneker.Bradix.Suite.Id;
using Soenneker.Bradix.Suite.Interop;
using Soenneker.Bradix.Suite.RadioGroup;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixRadioGroupRenderTests : Bunit.BunitContext
{
    public BradixRadioGroupRenderTests()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerRovingFocusNavigationKeys", _ => true);
        module.SetupVoid("unregisterRovingFocusNavigationKeys", _ => true);
        module.SetupVoid("registerRadioGroupItemKeys", _ => true);
        module.SetupVoid("unregisterRadioGroupItemKeys", _ => true);

        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
        Services.AddScoped<BradixSuiteInterop>();
    }

    [Fact]
    public void Radio_group_renders_checked_state_and_hidden_input()
    {
        var cut = Render(CreateRadioGroup(defaultValue: "one", name: "plan", required: true));

        var group = cut.Find("[role='radiogroup']");
        var buttons = cut.FindAll("button");
        var inputs = cut.FindAll("input[type='radio']");

        Assert.Equal("true", group.GetAttribute("aria-required"));
        Assert.Equal("true", buttons[0].GetAttribute("aria-checked"));
        Assert.Equal("checked", buttons[0].GetAttribute("data-state"));
        Assert.Equal("plan", inputs[0].GetAttribute("name"));
        Assert.NotNull(inputs.Single(input => input.HasAttribute("checked")));
    }

    [Fact]
    public void Clicking_item_updates_uncontrolled_selection()
    {
        var cut = Render(CreateRadioGroup(defaultValue: "one"));

        var buttons = cut.FindAll("button");
        buttons[2].Click();
        buttons = cut.FindAll("button");

        Assert.Equal("false", buttons[0].GetAttribute("aria-checked"));
        Assert.Equal("true", buttons[2].GetAttribute("aria-checked"));
    }

    [Fact]
    public void Arrow_navigation_moves_focus_and_selects_target()
    {
        string? requestedValue = null;

        var cut = Render(CreateRadioGroup(
            defaultValue: "one",
            onValueChange: EventCallback.Factory.Create<string?>(this, value => requestedValue = value)));

        var buttons = cut.FindAll("button");
        buttons[0].KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        buttons = cut.FindAll("button");
        buttons[2].Focus();
        buttons = cut.FindAll("button");

        Assert.Equal("three", requestedValue);
        Assert.Equal("-1", buttons[0].GetAttribute("tabindex"));
        Assert.Equal("0", buttons[2].GetAttribute("tabindex"));
        Assert.Equal("true", buttons[2].GetAttribute("aria-checked"));
    }

    [Fact]
    public void Enter_does_not_change_selection()
    {
        string? requestedValue = null;

        var cut = Render(CreateRadioGroup(
            defaultValue: "one",
            onValueChange: EventCallback.Factory.Create<string?>(this, value => requestedValue = value)));

        var buttons = cut.FindAll("button");
        buttons[2].KeyDown(new KeyboardEventArgs { Key = "Enter" });

        Assert.Null(requestedValue);
        Assert.Contains("one", cut.Markup);
    }

    [Fact]
    public void Force_mount_renders_all_indicators()
    {
        var cut = Render(CreateRadioGroup(forceMountIndicator: true));

        Assert.Equal(3, cut.FindAll("span").Count);
    }

    [Fact]
    public void Inherited_direction_flips_horizontal_navigation()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, nameof(BradixDirectionProvider.Dir), "rtl");
            builder.AddAttribute(2, nameof(BradixDirectionProvider.ChildContent), (RenderFragment) (contentBuilder =>
            {
                CreateRadioGroup(defaultValue: "one", includeDisabledMiddle: false)(contentBuilder);
            }));
            builder.CloseComponent();
        });

        var buttons = cut.FindAll("button");
        buttons[0].KeyDown(new KeyboardEventArgs { Key = "ArrowLeft" });
        buttons = cut.FindAll("button");
        buttons[2].Focus();
        buttons = cut.FindAll("button");

        Assert.Equal("true", buttons[2].GetAttribute("aria-checked"));
    }

    private static RenderFragment CreateRadioGroup(string? defaultValue = null, string? name = null, bool required = false, EventCallback<string?> onValueChange = default, bool forceMountIndicator = false, bool includeDisabledMiddle = true)
    {
        return builder =>
        {
            builder.OpenComponent<BradixRadioGroup>(0);

            if (defaultValue is not null)
                builder.AddAttribute(1, nameof(BradixRadioGroup.DefaultValue), defaultValue);

            if (name is not null)
                builder.AddAttribute(2, nameof(BradixRadioGroup.Name), name);

            builder.AddAttribute(3, nameof(BradixRadioGroup.Required), required);

            if (onValueChange.HasDelegate)
                builder.AddAttribute(4, nameof(BradixRadioGroup.OnValueChange), onValueChange);

            builder.AddAttribute(5, nameof(BradixRadioGroup.ChildContent), (RenderFragment) (contentBuilder =>
            {
                RenderItem(contentBuilder, 0, "one", forceMountIndicator);
                RenderItem(contentBuilder, 10, "two", forceMountIndicator, includeDisabledMiddle);
                RenderItem(contentBuilder, 20, "three", forceMountIndicator);
            }));
            builder.CloseComponent();
        };
    }

    private static void RenderItem(RenderTreeBuilder builder, int sequence, string value, bool forceMountIndicator, bool disabled = false)
    {
        builder.OpenComponent<BradixRadioGroupItem>(sequence);
        builder.AddAttribute(sequence + 1, nameof(BradixRadioGroupItem.Value), value);
        builder.AddAttribute(sequence + 2, nameof(BradixRadioGroupItem.InputValue), value);
        builder.AddAttribute(sequence + 3, nameof(BradixRadioGroupItem.Disabled), disabled);
        builder.AddAttribute(sequence + 4, nameof(BradixRadioGroupItem.ChildContent), (RenderFragment) (contentBuilder =>
        {
            contentBuilder.OpenComponent<BradixRadioGroupIndicator>(0);
            contentBuilder.AddAttribute(1, nameof(BradixRadioGroupIndicator.ForceMount), forceMountIndicator);
            contentBuilder.CloseComponent();
        }));
        builder.CloseComponent();
    }
}
