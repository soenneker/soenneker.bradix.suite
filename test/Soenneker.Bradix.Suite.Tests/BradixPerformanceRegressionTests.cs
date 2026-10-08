using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixPerformanceRegressionTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;
    public BradixPerformanceRegressionTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.Mode = JSRuntimeMode.Loose;
        Services.AddBradixTestInterops();
    }

    [Test]
    public async Task Slot_observes_mutated_attribute_text_and_preserves_whitespace_order(CancellationToken cancellationToken)
    {
        var text = new MutableAttributeText(" child ");
        var style = new MutableAttributeText(" color:red;; ");
        var attributes = new Dictionary<string, object> { ["class"] = text, ["style"] = style };
        var cut = Render<BradixSlot>(p => p.Add(c => c.ElementName, "div").Add(c => c.Class, "parent")
            .Add(c => c.Style, "display:block").Add(c => c.ChildAttributes, attributes));
        await Assert.That(cut.Find("div").GetAttribute("class")).IsEqualTo("parent  child ");
        await Assert.That(cut.Find("div").GetAttribute("style")).IsEqualTo("display:block; color:red;");
        text.Value = "next"; style.Value = "color:blue";
        cut.Render(p => p.Add(c => c.ChildAttributes, attributes));
        await Assert.That(cut.Find("div").GetAttribute("class")).IsEqualTo("parent next");
        await Assert.That(cut.Find("div").GetAttribute("style")).IsEqualTo("display:block; color:blue;");
    }

    [Test]
    public async Task Toast_hotkey_snapshot_detects_in_place_mutation_and_joined_key_collisions(CancellationToken cancellationToken)
    {
        var keys = new List<string> { "Alt", "F8" };
        var cut = Render<BradixToastViewport>(p => p.Add(c => c.Hotkey, keys));
        int initial = _module.Invocations.Count(c => c.Identifier == "registerToastViewport");
        keys.Clear(); keys.Add("Alt|F8");
        cut.Render(p => p.Add(c => c.Hotkey, keys));
        await Assert.That(_module.Invocations.Count(c => c.Identifier == "registerToastViewport")).IsEqualTo(initial + 1);
        var serialized = (JsonElement)_module.Invocations.Last(c => c.Identifier == "registerToastViewport").Arguments[4]!;
        await Assert.That(serialized.GetArrayLength()).IsEqualTo(1);
        await Assert.That(serialized[0].GetString()).IsEqualTo("Alt|F8");
        cut.Render(p => p.Add(c => c.Hotkey, keys));
        await Assert.That(_module.Invocations.Count(c => c.Identifier == "registerToastViewport")).IsEqualTo(initial + 1);
    }

    [Test]
    public async Task Popper_observes_in_place_collision_boundary_changes(CancellationToken cancellationToken)
    {
        var selectors = new List<string> { "#first", "#second" };
        var cut = Render<BradixPopper>(p => p.AddChildContent(builder =>
        {
            builder.OpenComponent<BradixPopperAnchor>(0); builder.CloseComponent();
            builder.OpenComponent<BradixPopperContent>(1);
            builder.AddAttribute(2, nameof(BradixPopperContent.CollisionBoundarySelectors), selectors);
            builder.CloseComponent();
        }));
        selectors[1] = "#changed";
        cut.FindComponent<BradixPopperContent>().Render(p => p.Add(c => c.CollisionBoundarySelectors, selectors));
        var invocation = _module.Invocations.Last(c => c.Identifier is "registerPopperContent" or "updatePopperContent");
        var options = (JsonElement)invocation.Arguments[invocation.Identifier == "registerPopperContent" ? 4 : 2]!;
        await Assert.That(options.GetProperty("collisionBoundarySelectors")[1].GetString()).IsEqualTo("#changed");
    }

    [Test]
    public async Task Slider_owns_callback_snapshots_and_suppresses_noop_and_rejected_moves(CancellationToken cancellationToken)
    {
        var callbacks = new List<IReadOnlyList<double>>();
        var cut = Render<BradixSlider>(p => p.Add(c => c.DefaultValues, new double[] { 10, 70 })
            .Add(c => c.MinStepsBetweenThumbs, 10).Add(c => c.ValuesChanged, values => callbacks.Add(values)));
        await cut.InvokeAsync(() => cut.Instance.HandlePointerStart(.1, 0, 0));
        await cut.InvokeAsync(() => cut.Instance.HandlePointerMove(.69, 0));
        await Assert.That(callbacks.Count).IsEqualTo(0);
        await cut.InvokeAsync(() => cut.Instance.HandlePointerMove(.2, 0));
        await cut.InvokeAsync(() => cut.Instance.HandlePointerMove(.3, 0));
        await Assert.That(callbacks[0][0]).IsEqualTo(20);
        await Assert.That(callbacks[1][0]).IsEqualTo(30);
        ((double[])callbacks[0])[0] = 99;
        await cut.InvokeAsync(() => cut.Instance.HandlePointerMove(.4, 0));
        await Assert.That(callbacks[2][0]).IsEqualTo(40);
        await Assert.That(callbacks[1][0]).IsEqualTo(30);
    }

    [Test]
    public async Task Slider_buffer_paths_match_existing_sort_gap_and_equality_semantics(CancellationToken cancellationToken)
    {
        MethodInfo method = typeof(BradixSlider).GetMethod("BuildChangedValues", BindingFlags.Static | BindingFlags.NonPublic)!;
        Type math = typeof(BradixSlider).Assembly.GetType("Soenneker.Bradix.BradixSliderMath")!;
        MethodInfo sort = math.GetMethod("GetNextSortedValues")!;
        MethodInfo gapCheck = math.GetMethod("HasMinStepsBetweenValues")!;
        var random = new Random(410);
        foreach (int count in new[] { 1, 2, 8, 64, 65, 100 })
        foreach (double gap in new[] { 0d, 1d, double.NaN })
        foreach (double value in new[] { 0d, -0d, 5d, double.NaN, double.PositiveInfinity })
        {
            double[] current = Enumerable.Range(0, count).Select(_ => (double)random.Next(0, 10)).ToArray();
            if (count > 1) current[0] = double.NaN;
            int index = count / 2;
            var expected = (List<double>)sort.Invoke(null, [current, value, index])!;
            bool changed = expected.Where((item, i) => item != current[i]).Any();
            bool valid = (bool)gapCheck.Invoke(null, [expected, gap])!;
            object?[] arguments = [current, value, index, gap, 0];
            var actual = (double[]?)method.Invoke(null, arguments);
            await Assert.That(actual is null).IsEqualTo(!changed || !valid);
            if (actual is not null)
            {
                await Assert.That(actual.SequenceEqual(expected)).IsTrue();
                await Assert.That((int)arguments[4]!).IsEqualTo(expected.IndexOf(value));
            }
        }
    }
}
