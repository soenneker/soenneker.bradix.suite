using System;
using System.Collections.Generic;
using Bunit;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit.Rendering;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixProgressRenderTests : BunitContext
{
    [Test]
    public async Task Progress_renders_loading_state_and_aria_values()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateProgress(30, 100));

        IElement root = cut.Find("[role='progressbar']");
        IElement indicator = cut.FindAll("div")[1];

        await Assert.That(root.GetAttribute("aria-valuenow")).IsEqualTo("30");
        await Assert.That(root.GetAttribute("aria-valuetext")).IsEqualTo("30%");
        await Assert.That(root.GetAttribute("data-state")).IsEqualTo("loading");
        await Assert.That(indicator.GetAttribute("data-state")).IsEqualTo("loading");
    }

    [Test]
    public async Task Progress_renders_indeterminate_when_value_is_invalid()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateProgress(150, 100));

        IElement root = cut.Find("[role='progressbar']");

        await Assert.That(root.GetAttribute("aria-valuenow")).IsNull();
        await Assert.That(root.GetAttribute("data-state")).IsEqualTo("indeterminate");
        await Assert.That(root.GetAttribute("data-value")).IsNull();
    }

    [Test]
    public async Task Progress_renders_complete_when_value_reaches_max()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateProgress(100, 100));

        IElement root = cut.Find("[role='progressbar']");
        IElement indicator = cut.FindAll("div")[1];

        await Assert.That(root.GetAttribute("data-state")).IsEqualTo("complete");
        await Assert.That(indicator.GetAttribute("data-state")).IsEqualTo("complete");
    }

    [Test]
    public async Task Progress_defaults_invalid_max_to_radix_default()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateProgress(30, 0));

        IElement root = cut.Find("[role='progressbar']");
        IElement indicator = cut.FindAll("div")[1];

        await Assert.That(root.GetAttribute("aria-valuemax")).IsEqualTo("100");
        await Assert.That(root.GetAttribute("data-max")).IsEqualTo("100");
        await Assert.That(indicator.GetAttribute("data-max")).IsEqualTo("100");
    }

    [Test]
    public async Task Progress_uses_custom_value_label_when_provided()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixProgress>(0);
            builder.AddAttribute(1, nameof(BradixProgress.Value), 3d);
            builder.AddAttribute(2, nameof(BradixProgress.Max), 5d);
            builder.AddAttribute(3, nameof(BradixProgress.GetValueLabel), (Func<double, double, string>)((value, max) => $"{value} of {max} tasks"));
            builder.AddAttribute(4, nameof(BradixProgress.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixProgressIndicator>(0);
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        IElement root = cut.Find("[role='progressbar']");

        await Assert.That(root.GetAttribute("aria-valuetext")).IsEqualTo("3 of 5 tasks");
    }

    [Test]
    public async Task Progress_allows_consumer_attributes_to_override_semantics_like_radix()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixProgress>(0);
            builder.AddAttribute(1, nameof(BradixProgress.Value), 30d);
            builder.AddAttribute(2, nameof(BradixProgress.AdditionalAttributes), new Dictionary<string, object>
            {
                ["aria-valuetext"] = "Custom label",
                ["data-state"] = "consumer-state"
            });
            builder.AddAttribute(3, nameof(BradixProgress.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixProgressIndicator>(0);
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        IElement root = cut.Find("div");
        await Assert.That(root.GetAttribute("aria-valuetext")).IsEqualTo("Custom label");
        await Assert.That(root.GetAttribute("data-state")).IsEqualTo("consumer-state");
    }

    private static RenderFragment CreateProgress(double? value, double max)
    {
        return builder =>
        {
            builder.OpenComponent<BradixProgress>(0);

            if (value.HasValue)
                builder.AddAttribute(1, nameof(BradixProgress.Value), value.Value);

            builder.AddAttribute(2, nameof(BradixProgress.Max), max);
            builder.AddAttribute(3, nameof(BradixProgress.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixProgressIndicator>(0);
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }
}
