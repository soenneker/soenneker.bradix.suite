using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixDismissableLayer"/>.
/// </summary>
public interface IBradixDismissableLayer : IAsyncDisposable {
    /// <summary>
    /// Gets or sets whether pointer events outside the layer are disabled.
    /// </summary>
    bool DisableOutsidePointerEvents { get; set; }

    /// <summary>
    /// Gets or sets whether the escape key dismisses the layer.
    /// </summary>
    bool DismissOnEscapeKeyDown { get; set; }

    /// <summary>
    /// Gets or sets whether a pointer down outside dismisses the layer.
    /// </summary>
    bool DismissOnPointerDownOutside { get; set; }

    /// <summary>
    /// Gets or sets whether focus moving outside dismisses the layer.
    /// </summary>
    bool DismissOnFocusOutside { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the escape key is pressed.
    /// </summary>
    EventCallback OnEscapeKeyDown { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the escape key is pressed, including detailed arguments.
    /// </summary>
    EventCallback<BradixEscapeKeyDownEventArgs> OnEscapeKeyDownDetailed { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when a pointer is pressed outside the layer.
    /// </summary>
    EventCallback OnPointerDownOutside { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when a pointer is pressed outside the layer, including detailed arguments.
    /// </summary>
    EventCallback<BradixPointerDownOutsideEventArgs> OnPointerDownOutsideDetailed { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when focus moves outside the layer.
    /// </summary>
    EventCallback OnFocusOutside { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when focus moves outside the layer, including detailed arguments.
    /// </summary>
    EventCallback<BradixFocusOutsideEventArgs> OnFocusOutsideDetailed { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when interaction occurs outside the layer.
    /// </summary>
    EventCallback OnInteractOutside { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when interaction occurs outside the layer, including detailed arguments.
    /// </summary>
    EventCallback<BradixInteractOutsideEventArgs> OnInteractOutsideDetailed { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the layer is dismissed.
    /// </summary>
    EventCallback OnDismiss { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the layer root element reference is available.
    /// </summary>
    EventCallback<ElementReference> OnElementReferenceCaptured { get; set; }


    /// <summary>
    /// Handles an escape key event forwarded from JavaScript.
    /// </summary>
    /// <param name="originalEvent">Optional original keyboard event payload.</param>
    /// <returns><c>true</c> if the layer was dismissed; otherwise <c>false</c>.</returns>
    Task<bool> HandleEscapeKeyDown(BradixDelegatedKeyboardEvent? originalEvent = null);

    /// <summary>
    /// Handles a pointer down outside event forwarded from JavaScript.
    /// </summary>
    /// <param name="originalEvent">Optional original mouse event payload.</param>
    Task HandlePointerDownOutside(BradixDelegatedMouseEvent? originalEvent = null);

    /// <summary>
    /// Handles a focus outside event forwarded from JavaScript.
    /// </summary>
    /// <param name="originalEvent">Optional original focus event payload.</param>
    Task HandleFocusOutside(BradixDelegatedFocusEvent? originalEvent = null);
}
