using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixVisuallyHiddenRenderTests : BunitContext
{
    [Test]
    public async Task Visually_hidden_renders_radix_hidden_styles_on_span()
    {
        IRenderedComponent<BradixVisuallyHidden> cut = Render<BradixVisuallyHidden>(parameters => parameters
            .Add(hidden => hidden.ChildContent, (RenderFragment)(builder =>
            {
                builder.AddContent(0, "Screen reader text");
            })));

        IElement span = cut.Find("span");
        string style = span.GetAttribute("style") ?? string.Empty;

        await Assert.That(style).Contains("position: absolute");
        await Assert.That(style).Contains("width: 1px");
        await Assert.That(style).Contains("clip: rect(0, 0, 0, 0)");
        await Assert.That(span.TextContent).IsEqualTo("Screen reader text");
    }

    [Test]
    public async Task Visually_hidden_appends_consumer_style_after_hidden_styles()
    {
        IRenderedComponent<BradixVisuallyHidden> cut = Render<BradixVisuallyHidden>(parameters => parameters
            .Add(hidden => hidden.Style, "width: 2px; color: red"));

        string style = cut.Find("span").GetAttribute("style") ?? string.Empty;

        await Assert.That(style).Contains("width: 1px");
        await Assert.That(style).EndsWith("width: 2px; color: red");
    }

    [Test]
    public async Task Visually_hidden_as_child_applies_hidden_styles_to_child_element()
    {
        IRenderedComponent<BradixVisuallyHidden> cut = Render<BradixVisuallyHidden>(parameters => parameters
            .Add(hidden => hidden.AsChild, true)
            .Add(hidden => hidden.ChildElementName, "strong")
            .Add(hidden => hidden.ChildAttributes, new Dictionary<string, object>
            {
                ["style"] = "color: red",
                ["class"] = "child-label"
            })
            .Add(hidden => hidden.ChildContent, (RenderFragment)(builder =>
            {
                builder.AddContent(0, "Hidden label");
            })));

        IElement element = cut.Find("strong");
        string style = element.GetAttribute("style") ?? string.Empty;

        await Assert.That(style).Contains("position: absolute");
        await Assert.That(style).EndsWith("color: red;");
        await Assert.That(element.GetAttribute("class")).Contains("child-label");
        await Assert.That(element.TextContent).IsEqualTo("Hidden label");
    }
}
