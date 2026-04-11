using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

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
        _module.SetupVoid("syncSliderBubbleInputValue", _ => true).SetVoidResult();

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Fact]
    public void Slider_renders_track_range_and_thumb_semantics()
    {
        var cut = Render(CreateSlider(defaultValues: [20]));

        var root = cut.Find("[role='group']");
        var thumb = cut.Find("[role='slider']");
        var range = cut.Find(".range");

        Assert.Equal("horizontal", root.GetAttribute("aria-orientation"));
        Assert.Equal("20", thumb.GetAttribute("aria-valuenow"));
        Assert.Equal("horizontal", thumb.GetAttribute("aria-orientation"));
        Assert.Contains("left:", range.GetAttribute("style"));
        Assert.DoesNotContain("data-bradix-slider-thumb-index", cut.Markup);
    }

    [Fact]
    public void Slider_arrow_key_updates_value()
    {
        var cut = Render(CreateSlider(defaultValues: [20]));

        var thumb = cut.Find("[role='slider']");
        thumb.KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        thumb = cut.Find("[role='slider']");

        Assert.Equal("21", thumb.GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void Home_and_end_update_first_and_last_thumb_in_multi_thumb_slider()
    {
        var cut = Render(CreateSlider(defaultValues: [20, 80]));
        var thumbs = cut.FindAll("[role='slider']");

        thumbs[1].KeyDown(new KeyboardEventArgs { Key = "Home" });
        thumbs = cut.FindAll("[role='slider']");
        Assert.Equal("0", thumbs[0].GetAttribute("aria-valuenow"));
        Assert.Equal("80", thumbs[1].GetAttribute("aria-valuenow"));

        thumbs[0].KeyDown(new KeyboardEventArgs { Key = "End" });
        thumbs = cut.FindAll("[role='slider']");
        Assert.Equal("0", thumbs[0].GetAttribute("aria-valuenow"));
        Assert.Equal("100", thumbs[1].GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void Slider_respects_min_steps_between_thumbs()
    {
        var cut = Render(CreateSlider(defaultValues: [20, 30], minStepsBetweenThumbs: 5));

        var thumbs = cut.FindAll("[role='slider']");
        thumbs[0].KeyDown(new KeyboardEventArgs { Key = "ArrowRight", ShiftKey = true });
        thumbs = cut.FindAll("[role='slider']");

        Assert.Equal("20", thumbs[0].GetAttribute("aria-valuenow"));
        Assert.Equal("30", thumbs[1].GetAttribute("aria-valuenow"));
    }

    [Fact]
    public async Task Slider_pointer_bridge_updates_closest_thumb()
    {
        var cut = Render(CreateSlider(defaultValues: [20, 80]));
        var slider = cut.FindComponent<BradixSlider>();

        await slider.Instance.HandlePointerStartAsync(0.75, 0.5, -1);

        var thumbs = cut.FindAll("[role='slider']");
        Assert.Equal("75", thumbs[1].GetAttribute("aria-valuenow"));
    }

    [Fact]
    public async Task Slider_pointer_cancel_does_not_commit_value()
    {
        int commitCount = 0;
        var cut = Render(CreateSlider(defaultValues: [20], onValueCommit: () => commitCount++));
        var slider = cut.FindComponent<BradixSlider>();

        await slider.Instance.HandlePointerStartAsync(0.75, 0.5, -1);
        await slider.Instance.HandlePointerMoveAsync(0.8, 0.5);
        await slider.Instance.HandlePointerCancelAsync();

        Assert.Equal(0, commitCount);
        Assert.Equal("80", cut.Find("[role='slider']").GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void Slider_with_name_outside_form_does_not_render_bubble_inputs()
    {
        var cut = Render(CreateSlider(defaultValues: [10, 30], name: "price"));
        Assert.Empty(cut.FindAll("input"));
    }

    [Fact]
    public void Slider_with_explicit_form_renders_bubble_inputs_outside_form()
    {
        var cut = Render(CreateSlider(defaultValues: [10, 30], name: "price", form: "settings-form"));

        var inputs = cut.FindAll("input");
        Assert.Equal(2, inputs.Count);
        Assert.All(inputs, input => Assert.Equal("settings-form", input.GetAttribute("form")));
        Assert.Contains(_module.Invocations, invocation =>
            invocation.Identifier == "isFormControl" &&
            invocation.Arguments.Count > 1 &&
            Equals(invocation.Arguments[1], "settings-form"));
    }

    [Fact]
    public void Inherited_direction_flips_horizontal_back_key()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, nameof(BradixDirectionProvider.Dir), "rtl");
            builder.AddAttribute(2, nameof(BradixDirectionProvider.ChildContent), (RenderFragment) (contentBuilder =>
            {
                CreateSlider(defaultValues: [20])(contentBuilder);
            }));
            builder.CloseComponent();
        });

        var thumb = cut.Find("[role='slider']");
        thumb.KeyDown(new KeyboardEventArgs { Key = "ArrowLeft" });
        thumb = cut.Find("[role='slider']");

        Assert.Equal("21", thumb.GetAttribute("aria-valuenow"));
    }

    private static RenderFragment CreateSlider(IReadOnlyList<double> defaultValues, double minStepsBetweenThumbs = 0, string? name = null, Action? onValueCommit = null, string? form = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixSlider>(0);
            builder.AddAttribute(1, nameof(BradixSlider.DefaultValues), defaultValues);
            builder.AddAttribute(2, nameof(BradixSlider.MinStepsBetweenThumbs), minStepsBetweenThumbs);

            if (name is not null)
                builder.AddAttribute(3, nameof(BradixSlider.Name), name);

            if (onValueCommit is not null)
                builder.AddAttribute(5, nameof(BradixSlider.OnValueCommit), EventCallback.Factory.Create<IReadOnlyList<double>>(new object(), _ => onValueCommit()));

            if (form is not null)
                builder.AddAttribute(6, nameof(BradixSlider.Form), form);

            builder.AddAttribute(4, nameof(BradixSlider.ChildContent), (RenderFragment) (contentBuilder =>
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
