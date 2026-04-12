using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAlertDialogContent"/>.
/// </summary>
public interface IBradixAlertDialogContent
{
    /// <summary>
    /// Gets or sets the element id for the dialog content container.
    /// </summary>
    string? Id { get; set; }

    /// <summary>
    /// Gets or sets the CSS class for the dialog content.
    /// </summary>
    string? Class { get; set; }

    /// <summary>
    /// Gets or sets the inline style for the dialog content.
    /// </summary>
    string? Style { get; set; }

    /// <summary>
    /// Gets or sets whether the content stays mounted while closed.
    /// </summary>
    bool ForceMount { get; set; }

    /// <summary>
    /// Gets or sets additional attributes applied to the dialog content.
    /// </summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

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

    /// <summary>
    /// Gets or sets the child content of the dialog.
    /// </summary>
    RenderFragment? ChildContent { get; set; }
}
