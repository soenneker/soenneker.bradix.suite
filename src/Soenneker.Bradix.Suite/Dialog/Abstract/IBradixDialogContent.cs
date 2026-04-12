using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixDialogContent"/>.
/// </summary>
public interface IBradixDialogContent
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
    /// Gets or sets the callback invoked when a pointer is pressed outside the dialog.
    /// </summary>
    EventCallback OnPointerDownOutside { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when a pointer is pressed outside the dialog, including detailed arguments.
    /// </summary>
    EventCallback<BradixPointerDownOutsideEventArgs> OnPointerDownOutsideDetailed { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when focus moves outside the dialog.
    /// </summary>
    EventCallback OnFocusOutside { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when focus moves outside the dialog, including detailed arguments.
    /// </summary>
    EventCallback<BradixFocusOutsideEventArgs> OnFocusOutsideDetailed { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when interaction occurs outside the dialog.
    /// </summary>
    EventCallback OnInteractOutside { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when interaction occurs outside the dialog, including detailed arguments.
    /// </summary>
    EventCallback<BradixInteractOutsideEventArgs> OnInteractOutsideDetailed { get; set; }

    /// <summary>
    /// Gets or sets the ARIA role applied to the dialog surface.
    /// </summary>
    string Role { get; set; }

    /// <summary>
    /// Gets or sets whether pressing Escape dismisses the dialog.
    /// </summary>
    bool CloseOnEscapeKeyDown { get; set; }

    /// <summary>
    /// Gets or sets whether a pointer down outside dismisses the dialog.
    /// </summary>
    bool CloseOnPointerDownOutside { get; set; }

    /// <summary>
    /// Gets or sets whether focus moving outside dismisses the dialog.
    /// </summary>
    bool CloseOnFocusOutside { get; set; }

    /// <summary>
    /// Releases resources used by the dialog content layer.
    /// </summary>
    ValueTask DisposeAsync();
}
