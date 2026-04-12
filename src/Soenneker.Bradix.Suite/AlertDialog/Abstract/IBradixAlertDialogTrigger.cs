using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAlertDialogTrigger"/>.
/// </summary>
public interface IBradixAlertDialogTrigger
{
    /// <summary>
    /// Gets or sets the CSS class for the trigger.
    /// </summary>
    string? Class { get; set; }

    /// <summary>
    /// Gets or sets the inline style for the trigger.
    /// </summary>
    string? Style { get; set; }

    /// <summary>
    /// Gets or sets additional attributes applied to the trigger.
    /// </summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the child content of the trigger.
    /// </summary>
    RenderFragment? ChildContent { get; set; }
}
