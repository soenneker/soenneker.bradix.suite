using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Floating content positioned relative to the popper anchor via JS interop.
/// </summary>
public interface IBradixPopperContent : IAsyncDisposable {
    /// <summary>Preferred popper side.</summary>
    BradixSide Side { get; set; }

    /// <summary>Offset along the side axis.</summary>
    double SideOffset { get; set; }

    /// <summary>Alignment along the cross axis.</summary>
    BradixAlignment Align { get; set; }

    /// <summary>Offset along the alignment axis.</summary>
    double AlignOffset { get; set; }

    /// <summary>Minimum padding between the arrow and content edges.</summary>
    double ArrowPadding { get; set; }

    /// <summary>When true, flips/shifts to stay in view.</summary>
    bool AvoidCollisions { get; set; }

    /// <summary>Padding used when resolving collisions.</summary>
    double CollisionPadding { get; set; }

    /// <summary>CSS selector for an explicit Floating UI collision boundary.</summary>
    string? CollisionBoundarySelector { get; set; }

    /// <summary>CSS selectors for explicit Floating UI collision boundaries.</summary>
    IReadOnlyList<string>? CollisionBoundarySelectors { get; set; }

    /// <summary>How strongly shifted content sticks to the collision boundary.</summary>
    string Sticky { get; set; }

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


    /// <summary>Interop handler when popper geometry updates.</summary>
    Task HandlePositionChanged(string side, string align, double left, double top, double availableWidth, double availableHeight, double anchorWidth,
        double anchorHeight, double? arrowX, double? arrowY, bool shouldHideArrow, bool hidden, string transformOriginX, string transformOriginY);
}
