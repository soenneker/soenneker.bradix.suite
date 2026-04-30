using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAlertDialogTrigger"/>.
/// </summary>
public interface IBradixAlertDialogTrigger
{
    /// <summary>
    /// Gets or sets whether the trigger is disabled.
    /// </summary>
    bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the trigger is clicked.
    /// </summary>
    EventCallback<MouseEventArgs> OnClick { get; set; }
}
