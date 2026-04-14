using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixRemoveScrollRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixRemoveScrollRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterRemoveScroll", _ => true).SetVoidResult();

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Remove_scroll_renders_child_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixRemoveScroll>(0);
            builder.AddAttribute(1, nameof(BradixRemoveScroll.ChildContent), (RenderFragment)(content =>
            {
                content.OpenElement(0, "div");
                content.AddContent(1, "Locked content");
                content.CloseElement();
            }));
            builder.CloseComponent();
        });

        await Assert.That(cut.Markup).Contains("Locked content");
        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerRemoveScroll")).IsTrue();
    }

    [Test]
    public async Task Remove_scroll_forwards_allow_pinch_zoom_to_interop()
    {
        Render(builder =>
        {
            builder.OpenComponent<BradixRemoveScroll>(0);
            builder.AddAttribute(1, nameof(BradixRemoveScroll.AllowPinchZoom), true);
            builder.CloseComponent();
        });

        await Assert.That(_module.Invocations.Any(invocation =>
            invocation.Identifier == "registerRemoveScroll" &&
            invocation.Arguments.Count > 0 &&
            invocation.Arguments[0] is bool allowPinchZoom &&
            allowPinchZoom)).IsTrue();
    }

    [Test]
    public async Task Remove_scroll_unregisters_interop_on_dispose()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixRemoveScroll>(0);
            builder.CloseComponent();
        });

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerRemoveScroll")).IsTrue();

        await cut.InvokeAsync(() => cut.FindComponent<BradixRemoveScroll>().Instance.DisposeAsync().AsTask());

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "unregisterRemoveScroll")).IsTrue();
        await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "registerRemoveScroll")).IsEqualTo(1);
        await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "unregisterRemoveScroll")).IsEqualTo(1);
    }
}