using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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
    /// Gets or sets whether the trigger is disabled.
    /// </summary>
    bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the trigger is clicked.
    /// </summary>
    EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Gets or sets additional attributes applied to the trigger.
    /// </summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the child content of the trigger.
    /// </summary>
    RenderFragment? ChildContent { get; set; }
}
