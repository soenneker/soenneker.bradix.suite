using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAlertDialogTitle"/>.
/// </summary>
public interface IBradixAlertDialogTitle
{
    /// <summary>
    /// Gets or sets the element id for the title.
    /// </summary>
    string? Id { get; set; }

    /// <summary>
    /// Gets or sets the CSS class for the title.
    /// </summary>
    string? Class { get; set; }

    /// <summary>
    /// Gets or sets the inline style for the title.
    /// </summary>
    string? Style { get; set; }

    /// <summary>
    /// Gets or sets additional attributes applied to the title.
    /// </summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the child content of the title.
    /// </summary>
    RenderFragment? ChildContent { get; set; }
}
