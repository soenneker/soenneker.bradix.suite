using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixTabsTrigger"/>.
/// </summary>
public interface IBradixTabsTrigger
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

    /// <summary>Gets or sets the value identifying this tab.</summary>
    string Value { get; set; }

    /// <summary>Gets or sets whether this trigger is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Gets or sets the callback invoked on mouse down.</summary>
    EventCallback<MouseEventArgs> OnMouseDown { get; set; }

    /// <summary>Gets or sets the callback invoked on key down.</summary>
    EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

    /// <summary>Gets or sets the callback invoked on focus.</summary>
    EventCallback<FocusEventArgs> OnFocus { get; set; }

    /// <summary>Releases script registrations for this trigger.</summary>
    ValueTask DisposeAsync();

    /// <summary>Called from script when delegated interaction wiring is ready.</summary>
    Task HandleDelegatedInteractionReadyAsync();

    /// <summary>Called from script for delegated mouse down handling.</summary>
    Task HandleDelegatedMouseDownAsync(BradixDelegatedMouseEvent _);

    /// <summary>Called from script when the roving-focus bridge is ready.</summary>
    Task HandleRovingFocusBridgeReadyAsync();

    /// <summary>Called from script for delegated key handling.</summary>
    Task HandleDelegatedKeyDownAsync(BradixDelegatedKeyboardEvent args);
}
