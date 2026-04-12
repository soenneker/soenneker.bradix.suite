using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixCheckbox"/>.
/// </summary>
public interface IBradixCheckbox : IAsyncDisposable {
    /// <summary>
    /// Gets or sets the controlled checked state.
    /// </summary>
    BradixCheckboxCheckedState? Checked { get; set; }

    /// <summary>
    /// Gets or sets the initial checked state when uncontrolled.
    /// </summary>
    BradixCheckboxCheckedState DefaultChecked { get; set; }

    /// <summary>
    /// Gets or sets whether the checkbox is required in a form.
    /// </summary>
    bool Required { get; set; }

    /// <summary>
    /// Gets or sets whether the checkbox is disabled.
    /// </summary>
    bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the form control name.
    /// </summary>
    string? Name { get; set; }

    /// <summary>
    /// Gets or sets the form owner id.
    /// </summary>
    string? Form { get; set; }

    /// <summary>
    /// Gets or sets the submitted value when checked.
    /// </summary>
    string Value { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the controlled checked state changes.
    /// </summary>
    EventCallback<BradixCheckboxCheckedState> CheckedChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the checked state changes.
    /// </summary>
    EventCallback<BradixCheckboxCheckedState> OnCheckedChange { get; set; }


    /// <summary>
    /// Called when delegated interaction handling is ready on the root button.
    /// </summary>
    Task HandleDelegatedInteractionReady();

    /// <summary>
    /// Resets the checkbox to its initial unchecked state when the owning form resets.
    /// </summary>
    Task HandleFormReset();

    /// <summary>
    /// Handles a delegated click routed from JavaScript.
    /// </summary>
    /// <param name="mouseEvent">The delegated mouse event payload.</param>
    Task HandleDelegatedClick(BradixDelegatedMouseEvent mouseEvent);
}
