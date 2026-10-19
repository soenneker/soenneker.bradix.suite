using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixGeneratedParameterTests : BunitContext
{
    [Test]
    public async Task Every_component_has_generated_bindings_and_an_opted_in_base()
    {
        // Test-only catalog inspection catches wrappers or providers accidentally left on framework binding.
        Type[] components = typeof(BradixElement).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && typeof(IComponent).IsAssignableFrom(t)).ToArray();
        await Assert.That(components.Length).IsGreaterThan(0);
        foreach (Type component in components)
        {
            bool optedIn = typeof(BradixElement).IsAssignableFrom(component) ||
                           typeof(BradixContent).IsAssignableFrom(component) ||
                           typeof(BradixComponent).IsAssignableFrom(component);
            await Assert.That(optedIn).IsTrue().Because(component.FullName!);
            await Assert.That(component.GetProperty("GeneratedParameterType",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                .IsNotNull().Because(component.FullName!);
        }
    }

    [Test]
    public async Task Generated_binding_assigns_inherited_and_own_parameters_and_captures_attributes()
    {
        var cut = Render<BradixToggle>(p => p.AddUnmatched("pReSsEd", true)
            .AddUnmatched("iD", "generated-toggle").AddUnmatched("data-test", "kept"));
        await Assert.That(cut.Find("button").Id).IsEqualTo("generated-toggle");
        await Assert.That(cut.Find("button").GetAttribute("aria-pressed")).IsEqualTo("true");
        await Assert.That(cut.Find("button").GetAttribute("data-test")).IsEqualTo("kept");
        cut.Render(p => p.Add(c => c.Pressed, false));
        await Assert.That(cut.Find("button").GetAttribute("aria-pressed")).IsEqualTo("false");
        await Assert.That(cut.Instance.Id).IsEqualTo("generated-toggle");
    }
}
