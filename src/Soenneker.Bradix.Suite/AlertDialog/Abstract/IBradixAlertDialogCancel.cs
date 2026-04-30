using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAlertDialogCancel"/>.
/// </summary>
public interface IBradixAlertDialogCancel
{
    /// <summary>
    /// Gets or sets whether the cancel control is disabled.
    /// </summary>
    bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the cancel control is clicked.
    /// </summary>
    EventCallback<MouseEventArgs> OnClick { get; set; }
}
