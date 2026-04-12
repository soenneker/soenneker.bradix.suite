using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixCorePrimitiveRenderTests : BunitContext
{
    public BradixCorePrimitiveRenderTests()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerLabelTextSelectionGuard", _ => true);
        module.SetupVoid("unregisterLabelTextSelectionGuard", _ => true);

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
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
    public async Task Label_forwards_js_mouse_down_to_consumer_callback()
    {
        MouseEventArgs? captured = null;
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixLabel>(0);
            builder.AddAttribute(1, nameof(BradixLabel.OnMouseDown), EventCallback.Factory.Create<MouseEventArgs>(this, args => captured = args));
            builder.AddAttribute(2, nameof(BradixLabel.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.AddContent(0, "Label");
            }));
            builder.CloseComponent();
        });

        await cut.FindComponent<BradixLabel>().Instance.HandleMouseDownFromJs(new BradixDelegatedMouseEvent
        {
            Button = 0,
            Detail = 2,
            CtrlKey = true
        });

        Assert.NotNull(captured);
        Assert.Equal(0, captured!.Button);
        Assert.Equal(2, captured.Detail);
        Assert.True(captured.CtrlKey);
    }

    [Fact]
    public void Separator_sets_semantic_and_decorative_roles_correctly()
    {
        var semantic = Render(builder =>
        {
            builder.OpenComponent<BradixSeparator>(0);
            builder.AddAttribute(1, nameof(BradixSeparator.Orientation), (object) BradixOrientation.Vertical);
            builder.CloseComponent();
        });

        var decorative = Render(builder =>
        {
            builder.OpenComponent<BradixSeparator>(0);
            builder.AddAttribute(1, nameof(BradixSeparator.Orientation), (object) BradixOrientation.Vertical);
            builder.AddAttribute(2, nameof(BradixSeparator.Decorative), true);
            builder.CloseComponent();
        });

        Assert.Equal("separator", semantic.Find("div").GetAttribute("role"));
        Assert.Equal("vertical", semantic.Find("div").GetAttribute("aria-orientation"));
        Assert.Equal("none", decorative.Find("div").GetAttribute("role"));
        Assert.Null(decorative.Find("div").GetAttribute("aria-orientation"));
    }

    [Fact]
    public void Separator_as_child_renders_requested_native_element_and_preserves_semantics()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixSeparator>(0);
            builder.AddAttribute(1, nameof(BradixSeparator.AsChild), true);
            builder.AddAttribute(2, nameof(BradixSeparator.ChildElementName), "span");
            builder.AddAttribute(3, nameof(BradixSeparator.Orientation), (object) BradixOrientation.Vertical);
            builder.AddAttribute(4, nameof(BradixSeparator.ChildAttributes), new Dictionary<string, object>
            {
                ["data-test-separator"] = "true"
            });
            builder.CloseComponent();
        });

        var separator = cut.Find("span[data-test-separator='true']");
        Assert.Equal("separator", separator.GetAttribute("role"));
        Assert.Equal("vertical", separator.GetAttribute("aria-orientation"));
        Assert.Equal("vertical", separator.GetAttribute("data-orientation"));
    }

    [Fact]
    public void Separator_as_child_requires_child_element_name()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Render(builder =>
            {
                builder.OpenComponent<BradixSeparator>(0);
                builder.AddAttribute(1, nameof(BradixSeparator.AsChild), true);
                builder.CloseComponent();
            });
        });
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

    [Fact]
    public void Visually_hidden_as_child_renders_requested_native_element_and_merges_attributes()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixVisuallyHidden>(0);
            builder.AddAttribute(1, nameof(BradixVisuallyHidden.AsChild), true);
            builder.AddAttribute(2, nameof(BradixVisuallyHidden.ChildElementName), "label");
            builder.AddAttribute(3, nameof(BradixVisuallyHidden.Class), "slot-root");
            builder.AddAttribute(4, nameof(BradixVisuallyHidden.ChildAttributes), new Dictionary<string, object>
            {
                ["for"] = "search-box",
                ["class"] = "slot-child",
                ["style"] = "color: rebeccapurple;"
            });
            builder.AddAttribute(5, nameof(BradixVisuallyHidden.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.AddContent(0, "Search");
            }));
            builder.CloseComponent();
        });

        var label = cut.Find("label");
        string style = label.GetAttribute("style") ?? string.Empty;

        Assert.Equal("search-box", label.GetAttribute("for"));
        Assert.Contains("slot-root", label.ClassName);
        Assert.Contains("slot-child", label.ClassName);
        Assert.Contains("position: absolute", style);
        Assert.Contains("color: rebeccapurple;", style);
        Assert.Equal("Search", label.TextContent);
    }

    [Fact]
    public void Visually_hidden_as_child_requires_child_element_name()
    {
        Assert.Throws<InvalidOperationException>(() => Render(builder =>
        {
            builder.OpenComponent<BradixVisuallyHidden>(0);
            builder.AddAttribute(1, nameof(BradixVisuallyHidden.AsChild), true);
            builder.CloseComponent();
        }));
    }

    [Fact]
    public void Accessible_icon_hides_visual_glyph_and_renders_hidden_label()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixAccessibleIcon>(0);
            builder.AddAttribute(1, nameof(BradixAccessibleIcon.Label), "Close panel");
            builder.AddAttribute(2, nameof(BradixAccessibleIcon.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenElement(0, "svg");
                contentBuilder.AddAttribute(1, "viewBox", "0 0 16 16");
                contentBuilder.OpenElement(2, "path");
                contentBuilder.AddAttribute(3, "d", "M3 3l10 10");
                contentBuilder.CloseElement();
                contentBuilder.CloseElement();
            }));
            builder.CloseComponent();
        });

        var spans = cut.FindAll("span");
        Assert.Equal("true", spans[0].GetAttribute("aria-hidden"));
        Assert.Equal("false", spans[0].GetAttribute("focusable"));
        Assert.Contains("Close panel", cut.Markup);
        Assert.Contains("position: absolute", spans[1].GetAttribute("style"));
    }

    [Fact]
    public void Accessible_icon_requires_non_empty_label()
    {
        Assert.Throws<InvalidOperationException>(() => Render(builder =>
        {
            builder.OpenComponent<BradixAccessibleIcon>(0);
            builder.AddAttribute(1, nameof(BradixAccessibleIcon.Label), "");
            builder.AddAttribute(2, nameof(BradixAccessibleIcon.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenElement(0, "svg");
                contentBuilder.CloseElement();
            }));
            builder.CloseComponent();
        }));
    }
}
