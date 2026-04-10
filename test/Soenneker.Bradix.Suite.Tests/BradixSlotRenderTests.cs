using Bunit;
using Microsoft.AspNetCore.Components;
using Soenneker.Bradix.Suite.Slot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixSlotRenderTests : BunitContext
{
    [Fact]
    public void Slot_merges_class_style_and_child_attribute_precedence()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixSlot>(0);
            builder.AddAttribute(1, nameof(BradixSlot.ElementName), "button");
            builder.AddAttribute(2, nameof(BradixSlot.Id), "slot-id");
            builder.AddAttribute(3, nameof(BradixSlot.Class), "slot-root");
            builder.AddAttribute(4, nameof(BradixSlot.Style), "color: steelblue; border: 1px solid currentColor;");
            builder.AddAttribute(5, nameof(BradixSlot.ChildAttributes), new Dictionary<string, object>
            {
                ["class"] = "child-root",
                ["style"] = "color: rebeccapurple; background: transparent;",
                ["type"] = "button",
                ["title"] = "child title"
            });
            builder.AddAttribute(6, nameof(BradixSlot.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.AddContent(0, "Hello");
            }));
            builder.CloseComponent();
        });

        var button = cut.Find("button");
        string style = button.GetAttribute("style") ?? string.Empty;

        Assert.Equal("slot-id", button.Id);
        Assert.Contains("slot-root", button.ClassName);
        Assert.Contains("child-root", button.ClassName);
        Assert.Contains("color: steelblue;", style);
        Assert.Contains("color: rebeccapurple;", style);
        Assert.Equal("button", button.GetAttribute("type"));
        Assert.Equal("child title", button.GetAttribute("title"));
        Assert.Equal("Hello", button.TextContent);
    }

    [Fact]
    public void Slot_composes_child_and_slot_click_handlers_in_child_first_order()
    {
        List<string> calls = [];

        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixSlot>(0);
            builder.AddAttribute(1, nameof(BradixSlot.ElementName), "button");
            builder.AddAttribute(2, nameof(BradixSlot.AdditionalAttributes), new Dictionary<string, object>
            {
                ["onclick"] = (Action)(() =>
                {
                    calls.Add("slot");
                })
            });
            builder.AddAttribute(3, nameof(BradixSlot.ChildAttributes), new Dictionary<string, object>
            {
                ["onclick"] = (Action)(() =>
                {
                    calls.Add("child");
                }),
                ["type"] = "button"
            });
            builder.AddAttribute(4, nameof(BradixSlot.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.AddContent(0, "Click");
            }));
            builder.CloseComponent();
        });

        cut.Find("button").Click();

        Assert.Equal(["child", "slot"], calls);
    }

    [Fact]
    public void Slot_requires_non_empty_element_name()
    {
        Assert.Throws<InvalidOperationException>(() => Render(builder =>
        {
            builder.OpenComponent<BradixSlot>(0);
            builder.AddAttribute(1, nameof(BradixSlot.ElementName), "");
            builder.CloseComponent();
        }));
    }
}
