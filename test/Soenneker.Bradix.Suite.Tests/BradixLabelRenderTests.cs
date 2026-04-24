using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixLabelRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixLabelRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerLabelTextSelectionGuard", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterLabelTextSelectionGuard", _ => true).SetVoidResult();

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Label_renders_native_for_attribute_and_additional_attributes()
    {
        IRenderedComponent<BradixLabel> cut = Render<BradixLabel>(parameters => parameters
            .Add(label => label.Id, "first-name-label")
            .Add(label => label.For, "firstName")
            .Add(label => label.Class, "label")
            .AddUnmatched("data-testid", "label")
            .Add(label => label.ChildContent, (RenderFragment)(builder =>
            {
                builder.AddContent(0, "First name");
            })));

        IElement label = cut.Find("label");

        await Assert.That(label.Id).IsEqualTo("first-name-label");
        await Assert.That(label.GetAttribute("for")).IsEqualTo("firstName");
        await Assert.That(label.GetAttribute("class")).IsEqualTo("label");
        await Assert.That(label.GetAttribute("data-testid")).IsEqualTo("label");
        await Assert.That(label.TextContent).IsEqualTo("First name");
    }

    [Test]
    public async Task Label_forwards_delegated_mouse_down_callback()
    {
        var count = 0;

        IRenderedComponent<BradixLabel> cut = Render<BradixLabel>(parameters => parameters
            .Add(label => label.OnMouseDown, EventCallback.Factory.Create<MouseEventArgs>(this, () => count++))
            .Add(label => label.ChildContent, (RenderFragment)(builder =>
            {
                builder.AddContent(0, "First name");
            })));

        await cut.Instance.HandleMouseDownFromJs(new BradixDelegatedMouseEvent { Detail = 2 });

        await Assert.That(count).IsEqualTo(1);
    }
}
