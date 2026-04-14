using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixCorePrimitiveRenderTests : BunitContext
{
    public BradixCorePrimitiveRenderTests()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerLabelTextSelectionGuard", _ => true);
        module.SetupVoid("unregisterLabelTextSelectionGuard", _ => true);

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Label_renders_native_relationship_attributes()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixLabel>(0);
            builder.AddAttribute(1, "for", "control");
            builder.AddAttribute(2, nameof(BradixLabel.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.AddContent(0, "Label");
            }));
            builder.CloseComponent();
        });

        IElement label = cut.Find("label");
        await Assert.That(label.GetAttribute("for")).IsEqualTo("control");
        await Assert.That(label.TextContent).IsEqualTo("Label");
    }

    [Test]
    public async Task Label_forwards_js_mouse_down_to_consumer_callback()
    {
        MouseEventArgs? captured = null;
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
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

        await Assert.That(captured).IsNotNull();
        await Assert.That(captured!.Button).IsEqualTo(0);
        await Assert.That(captured.Detail).IsEqualTo(2);
        await Assert.That(captured.CtrlKey).IsTrue();
    }

    [Test]
    public async Task Separator_sets_semantic_and_decorative_roles_correctly()
    {
        IRenderedComponent<ContainerFragment> semantic = Render(builder =>
        {
            builder.OpenComponent<BradixSeparator>(0);
            builder.AddAttribute(1, nameof(BradixSeparator.Orientation), (object) BradixOrientation.Vertical);
            builder.CloseComponent();
        });

        IRenderedComponent<ContainerFragment> decorative = Render(builder =>
        {
            builder.OpenComponent<BradixSeparator>(0);
            builder.AddAttribute(1, nameof(BradixSeparator.Orientation), (object) BradixOrientation.Vertical);
            builder.AddAttribute(2, nameof(BradixSeparator.Decorative), true);
            builder.CloseComponent();
        });

        await Assert.That(semantic.Find("div").GetAttribute("role")).IsEqualTo("separator");
        await Assert.That(semantic.Find("div").GetAttribute("aria-orientation")).IsEqualTo("vertical");
        await Assert.That(decorative.Find("div").GetAttribute("role")).IsEqualTo("none");
        await Assert.That(decorative.Find("div").GetAttribute("aria-orientation")).IsNull();
    }

    [Test]
    public async Task Separator_as_child_renders_requested_native_element_and_preserves_semantics()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
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

        IElement separator = cut.Find("span[data-test-separator='true']");
        await Assert.That(separator.GetAttribute("role")).IsEqualTo("separator");
        await Assert.That(separator.GetAttribute("aria-orientation")).IsEqualTo("vertical");
        await Assert.That(separator.GetAttribute("data-orientation")).IsEqualTo("vertical");
    }

    [Test]
    public async Task Separator_as_child_requires_child_element_name()
    {
        await Assert.That(() =>
        {
            Render(builder =>
            {
                builder.OpenComponent<BradixSeparator>(0);
                builder.AddAttribute(1, nameof(BradixSeparator.AsChild), true);
                builder.CloseComponent();
            });
        }).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task Aspect_ratio_renders_wrapper_and_absolute_content_slot()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixAspectRatio>(0);
            builder.AddAttribute(1, nameof(BradixAspectRatio.Ratio), 16d / 9d);
            builder.AddAttribute(2, nameof(BradixAspectRatio.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.AddContent(0, "content");
            }));
            builder.CloseComponent();
        });

        IElement wrapper = cut.Find("[data-radix-aspect-ratio-wrapper]");
        IElement content = wrapper.Children[0];

        await Assert.That(wrapper.GetAttribute("style")).Contains("padding-bottom");
        await Assert.That(content.GetAttribute("style")).Contains("position: absolute");
        await Assert.That(content.TextContent).IsEqualTo("content");
    }

    [Test]
    public async Task Visually_hidden_applies_screen_reader_only_styles()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixVisuallyHidden>(0);
            builder.AddAttribute(1, nameof(BradixVisuallyHidden.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.AddContent(0, "hidden text");
            }));
            builder.CloseComponent();
        });

        IElement span = cut.Find("span");
        string style = span.GetAttribute("style") ?? string.Empty;

        await Assert.That(style).Contains("position: absolute");
        await Assert.That(style).Contains("clip: rect(0, 0, 0, 0)");
        await Assert.That(style).Contains("white-space: nowrap");
    }

    [Test]
    public async Task Visually_hidden_as_child_renders_requested_native_element_and_merges_attributes()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
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

        IElement label = cut.Find("label");
        string style = label.GetAttribute("style") ?? string.Empty;

        await Assert.That(label.GetAttribute("for")).IsEqualTo("search-box");
        await Assert.That(label.ClassName).Contains("slot-root");
        await Assert.That(label.ClassName).Contains("slot-child");
        await Assert.That(style).Contains("position: absolute");
        await Assert.That(style).Contains("color: rebeccapurple;");
        await Assert.That(label.TextContent).IsEqualTo("Search");
    }

    [Test]
    public async Task Visually_hidden_as_child_requires_child_element_name()
    {
        await Assert.That(() => Render(builder =>
        {
            builder.OpenComponent<BradixVisuallyHidden>(0);
            builder.AddAttribute(1, nameof(BradixVisuallyHidden.AsChild), true);
            builder.CloseComponent();
        })).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task Accessible_icon_hides_visual_glyph_and_renders_hidden_label()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
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

        IReadOnlyList<IElement> spans = cut.FindAll("span");
        await Assert.That(spans[0].GetAttribute("aria-hidden")).IsEqualTo("true");
        await Assert.That(spans[0].GetAttribute("focusable")).IsEqualTo("false");
        await Assert.That(cut.Markup).Contains("Close panel");
        await Assert.That(spans[1].GetAttribute("style")).Contains("position: absolute");
    }

    [Test]
    public async Task Accessible_icon_requires_non_empty_label()
    {
        await Assert.That(() => Render(builder =>
        {
            builder.OpenComponent<BradixAccessibleIcon>(0);
            builder.AddAttribute(1, nameof(BradixAccessibleIcon.Label), "");
            builder.AddAttribute(2, nameof(BradixAccessibleIcon.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenElement(0, "svg");
                contentBuilder.CloseElement();
            }));
            builder.CloseComponent();
        })).Throws<InvalidOperationException>();
    }
}