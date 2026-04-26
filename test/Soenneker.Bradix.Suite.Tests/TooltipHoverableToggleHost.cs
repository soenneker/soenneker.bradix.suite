using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Soenneker.Bradix.Suite.Tests;

internal sealed class TooltipHoverableToggleHost : ComponentBase
{
    private bool _disableHoverableContent;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");

        builder.OpenElement(1, "button");
        builder.AddAttribute(2, "type", "button");
        builder.AddAttribute(3, "data-toggle-hoverable", "true");
        builder.AddAttribute(4, "onclick", EventCallback.Factory.Create(this, () => _disableHoverableContent = !_disableHoverableContent));
        builder.AddContent(5, "Toggle hoverable");
        builder.CloseElement();

        builder.OpenComponent<BradixTooltip>(6);
        builder.AddAttribute(7, nameof(BradixTooltip.DefaultOpen), true);
        builder.AddAttribute(8, nameof(BradixTooltip.DelayDuration), 0);
        builder.AddAttribute(9, nameof(BradixTooltip.DisableHoverableContent), _disableHoverableContent);
        builder.AddAttribute(10, nameof(BradixTooltip.ChildContent), (RenderFragment)(content =>
        {
            content.OpenComponent<BradixTooltipTrigger>(0);
            content.AddAttribute(1, nameof(BradixTooltipTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, "Trigger")));
            content.CloseComponent();

            content.OpenComponent<BradixTooltipPortal>(2);
            content.AddAttribute(3, nameof(BradixTooltipPortal.ChildContent), (RenderFragment)(portal =>
            {
                portal.OpenComponent<BradixTooltipContent>(0);
                portal.AddAttribute(1, nameof(BradixTooltipContent.Class), "tooltip-content");
                portal.AddAttribute(2, nameof(BradixTooltipContent.ChildContent), (RenderFragment)(tooltipContent => tooltipContent.AddContent(0, "Tooltip body")));
                portal.CloseComponent();
            }));
            content.CloseComponent();
        }));
        builder.CloseComponent();

        builder.CloseElement();
    }
}
