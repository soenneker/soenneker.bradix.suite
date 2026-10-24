using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixControlledValueTests : BunitContext
{
    public BradixControlledValueTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddBradixTestInterops();
    }

    [Test]
    [Arguments(typeof(BradixTabs), "")]
    [Arguments(typeof(BradixNavigationMenu), "")]
    [Arguments(typeof(BradixMenubar), null)]
    [Arguments(typeof(BradixMenuRadioGroup), null)]
    [Arguments(typeof(BradixOneTimePasswordField), "")]
    public async Task Clearing_controlled_selection_does_not_restore_default(Type type, string? empty)
    {
        var parameters = new Dictionary<string, object>
        {
            ["DefaultValue"] = "1",
            ["Value"] = "2",
            ["ValueChanged"] = EventCallback.Factory.Create<string>(this, _ => {})
        };
        var cut = Render<DynamicComponent>(p => p.Add(c => c.Type, type).Add(c => c.Parameters, parameters));
        var currentValue = type.GetProperty("CurrentValue", BindingFlags.Instance | BindingFlags.NonPublic)!;
        await Assert.That(currentValue.GetValue(cut.Instance.Instance)).IsEqualTo("2");
        parameters["Value"] = null!;
        cut.Render(p => p.Add(c => c.Parameters, parameters));
        await Assert.That(currentValue.GetValue(cut.Instance.Instance)).IsEqualTo(empty);
    }
}
