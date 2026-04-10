using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixFocusGuardsRenderTests : BunitContext
{
    public BradixFocusGuardsRenderTests()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerFocusGuards", _ => true).SetVoidResult();
        module.SetupVoid("unregisterFocusGuards", _ => true).SetVoidResult();

        Services.AddScoped<BradixSuiteInterop>();
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
}
