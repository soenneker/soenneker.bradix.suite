using Bunit;
using Bunit.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Bradix.Suite.Interop;
using Soenneker.Bradix.Suite.RemoveScroll;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixRemoveScrollRenderTests : Bunit.BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixRemoveScrollRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterRemoveScroll", _ => true).SetVoidResult();

        Services.AddScoped<BradixSuiteInterop>();
    }

    [Fact]
    public void Remove_scroll_renders_child_content()
    {
        var cut = Render(builder =>
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

        Assert.Contains("Locked content", cut.Markup);
        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "registerRemoveScroll");
    }

    [Fact]
    public void Remove_scroll_forwards_allow_pinch_zoom_to_interop()
    {
        Render(builder =>
        {
            builder.OpenComponent<BradixRemoveScroll>(0);
            builder.AddAttribute(1, nameof(BradixRemoveScroll.AllowPinchZoom), true);
            builder.CloseComponent();
        });

        Assert.Contains(_module.Invocations, invocation =>
            invocation.Identifier == "registerRemoveScroll" &&
            invocation.Arguments.Count > 0 &&
            invocation.Arguments[0] is bool allowPinchZoom &&
            allowPinchZoom);
    }
}
