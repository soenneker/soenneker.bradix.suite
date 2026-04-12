using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixSwitch"/>.
/// </summary>
public interface IBradixSwitch
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

    /// <summary>Gets or sets the controlled checked state.</summary>
    bool? Checked { get; set; }

    /// <summary>Gets or sets the initial checked state when uncontrolled.</summary>
    bool DefaultChecked { get; set; }

    /// <summary>Gets or sets whether the switch is required in a form.</summary>
    bool Required { get; set; }

    /// <summary>Gets or sets whether the switch is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Gets or sets the field name for the native bubble input.</summary>
    string? Name { get; set; }

    /// <summary>Gets or sets the <c>form</c> attribute for detached native inputs.</summary>
    string? Form { get; set; }

    /// <summary>Gets or sets the value submitted with the native input.</summary>
    string Value { get; set; }

    /// <summary>Gets or sets the callback invoked when checked state changes (two-way bind).</summary>
    EventCallback<bool> CheckedChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when checked state changes.</summary>
    EventCallback<bool> OnCheckedChange { get; set; }

    /// <summary>Releases script registrations for the switch.</summary>
    ValueTask DisposeAsync();

    /// <summary>Called from script when delegated interaction wiring is ready.</summary>
    Task HandleDelegatedInteractionReadyAsync();

    /// <summary>Called from script when the owning form is reset.</summary>
    Task HandleFormResetAsync();

    /// <summary>Called from script for delegated click handling.</summary>
    Task HandleDelegatedClickAsync(BradixDelegatedMouseEvent _);
}
