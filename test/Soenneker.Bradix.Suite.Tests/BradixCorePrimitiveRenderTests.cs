using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Bradix.Suite.AspectRatio;
using Soenneker.Bradix.Suite.Interop;
using Soenneker.Bradix.Suite.Label;
using Soenneker.Bradix.Suite.Separator;
using Soenneker.Bradix.Suite.VisuallyHidden;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixCorePrimitiveRenderTests : Bunit.BunitContext
{
    public BradixCorePrimitiveRenderTests()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerLabelTextSelectionGuard", _ => true);
        module.SetupVoid("unregisterLabelTextSelectionGuard", _ => true);

        Services.AddScoped<BradixSuiteInterop>();
    }

    [Fact]
    public void Label_renders_native_relationship_attributes()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixLabel>(0);
            builder.AddAttribute(1, "for", "control");
            builder.AddAttribute(2, nameof(BradixLabel.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.AddContent(0, "Label");
            }));
            builder.CloseComponent();
        });

        var label = cut.Find("label");
        Assert.Equal("control", label.GetAttribute("for"));
        Assert.Equal("Label", label.TextContent);
    }

    [Fact]
    public void Separator_sets_semantic_and_decorative_roles_correctly()
    {
        var semantic = Render(builder =>
        {
            builder.OpenComponent<BradixSeparator>(0);
            builder.AddAttribute(1, nameof(BradixSeparator.Orientation), "vertical");
            builder.CloseComponent();
        });

        var decorative = Render(builder =>
        {
            builder.OpenComponent<BradixSeparator>(0);
            builder.AddAttribute(1, nameof(BradixSeparator.Orientation), "vertical");
            builder.AddAttribute(2, nameof(BradixSeparator.Decorative), true);
            builder.CloseComponent();
        });

        Assert.Equal("separator", semantic.Find("div").GetAttribute("role"));
        Assert.Equal("vertical", semantic.Find("div").GetAttribute("aria-orientation"));
        Assert.Equal("none", decorative.Find("div").GetAttribute("role"));
        Assert.Null(decorative.Find("div").GetAttribute("aria-orientation"));
    }

    [Fact]
    public void Aspect_ratio_renders_wrapper_and_absolute_content_slot()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixAspectRatio>(0);
            builder.AddAttribute(1, nameof(BradixAspectRatio.Ratio), 16d / 9d);
            builder.AddAttribute(2, nameof(BradixAspectRatio.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.AddContent(0, "content");
            }));
            builder.CloseComponent();
        });

        var wrapper = cut.Find("[data-radix-aspect-ratio-wrapper]");
        var content = wrapper.Children[0];

        Assert.Contains("padding-bottom", wrapper.GetAttribute("style"));
        Assert.Contains("position: absolute", content.GetAttribute("style"));
        Assert.Equal("content", content.TextContent);
    }

    [Fact]
    public void Visually_hidden_applies_screen_reader_only_styles()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixVisuallyHidden>(0);
            builder.AddAttribute(1, nameof(BradixVisuallyHidden.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.AddContent(0, "hidden text");
            }));
            builder.CloseComponent();
        });

        var span = cut.Find("span");
        string style = span.GetAttribute("style") ?? string.Empty;

        Assert.Contains("position: absolute", style);
        Assert.Contains("clip: rect(0, 0, 0, 0)", style);
        Assert.Contains("white-space: nowrap", style);
    }
}
