using System.Collections.Generic;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixToggleGroupRenderTests : BunitContext
{
    public BradixToggleGroupRenderTests()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerRovingFocusNavigationKeys", _ => true).SetVoidResult();
        module.SetupVoid("unregisterRovingFocusNavigationKeys", _ => true).SetVoidResult();
        module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        module.SetupVoid("unregisterDelegatedInteraction", _ => true).SetVoidResult();
        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Fact]
    public void Single_group_toggles_between_values_and_radio_semantics()
    {
        string? requestedValue = null;

        var cut = Render(CreateSingleGroup(EventCallback.Factory.Create<string?>(this, value => requestedValue = value)));
        var group = cut.Find("[role='radiogroup']");

        var buttons = cut.FindAll("button");

        Assert.Equal("horizontal", group.GetAttribute("aria-orientation"));
        Assert.Equal("radio", buttons[0].GetAttribute("role"));
        Assert.Equal("false", buttons[0].GetAttribute("aria-checked"));

        buttons[0].Click();
        buttons = cut.FindAll("button");

        Assert.Equal("one", requestedValue);
        Assert.Equal("true", buttons[0].GetAttribute("aria-checked"));
        Assert.Equal("on", buttons[0].GetAttribute("data-state"));

        buttons[0].Click();

        Assert.Equal(string.Empty, requestedValue);
    }

    [Fact]
    public void Multiple_group_accumulates_values_and_removes_them_independently()
    {
        IReadOnlyCollection<string>? requestedValues = null;

        var cut = Render(CreateMultipleGroup(EventCallback.Factory.Create<IReadOnlyCollection<string>>(this, values => requestedValues = values)));
        Assert.NotNull(cut.Find("[role='group']"));

        var buttons = cut.FindAll("button");

        buttons[0].Click();
        buttons[1].Click();

        Assert.Equal(["one", "two"], requestedValues);

        buttons = cut.FindAll("button");
        buttons[0].Click();

        Assert.Equal(["two"], requestedValues);
    }

    [Fact]
    public void Roving_focus_moves_current_tab_stop_with_arrow_keys()
    {
        var cut = Render(CreateSingleGroup());

        var buttons = cut.FindAll("button");
        Assert.Equal("0", buttons[0].GetAttribute("tabindex"));
        Assert.Equal("-1", buttons[1].GetAttribute("tabindex"));

        buttons[0].KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        buttons = cut.FindAll("button");

        Assert.Equal("-1", buttons[0].GetAttribute("tabindex"));
        Assert.Equal("0", buttons[1].GetAttribute("tabindex"));
    }

    [Fact]
    public void Direction_provider_flips_horizontal_navigation()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, nameof(BradixDirectionProvider.Dir), "rtl");
            builder.AddAttribute(2, nameof(BradixDirectionProvider.ChildContent), (RenderFragment) (contentBuilder =>
            {
                CreateSingleGroup()(contentBuilder);
            }));
            builder.CloseComponent();
        });

        var buttons = cut.FindAll("button");
        buttons[0].KeyDown(new KeyboardEventArgs { Key = "ArrowLeft" });
        buttons = cut.FindAll("button");

        Assert.Equal("-1", buttons[0].GetAttribute("tabindex"));
        Assert.Equal("0", buttons[1].GetAttribute("tabindex"));
    }

    private static RenderFragment CreateSingleGroup(EventCallback<string?> onValueChange = default)
    {
        return builder =>
        {
            builder.OpenComponent<BradixToggleGroup>(0);
            builder.AddAttribute(1, nameof(BradixToggleGroup.Type), (object) BradixSelectionMode.Single);
            builder.AddAttribute(2, nameof(BradixToggleGroup.Orientation), (object) BradixOrientation.Horizontal);

            if (onValueChange.HasDelegate)
                builder.AddAttribute(3, nameof(BradixToggleGroup.OnValueChange), onValueChange);

            builder.AddAttribute(4, nameof(BradixToggleGroup.ChildContent), (RenderFragment) (contentBuilder =>
            {
                RenderItem(contentBuilder, 0, "one");
                RenderItem(contentBuilder, 10, "two");
                RenderItem(contentBuilder, 20, "three");
            }));
            builder.CloseComponent();
        };
    }

    private static RenderFragment CreateMultipleGroup(EventCallback<IReadOnlyCollection<string>> onValuesChange = default)
    {
        return builder =>
        {
            builder.OpenComponent<BradixToggleGroup>(0);
            builder.AddAttribute(1, nameof(BradixToggleGroup.Type), (object) BradixSelectionMode.Multiple);

            if (onValuesChange.HasDelegate)
                builder.AddAttribute(2, nameof(BradixToggleGroup.OnValuesChange), onValuesChange);

            builder.AddAttribute(3, nameof(BradixToggleGroup.ChildContent), (RenderFragment) (contentBuilder =>
            {
                RenderItem(contentBuilder, 0, "one");
                RenderItem(contentBuilder, 10, "two");
                RenderItem(contentBuilder, 20, "three");
            }));
            builder.CloseComponent();
        };
    }

    private static void RenderItem(RenderTreeBuilder builder, int sequence, string value)
    {
        builder.OpenComponent<BradixToggleGroupItem>(sequence);
        builder.AddAttribute(sequence + 1, nameof(BradixToggleGroupItem.Value), value);
        builder.AddAttribute(sequence + 2, nameof(BradixToggleGroupItem.ChildContent), (RenderFragment) (contentBuilder =>
        {
            contentBuilder.AddContent(0, value);
        }));
        builder.CloseComponent();
    }
}
