using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixSeparatorRenderTests : BunitContext
{
    [Test]
    public async Task Separator_defaults_to_semantic_horizontal_without_aria_orientation()
    {
        IRenderedComponent<BradixSeparator> cut = Render<BradixSeparator>();

        IElement separator = cut.Find("div");

        await Assert.That(separator.GetAttribute("role")).IsEqualTo("separator");
        await Assert.That(separator.GetAttribute("data-orientation")).IsEqualTo("horizontal");
        await Assert.That(separator.GetAttribute("aria-orientation")).IsNull();
    }

    [Test]
    public async Task Separator_vertical_sets_aria_orientation()
    {
        IRenderedComponent<BradixSeparator> cut = Render<BradixSeparator>(parameters => parameters
            .Add(separator => separator.Orientation, BradixOrientation.Vertical));

        IElement separator = cut.Find("div");

        await Assert.That(separator.GetAttribute("role")).IsEqualTo("separator");
        await Assert.That(separator.GetAttribute("data-orientation")).IsEqualTo("vertical");
        await Assert.That(separator.GetAttribute("aria-orientation")).IsEqualTo("vertical");
    }

    [Test]
    public async Task Separator_decorative_is_removed_from_separator_semantics()
    {
        IRenderedComponent<BradixSeparator> cut = Render<BradixSeparator>(parameters => parameters
            .Add(separator => separator.Decorative, true)
            .Add(separator => separator.Orientation, BradixOrientation.Vertical));

        IElement separator = cut.Find("div");

        await Assert.That(separator.GetAttribute("role")).IsEqualTo("none");
        await Assert.That(separator.GetAttribute("data-orientation")).IsEqualTo("vertical");
        await Assert.That(separator.GetAttribute("aria-orientation")).IsNull();
    }

    [Test]
    public async Task Separator_as_child_applies_semantics_to_child_element()
    {
        IRenderedComponent<BradixSeparator> cut = Render<BradixSeparator>(parameters => parameters
            .Add(separator => separator.AsChild, true)
            .Add(separator => separator.ChildElementName, "hr")
            .Add(separator => separator.ChildContent, (RenderFragment)(builder =>
            {
                builder.AddContent(0, "ignored");
            })));

        IElement separator = cut.Find("hr");

        await Assert.That(separator.GetAttribute("role")).IsEqualTo("separator");
        await Assert.That(separator.GetAttribute("data-orientation")).IsEqualTo("horizontal");
    }

    [Test]
    public async Task Separator_allows_consumer_attributes_to_override_default_semantics()
    {
        IRenderedComponent<BradixSeparator> cut = Render<BradixSeparator>(parameters => parameters
            .AddUnmatched("role", "presentation")
            .AddUnmatched("aria-orientation", "vertical")
            .AddUnmatched("data-orientation", "vertical"));

        IElement separator = cut.Find("div");

        await Assert.That(separator.GetAttribute("role")).IsEqualTo("presentation");
        await Assert.That(separator.GetAttribute("aria-orientation")).IsEqualTo("vertical");
        await Assert.That(separator.GetAttribute("data-orientation")).IsEqualTo("vertical");
    }
}
