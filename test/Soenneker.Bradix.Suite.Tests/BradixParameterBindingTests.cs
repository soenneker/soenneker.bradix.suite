using System.Threading.Tasks;
using Bunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixParameterBindingTests : BunitContext
{
    [Test]
    public async Task Binding_assigns_inherited_and_own_parameters_and_captures_attributes()
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
