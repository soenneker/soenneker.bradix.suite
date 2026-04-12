using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAlertDialogOverlay"/>.
/// </summary>
public interface IBradixAlertDialogOverlay
{
    /// <summary>
    /// Gets or sets the CSS class for the overlay.
    /// </summary>
    string? Class { get; set; }

    /// <summary>
    /// Gets or sets the inline style for the overlay.
    /// </summary>
    string? Style { get; set; }

    /// <summary>
    /// Gets or sets whether the overlay stays mounted while closed.
    /// </summary>
    bool ForceMount { get; set; }

    /// <summary>
    /// Gets or sets additional attributes applied to the overlay.
    /// </summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the child content of the overlay.
    /// </summary>
    RenderFragment? ChildContent { get; set; }
}
