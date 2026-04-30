using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAlertDialogAction"/>.
/// </summary>
public interface IBradixAlertDialogAction
{
    /// <summary>
    /// Gets or sets whether the action control is disabled.
    /// </summary>
    bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the action control is clicked.
    /// </summary>
    EventCallback<MouseEventArgs> OnClick { get; set; }
}
