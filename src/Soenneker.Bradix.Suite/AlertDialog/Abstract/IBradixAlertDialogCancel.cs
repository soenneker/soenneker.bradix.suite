using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAlertDialogCancel"/>.
/// </summary>
public interface IBradixAlertDialogCancel
{
    /// <summary>
    /// Gets or sets the CSS class for the cancel control.
    /// </summary>
    string? Class { get; set; }

    /// <summary>
    /// Gets or sets the inline style for the cancel control.
    /// </summary>
    string? Style { get; set; }

    /// <summary>
    /// Gets or sets whether the cancel control is disabled.
    /// </summary>
    bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the cancel control is clicked.
    /// </summary>
    EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Gets or sets additional attributes applied to the cancel control.
    /// </summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the child content of the cancel control.
    /// </summary>
    RenderFragment? ChildContent { get; set; }
}
