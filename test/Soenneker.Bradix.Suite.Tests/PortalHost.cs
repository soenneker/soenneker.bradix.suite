using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Soenneker.Bradix.Suite.Tests;

internal sealed class PortalHost : ComponentBase
{
    private int _version;

    private void HandleClick()
    {
        _version++;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "button");
        builder.AddAttribute(1, "type", "button");
        builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, HandleClick));
        builder.AddContent(3, "Rerender");
        builder.CloseElement();

        builder.OpenComponent<BradixPortal>(4);
        builder.AddAttribute(5, nameof(BradixPortal.ChildContent), (RenderFragment)(contentBuilder =>
        {
            contentBuilder.OpenElement(0, "div");
            contentBuilder.AddContent(1, $"Portaled content {_version}");
            contentBuilder.CloseElement();
        }));
        builder.CloseComponent();
    }
}
