using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixSwitch"/>.
/// </summary>
public interface IBradixSwitch : IAsyncDisposable {
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


    /// <summary>
    /// Called from script when delegated interaction wiring is ready.
    /// </summary>
    /// <returns>A task that completes when the handle delegated interaction ready operation is complete.</returns>
    Task HandleDelegatedInteractionReady();

    /// <summary>
    /// Called from script when the owning form is reset.
    /// </summary>
    /// <returns>A task that completes when the handle form reset operation is complete.</returns>
    Task HandleFormReset();

    /// <summary>
    /// Called from script for delegated click handling.
    /// </summary>
    /// <param name="_">_ for the handle delegated click operation.</param>
    /// <returns>A task that completes when the handle delegated click operation is complete.</returns>
    Task HandleDelegatedClick(BradixDelegatedMouseEvent _);

    /// <summary>
    /// Called from script for delegated keyboard activation handling.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>A task that completes when the handle delegated key down operation is complete.</returns>
    Task HandleDelegatedKeyDown(BradixDelegatedKeyboardEvent args);
}
