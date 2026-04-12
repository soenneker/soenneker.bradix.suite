using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAlertDialogDescription"/>.
/// </summary>
public interface IBradixAlertDialogDescription
{
    /// <summary>
    /// Gets or sets the element id for the description.
    /// </summary>
    string? Id { get; set; }

    /// <summary>
    /// Gets or sets the CSS class for the description.
    /// </summary>
    string? Class { get; set; }

    /// <summary>
    /// Gets or sets the inline style for the description.
    /// </summary>
    string? Style { get; set; }

    /// <summary>
    /// Gets or sets additional attributes applied to the description.
    /// </summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the child content of the description.
    /// </summary>
    RenderFragment? ChildContent { get; set; }
}
