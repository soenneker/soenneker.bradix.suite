using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixSelectContent"/>.
/// </summary>
public interface IBradixSelectContent
{
    /// <summary>Gets or sets the element identifier.</summary>
    string? Id { get; set; }

    /// <summary>Gets or sets the CSS class.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }

    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets additional attributes merged onto the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Gets or sets whether the content stays mounted when closed.</summary>
    bool ForceMount { get; set; }

    /// <summary>Gets or sets the positioning mode (<c>item-aligned</c> or <c>popper</c>).</summary>
    string Position { get; set; }

    /// <summary>Gets or sets the popper side.</summary>
    string Side { get; set; }

    /// <summary>Gets or sets the popper side offset.</summary>
    double SideOffset { get; set; }

    /// <summary>Gets or sets the popper alignment.</summary>
    string Align { get; set; }

    /// <summary>Gets or sets the popper alignment offset.</summary>
    double AlignOffset { get; set; }

    /// <summary>Gets or sets padding reserved for the arrow when using popper positioning.</summary>
    double ArrowPadding { get; set; }

    /// <summary>Gets or sets whether the popper should avoid collisions.</summary>
    bool AvoidCollisions { get; set; }

    /// <summary>Gets or sets collision padding for the popper.</summary>
    double CollisionPadding { get; set; }

    /// <summary>Gets or sets whether the popper content hides when detached.</summary>
    bool HideWhenDetached { get; set; }

    /// <summary>Gets or sets the callback invoked when escape is pressed.</summary>
    EventCallback OnEscapeKeyDown { get; set; }

    /// <summary>Gets or sets the detailed callback invoked when escape is pressed.</summary>
    EventCallback<BradixEscapeKeyDownEventArgs> OnEscapeKeyDownDetailed { get; set; }

    /// <summary>Gets or sets the callback invoked on pointer down outside.</summary>
    EventCallback OnPointerDownOutside { get; set; }

    /// <summary>Gets or sets the detailed callback invoked on pointer down outside.</summary>
    EventCallback<BradixPointerDownOutsideEventArgs> OnPointerDownOutsideDetailed { get; set; }

    /// <summary>Gets or sets the callback invoked when focus returns after close.</summary>
    EventCallback OnCloseAutoFocus { get; set; }

    /// <summary>Gets or sets the detailed callback invoked when focus returns after close.</summary>
    EventCallback<BradixAutoFocusEventArgs> OnCloseAutoFocusDetailed { get; set; }

    /// <summary>Gets or sets the callback invoked after the popper is placed.</summary>
    EventCallback OnPlaced { get; set; }

    /// <summary>Releases script registrations for the content layer.</summary>
    ValueTask DisposeAsync();

    /// <summary>Called from script when viewport scroll metrics change.</summary>
    Task HandleViewportMetricsChanged(double scrollTop, double scrollHeight, double viewportHeight);

    /// <summary>Called from script with pointer guard results from the trigger.</summary>
    Task HandleTriggerPointerGuardResult(bool suppressSelection, bool shouldClose);

    /// <summary>Called from script when a window-level dismiss should run.</summary>
    Task HandleWindowDismiss();

    /// <summary>Called from script for delegated keyboard handling in the content.</summary>
    Task HandleDelegatedContentKeyDown(BradixDelegatedKeyboardEvent args);
}
