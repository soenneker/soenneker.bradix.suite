using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Floating content positioned relative to the popper anchor via JS interop.
/// </summary>
public interface IBradixPopperContent
{
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

    /// <summary>Raised after the popper completes placement.</summary>
    EventCallback OnPlaced { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Floating content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Unregisters popper content interop.</summary>
    ValueTask DisposeAsync();

    /// <summary>Interop handler when popper geometry updates.</summary>
    Task HandlePositionChangedAsync(string side, string align, double left, double top, double availableWidth, double availableHeight, double anchorWidth,
        double anchorHeight, double? arrowX, double? arrowY, bool shouldHideArrow, bool hidden, string transformOriginX, string transformOriginY);
}
