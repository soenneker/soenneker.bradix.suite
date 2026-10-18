using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixRadixParityTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixRadixParityTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.Mode = JSRuntimeMode.Loose;
        _module.Setup<bool>("isFormControl", _ => true).SetResult(true);
        Services.AddBradixTestInterops();
    }

    [Test]
    public async Task Popover_title_and_description_connect_only_while_present()
    {
        RenderFragment parts = b =>
        {
            b.OpenComponent<BradixPopoverContent>(0);
            b.AddAttribute(1, "ChildContent", (RenderFragment)(c =>
            {
                c.OpenComponent<BradixPopoverTitle>(0); c.AddAttribute(1, "Id", "popover-heading"); c.CloseComponent();
                c.OpenComponent<BradixPopoverDescription>(2); c.AddAttribute(3, "Id", "popover-description"); c.CloseComponent();
            }));
            b.CloseComponent();
        };
        var cut = Render<BradixPopover>(p => p.Add(c => c.DefaultOpen, true).AddChildContent(parts));
        await Assert.That(cut.Find("[role=dialog]").GetAttribute("aria-labelledby")).IsEqualTo("popover-heading");
        await Assert.That(cut.Find("[role=dialog]").GetAttribute("aria-describedby")).IsEqualTo("popover-description");
        cut.Render(p => p.AddChildContent<BradixPopoverContent>(p => p.Add(c => c.ChildContent, (RenderFragment)(_ => { }))));
        await cut.WaitForAssertionAsync(async () =>
            await Assert.That(cut.Find("[role=dialog]").HasAttribute("aria-labelledby")).IsFalse());
    }

    [Test]
    public async Task Decimal_slider_keyboard_steps_do_not_stall_on_grid_points()
    {
        var cut = Render<BradixSlider>(p => p.Add(c => c.DefaultValues, new double[] { 0.3 })
            .Add(c => c.Step, 0.1).Add(c => c.Max, 1).AddChildContent<BradixSliderThumb>());
        await cut.Find("[role=slider]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });
        await Assert.That(cut.Find("[role=slider]").GetAttribute("aria-valuenow")).IsEqualTo("0.4");
    }

    [Test]
    public async Task Preserved_slider_thumb_order_clamps_at_neighbor()
    {
        var cut = Render<BradixSlider>(p => p.Add(c => c.DefaultValues, new double[] { 20, 40 })
            .Add(c => c.PreserveThumbOrder, true).Add(c => c.MinStepsBetweenThumbs, 5)
            .AddChildContent<BradixSliderThumb>());
        await cut.InvokeAsync(() => cut.Instance.HandlePointerStart(0.2, 0, 0));
        await cut.InvokeAsync(() => cut.Instance.HandlePointerMove(0.8, 0));
        await Assert.That(cut.Find("[role=slider]").GetAttribute("aria-valuenow")).IsEqualTo("35");
    }

    [Test]
    public async Task Scroll_area_explicit_content_avoids_double_measurement_wrappers()
    {
        var cut = Render<BradixScrollArea>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<BradixScrollAreaViewport>(0);
            b.AddAttribute(1, "DisableImplicitContentElement", true);
            b.AddAttribute(2, "ChildContent", (RenderFragment)(c =>
            {
                c.OpenComponent<BradixScrollAreaContent>(0);
                c.AddAttribute(1, "ChildContent", (RenderFragment)(text => text.AddContent(0, "Measured content")));
                c.CloseComponent();
            }));
            b.CloseComponent();
        }));
        await Assert.That(cut.FindAll("[data-radix-scroll-area-viewport] > div").Count).IsEqualTo(1);
        await Assert.That(_module.Invocations.Count(i => i.Identifier == "registerScrollAreaViewport")).IsEqualTo(1);
        cut.Render(p => p.AddChildContent<BradixScrollAreaViewport>(v => v
            .Add(c => c.DisableImplicitContentElement, true)
            .Add(c => c.ChildContent, (RenderFragment)(_ => { }))));
        await Assert.That(_module.Invocations.Count(i => i.Identifier == "unregisterScrollAreaViewport")).IsEqualTo(1);
    }

    [Test]
    public async Task Composed_switch_can_omit_or_relocate_its_form_input()
    {
        var cut = Render<BradixSwitchProvider>(p => p.Add(c => c.Name, "enabled")
            .AddChildContent<BradixSwitchTrigger>());
        await Assert.That(cut.FindAll("input").Count).IsEqualTo(0);
        await cut.Find("button").ClickAsync();
        await Assert.That(cut.Find("button").GetAttribute("aria-checked")).IsEqualTo("true");
        cut.Render(p => p.AddChildContent(b =>
        {
            b.OpenComponent<BradixSwitchTrigger>(0); b.CloseComponent();
            b.OpenElement(1, "section");
            b.OpenComponent<BradixSwitchBubbleInput>(2); b.CloseComponent();
            b.CloseElement();
        }));
        await Assert.That(cut.Find("section input").GetAttribute("name")).IsEqualTo("enabled");
        await Assert.That(cut.Find("section input").HasAttribute("checked")).IsTrue();
    }

    [Test]
    public async Task Composed_slider_thumb_preserves_semantics_and_keyboard_behavior()
    {
        var cut = Render<BradixSlider>(p => p.Add(c => c.DefaultValues, new double[] { 30 })
            .AddChildContent(b =>
            {
                b.OpenComponent<BradixSliderThumbProvider>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(c =>
                {
                    c.OpenComponent<BradixSliderThumbTrigger>(0); c.CloseComponent();
                    c.OpenComponent<BradixSliderBubbleInput>(1); c.CloseComponent();
                }));
                b.CloseComponent();
            }));
        await cut.Find("[role=slider]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });
        await Assert.That(cut.Find("[role=slider]").GetAttribute("aria-valuenow")).IsEqualTo("31");
        await Assert.That(cut.FindAll("input").Count).IsEqualTo(1);
    }

    [Test]
    public async Task Programmatic_switch_changes_do_not_emit_clicks_but_delayed_user_changes_do()
    {
        bool? requested = null;
        var cut = Render<BradixSwitch>(p => p.Add(c => c.Checked, false)
            .Add(c => c.Form, "settings")
            .Add(c => c.CheckedChanged, value => requested = value));
        cut.Render(p => p.Add(c => c.Checked, true));
        await Assert.That((bool)_module.Invocations.Last(i => i.Identifier == "syncCheckboxBubbleInputState").Arguments[3]!).IsFalse();
        await cut.Find("button").ClickAsync();
        await Assert.That(requested).IsFalse();
        await Assert.That(cut.Find("button").GetAttribute("aria-checked")).IsEqualTo("true");
        cut.Render(p => p.Add(c => c.Checked, false));
        await Assert.That((bool)_module.Invocations.Last(i => i.Identifier == "syncCheckboxBubbleInputState").Arguments[3]!).IsTrue();
    }

    [Test]
    public async Task Context_menu_controlled_state_changes_without_replacing_root()
    {
        var cut = Render<BradixContextMenu>(p => p.Add(c => c.Open, false)
            .AddChildContent<BradixContextMenuTrigger>());
        await Assert.That(cut.Find("span").GetAttribute("data-state")).IsEqualTo("closed");
        cut.Render(p => p.Add(c => c.Open, true));
        await Assert.That(cut.Find("span").GetAttribute("data-state")).IsEqualTo("open");
        cut.Render(p => p.Add(c => c.Open, false));
        await Assert.That(cut.Find("span").GetAttribute("data-state")).IsEqualTo("closed");
    }

    [Test]
    public async Task Slider_steps_to_next_grid_value_and_commits_crossing_thumbs()
    {
        IReadOnlyList<double>? committed = null;
        var cut = Render<BradixSlider>(p => p.Add(c => c.DefaultValues, new double[] { 15, 20 })
            .Add(c => c.Step, 10).Add(c => c.OnValueCommit, v => committed = v)
            .AddChildContent<BradixSliderThumb>());
        await cut.Find("[role=slider]").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });
        await Assert.That(cut.Find("[role=slider]").GetAttribute("aria-valuenow")).IsEqualTo("10");
        await cut.InvokeAsync(() => cut.Instance.HandlePointerStart(0.1, 0, 0));
        await cut.InvokeAsync(() => cut.Instance.HandlePointerMove(0.3, 0));
        await cut.InvokeAsync(() => cut.Instance.HandlePointerMove(0.2, 0));
        committed = null;
        await cut.InvokeAsync(() => cut.Instance.HandlePointerEnd());
        await Assert.That(committed).IsNotNull();
    }

    [Test]
    public async Task Slider_reset_restores_initial_values_without_committing()
    {
        var commits = 0;
        var cut = Render<BradixSlider>(p => p.Add(c => c.DefaultValues, new double[] { 25 })
            .Add(c => c.OnValueCommit, _ => commits++)
            .AddChildContent<BradixSliderThumb>());
        await cut.InvokeAsync(() => cut.Instance.HandlePointerStart(0.8, 0, -1));
        await cut.InvokeAsync(() => cut.Instance.HandleFormReset());
        await Assert.That(cut.Find("[role=slider]").GetAttribute("aria-valuenow")).IsEqualTo("25");
        await Assert.That(commits).IsEqualTo(0);
    }

    [Test]
    public async Task Password_visibility_toggle_and_form_reset_preserve_input_value()
    {
        var cut = Render<BradixPasswordToggleField>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<BradixPasswordToggleFieldInput>(0);
            b.AddAttribute(1, "Value", "secret");
            b.AddAttribute(2, "Id", "password");
            b.CloseComponent();
            b.OpenComponent<BradixPasswordToggleFieldToggle>(3);
            b.AddAttribute(4, "ChildContent", (RenderFragment)(c => c.AddContent(0, "Toggle")));
            b.CloseComponent();
        }));
        await Assert.That(cut.Find("input").GetAttribute("type")).IsEqualTo("password");
        await Assert.That(cut.Find("button").GetAttribute("aria-controls")).IsEqualTo("password");
        await cut.Find("button").ClickAsync();
        await Assert.That(cut.Find("input").GetAttribute("type")).IsEqualTo("text");
        await cut.InvokeAsync(() => cut.FindComponent<BradixPasswordToggleFieldInput>().Instance.HidePassword());
        await Assert.That(cut.Find("input").GetAttribute("type")).IsEqualTo("password");
        await Assert.That(cut.Find("input").GetAttribute("value")).IsEqualTo("secret");
    }

    [Test]
    public async Task Password_controlled_visibility_only_changes_when_parent_accepts_request()
    {
        bool? requested = null;
        var cut = Render<BradixPasswordToggleField>(p => p.Add(c => c.Visible, false)
            .Add(c => c.VisibleChanged, v => requested = v)
            .AddChildContent<BradixPasswordToggleFieldToggle>());
        await cut.Find("button").ClickAsync();
        await Assert.That(requested).IsTrue();
        await Assert.That(cut.Find("button").GetAttribute("data-state")).IsEqualTo("hidden");
        cut.Render(p => p.Add(c => c.Visible, true));
        await Assert.That(cut.Find("button").GetAttribute("data-state")).IsEqualTo("visible");
    }
}
