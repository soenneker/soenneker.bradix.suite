using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixToggle"/>.
/// </summary>
public interface IBradixToggle
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

    /// <summary>Gets or sets the controlled pressed state.</summary>
    bool? Pressed { get; set; }

    /// <summary>Gets or sets the initial pressed state when uncontrolled.</summary>
    bool DefaultPressed { get; set; }

    /// <summary>Gets or sets whether the toggle is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Gets or sets whether <c>aria-pressed</c> is rendered.</summary>
    bool RenderAriaPressed { get; set; }

    /// <summary>Gets or sets the callback invoked when pressed state changes (two-way bind).</summary>
    EventCallback<bool> PressedChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when pressed state changes.</summary>
    EventCallback<bool> OnPressedChange { get; set; }

    /// <summary>Gets or sets the callback invoked on click.</summary>
    EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>Gets or sets the callback invoked on mouse down.</summary>
    EventCallback<MouseEventArgs> OnMouseDown { get; set; }

    /// <summary>Gets or sets the callback invoked on focus.</summary>
    EventCallback<FocusEventArgs> OnFocus { get; set; }

    /// <summary>Gets or sets the callback invoked on key down.</summary>
    EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

    /// <summary>Releases script registrations for the toggle.</summary>
    ValueTask DisposeAsync();

    /// <summary>Called from script when delegated interaction wiring is ready.</summary>
    Task HandleDelegatedInteractionReadyAsync();

    /// <summary>Called from script for delegated click handling.</summary>
    Task HandleDelegatedClickAsync(BradixDelegatedMouseEvent _);
}
