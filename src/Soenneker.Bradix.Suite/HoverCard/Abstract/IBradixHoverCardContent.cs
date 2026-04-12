using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Hover card floating content with dismissable-layer and popper placement.
/// </summary>
public interface IBradixHoverCardContent : IAsyncDisposable {
    /// <summary>When true, keeps the subtree mounted for exit animations.</summary>
    bool ForceMount { get; set; }

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

    /// <summary>When true, hides the content if the anchor detaches.</summary>
    bool HideWhenDetached { get; set; }

    /// <summary>Raised when Escape is pressed.</summary>
    EventCallback OnEscapeKeyDown { get; set; }

    /// <summary>Raised on pointer down outside.</summary>
    EventCallback OnPointerDownOutside { get; set; }

    /// <summary>Raised on focus moving outside.</summary>
    EventCallback OnFocusOutside { get; set; }

    /// <summary>Raised on any outside interaction.</summary>
    EventCallback OnInteractOutside { get; set; }

    /// <summary>Raised when Escape is pressed with detailed args.</summary>
    EventCallback<BradixEscapeKeyDownEventArgs> OnEscapeKeyDownDetailed { get; set; }

    /// <summary>Raised on pointer down outside with detailed args.</summary>
    EventCallback<BradixPointerDownOutsideEventArgs> OnPointerDownOutsideDetailed { get; set; }

    /// <summary>Raised on focus outside with detailed args.</summary>
    EventCallback<BradixFocusOutsideEventArgs> OnFocusOutsideDetailed { get; set; }

    /// <summary>Raised on interact outside with detailed args.</summary>
    EventCallback<BradixInteractOutsideEventArgs> OnInteractOutsideDetailed { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Content inside the hover card.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }


    /// <summary>Interop handler when the document receives pointer up after selection.</summary>
    Task HandleDocumentPointerUp(bool hasSelection);
}
