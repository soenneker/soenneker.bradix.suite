using System.Collections.Generic;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixProgressRenderTests : BunitContext
{
    [Fact]
    public void Progress_renders_loading_state_and_aria_values()
    {
        var cut = Render(CreateProgress(30, 100));

        var root = cut.Find("[role='progressbar']");
        var indicator = cut.FindAll("div")[1];

        Assert.Equal("30", root.GetAttribute("aria-valuenow"));
        Assert.Equal("30%", root.GetAttribute("aria-valuetext"));
        Assert.Equal("loading", root.GetAttribute("data-state"));
        Assert.Equal("loading", indicator.GetAttribute("data-state"));
    }

    [Fact]
    public void Progress_renders_indeterminate_when_value_is_invalid()
    {
        var cut = Render(CreateProgress(150, 100));

        var root = cut.Find("[role='progressbar']");

        Assert.Null(root.GetAttribute("aria-valuenow"));
        Assert.Equal("indeterminate", root.GetAttribute("data-state"));
        Assert.Null(root.GetAttribute("data-value"));
    }

    [Fact]
    public void Progress_renders_complete_when_value_reaches_max()
    {
        var cut = Render(CreateProgress(100, 100));

        var root = cut.Find("[role='progressbar']");
        var indicator = cut.FindAll("div")[1];

        Assert.Equal("complete", root.GetAttribute("data-state"));
        Assert.Equal("complete", indicator.GetAttribute("data-state"));
    }

    [Fact]
    public void Progress_allows_consumer_attributes_to_override_semantics_like_radix()
    {
        var cut = Render(builder =>
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

        var root = cut.Find("div");
        Assert.Equal("Custom label", root.GetAttribute("aria-valuetext"));
        Assert.Equal("consumer-state", root.GetAttribute("data-state"));
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
