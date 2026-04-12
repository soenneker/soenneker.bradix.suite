using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAlertDialogAction"/>.
/// </summary>
public interface IBradixAlertDialogAction
{
    /// <summary>
    /// Gets or sets the CSS class for the action control.
    /// </summary>
    string? Class { get; set; }

    /// <summary>
    /// Gets or sets the inline style for the action control.
    /// </summary>
    string? Style { get; set; }

    /// <summary>
    /// Gets or sets additional attributes applied to the action control.
    /// </summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the child content of the action control.
    /// </summary>
    RenderFragment? ChildContent { get; set; }
}
