using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixFocusGuardsRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixFocusGuardsRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerFocusGuards", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusGuards", _ => true).SetVoidResult();

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Focus_guards_render_child_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixFocusGuards>(0);
            builder.AddAttribute(1, nameof(BradixFocusGuards.ChildContent), (RenderFragment)(content =>
            {
                content.OpenElement(0, "button");
                content.AddContent(1, "Focusable");
                content.CloseElement();
            }));
            builder.CloseComponent();
        });

        await Assert.That(cut.Markup).Contains("Focusable");
    }

    [Test]
    public async Task Focus_guards_register_and_unregister_interop_on_lifecycle()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixFocusGuards>(0);
            builder.AddAttribute(1, nameof(BradixFocusGuards.ChildContent), (RenderFragment)(content =>
            {
                content.AddContent(0, "Focusable");
            }));
            builder.CloseComponent();
        });

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerFocusGuards")).IsTrue();

        await cut.InvokeAsync(() => cut.FindComponent<BradixFocusGuards>().Instance.DisposeAsync().AsTask());

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "unregisterFocusGuards")).IsTrue();
        await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "registerFocusGuards")).IsEqualTo(1);
        await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "unregisterFocusGuards")).IsEqualTo(1);
    }
}