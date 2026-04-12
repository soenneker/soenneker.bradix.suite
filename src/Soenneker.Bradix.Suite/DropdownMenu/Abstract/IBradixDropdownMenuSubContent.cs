using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixDropdownMenuSubContent"/>.</summary>
public interface IBradixDropdownMenuSubContent
{
    /// <summary>Gets or sets a value indicating whether content is mounted while closed for measurement.</summary>
    bool ForceMount { get; set; }

    /// <summary>Gets or sets a value indicating whether focus loops at the ends of the submenu.</summary>
    bool Loop { get; set; }

    /// <summary>Gets or sets the offset from the side edge.</summary>
    double SideOffset { get; set; }

    /// <summary>Gets or sets the alignment offset.</summary>
    double AlignOffset { get; set; }

    /// <summary>Gets or sets the minimum padding between the arrow and the edges of the content.</summary>
    double ArrowPadding { get; set; }

    /// <summary>Gets or sets a value indicating whether to flip the content to avoid collisions.</summary>
    bool AvoidCollisions { get; set; }

    /// <summary>Gets or sets the padding used when resolving collisions.</summary>
    double CollisionPadding { get; set; }

    /// <summary>Gets or sets a value indicating whether to hide content when detached from the anchor.</summary>
    bool HideWhenDetached { get; set; }

    /// <summary>Gets or sets the CSS class names.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }

    /// <summary>Gets or sets additional attributes spread onto the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets the callback invoked when the content receives auto focus on open.</summary>
    EventCallback OnOpenAutoFocus { get; set; }

    /// <summary>Gets or sets the callback invoked when the content receives auto focus on open, with event details.</summary>
    EventCallback<BradixAutoFocusEventArgs> OnOpenAutoFocusDetailed { get; set; }

    /// <summary>Gets or sets the callback invoked when the escape key is pressed.</summary>
    EventCallback OnEscapeKeyDown { get; set; }

    /// <summary>Gets or sets the callback invoked when the escape key is pressed, with event details.</summary>
    EventCallback<BradixEscapeKeyDownEventArgs> OnEscapeKeyDownDetailed { get; set; }

    /// <summary>Gets or sets the callback invoked on pointer down outside.</summary>
    EventCallback OnPointerDownOutside { get; set; }

    /// <summary>Gets or sets the callback invoked on pointer down outside, with event details.</summary>
    EventCallback<BradixPointerDownOutsideEventArgs> OnPointerDownOutsideDetailed { get; set; }

    /// <summary>Gets or sets the callback invoked on focus outside.</summary>
    EventCallback OnFocusOutside { get; set; }

    /// <summary>Gets or sets the callback invoked on focus outside, with event details.</summary>
    EventCallback<BradixFocusOutsideEventArgs> OnFocusOutsideDetailed { get; set; }

    /// <summary>Gets or sets the callback invoked on interact outside.</summary>
    EventCallback OnInteractOutside { get; set; }

    /// <summary>Gets or sets the callback invoked on interact outside, with event details.</summary>
    EventCallback<BradixInteractOutsideEventArgs> OnInteractOutsideDetailed { get; set; }

    /// <summary>Gets or sets the callback invoked when a key is pressed on the content.</summary>
    EventCallback<KeyboardEventArgs> OnContentKeyDown { get; set; }

    /// <summary>Gets or sets the callback invoked when the content is placed.</summary>
    EventCallback OnPlaced { get; set; }
}
