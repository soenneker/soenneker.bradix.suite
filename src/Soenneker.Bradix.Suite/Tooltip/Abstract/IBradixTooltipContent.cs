using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Tooltip floating content with popper placement and accessible labeling.
/// </summary>
public interface IBradixTooltipContent
{
    /// <summary>When true, keeps content mounted while closed.</summary>
    bool ForceMount { get; set; }

    /// <summary>Preferred popper side.</summary>
    string Side { get; set; }

    /// <summary>Offset along the side axis.</summary>
    double SideOffset { get; set; }

    /// <summary>Alignment along the cross axis.</summary>
    string Align { get; set; }

    /// <summary>Offset along the alignment axis.</summary>
    double AlignOffset { get; set; }

    /// <summary>Minimum padding between the arrow and content edges.</summary>
    double ArrowPadding { get; set; }

    /// <summary>When true, flips/shifts to stay in view.</summary>
    bool AvoidCollisions { get; set; }

    /// <summary>Padding used when resolving collisions.</summary>
    double CollisionPadding { get; set; }

    /// <summary>When true, hides the content if the anchor detaches.</summary>
    bool HideWhenDetached { get; set; }

    /// <summary>Explicit accessible name for the tooltip.</summary>
    string? AriaLabel { get; set; }

    /// <summary>Raised when Escape is pressed.</summary>
    EventCallback OnEscapeKeyDown { get; set; }

    /// <summary>Raised on pointer down outside.</summary>
    EventCallback OnPointerDownOutside { get; set; }

    /// <summary>Raised when Escape is pressed with detailed args.</summary>
    EventCallback<BradixEscapeKeyDownEventArgs> OnEscapeKeyDownDetailed { get; set; }

    /// <summary>Raised on pointer down outside with detailed args.</summary>
    EventCallback<BradixPointerDownOutsideEventArgs> OnPointerDownOutsideDetailed { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Visible tooltip content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Unregisters tooltip content interop.</summary>
    ValueTask DisposeAsync();

    /// <summary>Interop handler when another tooltip requests exclusive open.</summary>
    Task HandleTooltipOpenFromOutsideAsync();

    /// <summary>Interop handler when the trigger scrolls.</summary>
    Task HandleTooltipTriggerScrollAsync();

    /// <summary>Interop handler when the pointer leaves the grace area.</summary>
    Task HandleTooltipGraceAreaExitAsync();

    /// <summary>Interop handler when pointer grace transit state changes.</summary>
    Task HandlePointerGraceAreaChangedAsync(bool inTransit);
}
