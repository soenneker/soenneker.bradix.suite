using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixFormAssociationTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixFormAssociationTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.Mode = JSRuntimeMode.Loose;
        _module.Setup<bool>("isFormControl", _ => true).SetResult(true);
        Services.AddBradixTestInterops();
    }

    [Test]
    public async Task Form_changes_rebind_all_reset_handlers_once()
    {
        var checkbox = Render<BradixCheckbox>(p => p.Add(c => c.Form, "first"));
        var toggle = Render<BradixSwitch>(p => p.Add(c => c.Form, "first"));
        var radio = Render<BradixRadioGroup>(p => p.Add(c => c.Form, "first"));
        var slider = Render<BradixSlider>(p => p.Add(c => c.Form, "first"));
        checkbox.Render(p => p.Add(c => c.Form, "second"));
        toggle.Render(p => p.Add(c => c.Form, "second"));
        radio.Render(p => p.Add(c => c.Form, "second"));
        slider.Render(p => p.Add(c => c.Form, "second"));
        checkbox.Render(); toggle.Render(); radio.Render(); slider.Render();
        await Assert.That(_module.Invocations["registerCheckboxRoot"].Count).IsEqualTo(2);
        await Assert.That(_module.Invocations["registerCheckboxRoot"].Last().Arguments.Last()).IsEqualTo("second");
        var resets = _module.Invocations["registerAssociatedFormReset"];
        await Assert.That(resets.Count).IsEqualTo(6);
        await Assert.That(resets.Count(x => Equals(x.Arguments.Last(), "second"))).IsEqualTo(3);
    }

    [Test]
    public async Task Controlled_radio_can_be_cleared_despite_default_value()
    {
        var cut = Render<BradixRadioGroup>(p => p.Add(c => c.DefaultValue, "default")
            .Add(c => c.Value, "chosen").Add(c => c.ValueChanged, (string? _) => {})
            .AddChildContent<BradixRadioGroupItem>(p => p.Add(c => c.Value, "default")));
        cut.Render(p => p.Add(c => c.Value, (string?)null));
        await Assert.That(cut.Find("[role=radio]").GetAttribute("aria-checked")).IsEqualTo("false");
    }
}
