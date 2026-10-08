using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixSliderRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixSliderRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.Setup<bool>("isFormControl", invocation =>
                invocation.Arguments.Count > 1 && invocation.Arguments[1] is string formId && !string.IsNullOrWhiteSpace(formId))
            .SetResult(true);
        _module.Setup<bool>("isFormControl", _ => true).SetResult(false);
        _module.SetupVoid("registerSliderPointerBridge", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterSliderPointerBridge", _ => true).SetVoidResult();
        _module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("syncSliderBubbleInputValue", _ => true).SetVoidResult();

        Services.AddBradixTestInterops();
    }

    [Test]
    public async ValueTask Slider_renders_track_range_and_thumb_semantics(CancellationToken cancellationToken)
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSlider(defaultValues: [20]));

        IElement root = cut.Find("[data-orientation='horizontal']");
        IElement thumb = cut.Find("[role='slider']");
        IElement range = cut.Find(".range");

        await Assert.That(root.GetAttribute("aria-disabled")).IsEqualTo("false");
        await Assert.That(thumb.GetAttribute("aria-valuenow")).IsEqualTo("20");
        await Assert.That(thumb.GetAttribute("aria-orientation")).IsEqualTo("horizontal");
        await Assert.That(range.GetAttribute("style")).Contains("left:");
        await Assert.That(cut.Markup).DoesNotContain("data-bradix-slider-thumb-index");
    }

    [Test]
    public async ValueTask Thumb_attributes_do_not_create_duplicate_ids_or_wrapper_tab_stops(CancellationToken cancellationToken)
    {
        var cut = Render<BradixSlider>(p => p.Add(c => c.DefaultValues, new double[] { 20 }).AddChildContent(builder =>
        {
            builder.OpenComponent<BradixSliderThumb>(0);
            builder.AddAttribute(1, "id", "volume");
            builder.AddAttribute(2, "aria-label", "Volume");
            builder.AddAttribute(3, "tabindex", 0);
            builder.CloseComponent();
        }));

        await Assert.That(cut.FindAll("#volume").Count).IsEqualTo(1);
        await Assert.That(cut.FindAll("[tabindex='0']").Count).IsEqualTo(1);
        await Assert.That(cut.Find("#volume").GetAttribute("role")).IsEqualTo("slider");
        cut.Render(p => p.Add(c => c.Disabled, true));
        await Assert.That(cut.Find("[role='slider']").GetAttribute("aria-disabled")).IsEqualTo("true");
        await Assert.That(cut.Find("[role='slider']").GetAttribute("tabindex")).IsEqualTo("-1");
    }

    [Test]
    public async ValueTask Slider_arrow_key_updates_value(CancellationToken cancellationToken)
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSlider(defaultValues: [20]));

        IElement thumb = cut.Find("[role='slider']");
        await thumb.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });
        thumb = cut.Find("[role='slider']");

        await Assert.That(thumb.GetAttribute("aria-valuenow")).IsEqualTo("21");
    }

    [Test]
    public async ValueTask Slider_scientific_notation_step_preserves_decimal_precision(CancellationToken cancellationToken)
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSlider(defaultValues: [0], step: 1e-7));

        IElement thumb = cut.Find("[role='slider']");
        await thumb.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });
        thumb = cut.Find("[role='slider']");

        await Assert.That(thumb.GetAttribute("aria-valuenow")).IsEqualTo("1E-07");
    }

    [Test]
    public async ValueTask Slider_subnormal_scientific_notation_step_remains_finite(CancellationToken cancellationToken)
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSlider(defaultValues: [0], step: 1e-309));

        IElement thumb = cut.Find("[role='slider']");
        await thumb.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });
        thumb = cut.Find("[role='slider']");

        await Assert.That(thumb.GetAttribute("aria-valuenow")).IsEqualTo("1E-309");
    }

    [Test]
    public async ValueTask Slider_ordinary_decimal_step_preserves_decimal_precision(CancellationToken cancellationToken)
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSlider(defaultValues: [0.25], step: 0.125));

        IElement thumb = cut.Find("[role='slider']");
        await thumb.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });
        thumb = cut.Find("[role='slider']");

        await Assert.That(thumb.GetAttribute("aria-valuenow")).IsEqualTo("0.375");
    }

    [Test]
    public async ValueTask Home_and_end_update_first_and_last_thumb_in_multi_thumb_slider(CancellationToken cancellationToken)
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSlider(defaultValues: [20, 80]));
        IReadOnlyList<IElement> thumbs = cut.FindAll("[role='slider']");

        await thumbs[1].KeyDownAsync(new KeyboardEventArgs { Key = "Home" });
        thumbs = cut.FindAll("[role='slider']");
        await Assert.That(thumbs[0].GetAttribute("aria-valuenow")).IsEqualTo("0");
        await Assert.That(thumbs[1].GetAttribute("aria-valuenow")).IsEqualTo("80");

        await thumbs[0].KeyDownAsync(new KeyboardEventArgs { Key = "End" });
        thumbs = cut.FindAll("[role='slider']");
        await Assert.That(thumbs[0].GetAttribute("aria-valuenow")).IsEqualTo("0");
        await Assert.That(thumbs[1].GetAttribute("aria-valuenow")).IsEqualTo("100");
    }

    [Test]
    public async ValueTask Slider_respects_min_steps_between_thumbs(CancellationToken cancellationToken)
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSlider(defaultValues: [20, 30], minStepsBetweenThumbs: 5));

        IReadOnlyList<IElement> thumbs = cut.FindAll("[role='slider']");
        await thumbs[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight", ShiftKey = true });
        thumbs = cut.FindAll("[role='slider']");

        await Assert.That(thumbs[0].GetAttribute("aria-valuenow")).IsEqualTo("20");
        await Assert.That(thumbs[1].GetAttribute("aria-valuenow")).IsEqualTo("30");
    }

    [Test]
    public async ValueTask Slider_pointer_bridge_updates_closest_thumb(CancellationToken cancellationToken)
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSlider(defaultValues: [20, 80]));
        IRenderedComponent<BradixSlider> slider = cut.FindComponent<BradixSlider>();

        await slider.Instance.HandlePointerStart(0.75, 0.5, -1);

        IReadOnlyList<IElement> thumbs = cut.FindAll("[role='slider']");
        await Assert.That(thumbs[1].GetAttribute("aria-valuenow")).IsEqualTo("75");
    }

    [Test]
    public async ValueTask Slider_pointer_cancel_does_not_commit_value(CancellationToken cancellationToken)
    {
        var commitCount = 0;
        IRenderedComponent<ContainerFragment> cut = Render(CreateSlider(defaultValues: [20], onValueCommit: () => commitCount++));
        IRenderedComponent<BradixSlider> slider = cut.FindComponent<BradixSlider>();

        await slider.Instance.HandlePointerStart(0.75, 0.5, -1);
        await slider.Instance.HandlePointerMove(0.8, 0.5);
        await slider.Instance.HandlePointerCancel();

        await Assert.That(commitCount).IsEqualTo(0);
        await Assert.That(cut.Find("[role='slider']").GetAttribute("aria-valuenow")).IsEqualTo("20");
    }

    [Test]
    public async ValueTask Slider_with_name_outside_form_does_not_render_bubble_inputs(CancellationToken cancellationToken)
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSlider(defaultValues: [10, 30], name: "price"));
        await Assert.That(cut.FindAll("input")).IsEmpty();
    }

    [Test]
    public async ValueTask Slider_with_explicit_form_renders_bubble_inputs_outside_form(CancellationToken cancellationToken)
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSlider(defaultValues: [10, 30], name: "price", form: "settings-form"));

        IReadOnlyList<IElement> inputs = cut.FindAll("input");
        await Assert.That(inputs.Count).IsEqualTo(2);
        await Assert.That(inputs[0].GetAttribute("form")).IsEqualTo("settings-form");
        await Assert.That(_module.Invocations.Any(invocation =>
            invocation.Identifier == "isFormControl" &&
            invocation.Arguments.Count > 1 &&
            Equals(invocation.Arguments[1], "settings-form"))).IsTrue();
    }

    [Test]
    public async ValueTask Inherited_direction_flips_horizontal_back_key(CancellationToken cancellationToken)
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, nameof(BradixDirectionProvider.Dir), "rtl");
            builder.AddAttribute(2, nameof(BradixDirectionProvider.ChildContent), (RenderFragment) (contentBuilder =>
            {
                CreateSlider(defaultValues: [20])(contentBuilder);
            }));
            builder.CloseComponent();
        });

        IElement thumb = cut.Find("[role='slider']");
        await thumb.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });
        thumb = cut.Find("[role='slider']");

        await Assert.That(thumb.GetAttribute("aria-valuenow")).IsEqualTo("21");
    }

    [Test]
    public async ValueTask Slider_thumb_registers_keyboard_default_prevention(CancellationToken cancellationToken)
    {
        _ = Render(CreateSlider(defaultValues: [20]));

        object? options = _module.Invocations.First(invocation => invocation.Identifier == "registerDelegatedInteraction").Arguments[2];
        JsonElement keydown = ((JsonElement)options!).GetProperty("keydown");
        object? preventDefaultKeys = keydown.GetProperty("preventDefaultKeys").EnumerateArray().Select(item => item.GetString()!).ToArray();

        await Assert.That(preventDefaultKeys).IsAssignableTo<string[]>();
        var keys = (string[])preventDefaultKeys!;
        await Assert.That(keys.Contains("Home")).IsTrue();
        await Assert.That(keys.Contains("End")).IsTrue();
        await Assert.That(keys.Contains("ArrowRight")).IsTrue();
        await Assert.That(keys.Contains("PageDown")).IsTrue();
    }

    private static RenderFragment CreateSlider(IReadOnlyList<double> defaultValues, double minStepsBetweenThumbs = 0, string? name = null, Action? onValueCommit = null,
        string? form = null, double step = 1)
    {
        return builder =>
        {
            builder.OpenComponent<BradixSlider>(0);
            builder.AddAttribute(1, nameof(BradixSlider.DefaultValues), defaultValues);
            builder.AddAttribute(2, nameof(BradixSlider.MinStepsBetweenThumbs), minStepsBetweenThumbs);
            builder.AddAttribute(3, nameof(BradixSlider.Step), step);

            if (name is not null)
                builder.AddAttribute(4, nameof(BradixSlider.Name), name);

            if (onValueCommit is not null)
                builder.AddAttribute(5, nameof(BradixSlider.OnValueCommit), EventCallback.Factory.Create<IReadOnlyList<double>>(new object(), _ => onValueCommit()));

            if (form is not null)
                builder.AddAttribute(6, nameof(BradixSlider.Form), form);

            builder.AddAttribute(7, nameof(BradixSlider.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixSliderTrack>(0);
                contentBuilder.AddAttribute(1, nameof(BradixSliderTrack.Class), "track");
                contentBuilder.AddAttribute(2, nameof(BradixSliderTrack.ChildContent), (RenderFragment) (trackBuilder =>
                {
                    trackBuilder.OpenComponent<BradixSliderRange>(0);
                    trackBuilder.AddAttribute(1, nameof(BradixSliderRange.Class), "range");
                    trackBuilder.CloseComponent();
                }));
                contentBuilder.CloseComponent();

                for (var i = 0; i < defaultValues.Count; i++)
                {
                    contentBuilder.OpenComponent<BradixSliderThumb>(10 + i);
                    contentBuilder.CloseComponent();
                }
            }));
            builder.CloseComponent();
        };
    }
}
