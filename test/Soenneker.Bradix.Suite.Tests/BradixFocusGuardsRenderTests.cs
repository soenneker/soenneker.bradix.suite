using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

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

    [Fact]
    public void Focus_guards_render_child_content()
    {
        var cut = Render(builder =>
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

        Assert.Contains("Focusable", cut.Markup);
    }

    [Fact]
    public async Task Focus_guards_register_and_unregister_interop_on_lifecycle()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixFocusGuards>(0);
            builder.AddAttribute(1, nameof(BradixFocusGuards.ChildContent), (RenderFragment)(content =>
            {
                content.AddContent(0, "Focusable");
            }));
            builder.CloseComponent();
        });

        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "registerFocusGuards");

        await cut.InvokeAsync(() => cut.FindComponent<BradixFocusGuards>().Instance.DisposeAsync().AsTask());

        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "unregisterFocusGuards");
        Assert.Equal(1, _module.Invocations.Count(invocation => invocation.Identifier == "registerFocusGuards"));
        Assert.Equal(1, _module.Invocations.Count(invocation => invocation.Identifier == "unregisterFocusGuards"));
    }
}
