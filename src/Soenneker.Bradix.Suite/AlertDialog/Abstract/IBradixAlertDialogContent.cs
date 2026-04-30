using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAlertDialogContent"/>.
/// </summary>
public interface IBradixAlertDialogContent
{
    /// <summary>
    /// Gets or sets whether the content stays mounted while closed.
    /// </summary>
    bool ForceMount { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the dialog requests initial focus.
    /// </summary>
    EventCallback OnOpenAutoFocus { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the dialog requests initial focus, including detailed arguments.
    /// </summary>
    EventCallback<BradixAutoFocusEventArgs> OnOpenAutoFocusDetailed { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when focus should return after the dialog closes.
    /// </summary>
    EventCallback OnCloseAutoFocus { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when focus should return after the dialog closes, including detailed arguments.
    /// </summary>
    EventCallback<BradixAutoFocusEventArgs> OnCloseAutoFocusDetailed { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the escape key is pressed.
    /// </summary>
    EventCallback OnEscapeKeyDown { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the escape key is pressed, including detailed arguments.
    /// </summary>
    EventCallback<BradixEscapeKeyDownEventArgs> OnEscapeKeyDownDetailed { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when focus moves outside the dialog.
    /// </summary>
    EventCallback OnFocusOutside { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when focus moves outside the dialog, including detailed arguments.
    /// </summary>
    EventCallback<BradixFocusOutsideEventArgs> OnFocusOutsideDetailed { get; set; }
}
