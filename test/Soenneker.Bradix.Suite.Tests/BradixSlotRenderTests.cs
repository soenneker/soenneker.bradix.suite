using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixSlotRenderTests : BunitContext
{
    [Test]
    public async Task Slot_merges_class_style_and_child_attribute_precedence()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
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

        IElement button = cut.Find("button");
        string style = button.GetAttribute("style") ?? string.Empty;

        await Assert.That(button.Id).IsEqualTo("slot-id");
        await Assert.That(button.ClassName).Contains("slot-root");
        await Assert.That(button.ClassName).Contains("child-root");
        await Assert.That(style).Contains("color: steelblue;");
        await Assert.That(style).Contains("color: rebeccapurple;");
        await Assert.That(button.GetAttribute("type")).IsEqualTo("button");
        await Assert.That(button.GetAttribute("title")).IsEqualTo("child title");
        await Assert.That(button.TextContent).IsEqualTo("Hello");
    }

    [Test]
    public async Task Slot_composes_child_and_slot_click_handlers_in_child_first_order()
    {
        List<string> calls = [];

        IRenderedComponent<ContainerFragment> cut = Render(builder =>
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

        await cut.Find("button").ClickAsync();

        await Assert.That(string.Join(",", calls)).IsEqualTo("child,slot");
    }

    [Test]
    public async Task Slot_requires_non_empty_element_name()
    {
        await Assert.That(() => Render(builder =>
        {
            builder.OpenComponent<BradixSlot>(0);
            builder.AddAttribute(1, nameof(BradixSlot.ElementName), "");
            builder.CloseComponent();
        })).Throws<InvalidOperationException>();
    }
}