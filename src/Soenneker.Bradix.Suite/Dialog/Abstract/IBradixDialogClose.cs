using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixDialogClose"/>.
/// </summary>
public interface IBradixDialogClose
{
    /// <summary>
    /// Gets or sets the callback invoked when the close button element reference is available.
    /// </summary>
    EventCallback<ElementReference> OnElementReferenceCaptured { get; set; }

    /// <summary>
    /// Gets or sets the accessible label for the close button.
    /// </summary>
    string? AriaLabel { get; set; }

    /// <summary>
    /// Releases resources used by delegated interaction registration.
    /// </summary>
    ValueTask DisposeAsync();

    /// <summary>
    /// Called when delegated interaction handling is ready on the close button.
    /// </summary>
    Task HandleDelegatedInteractionReadyAsync();

    /// <summary>
    /// Handles a delegated click routed from JavaScript.
    /// </summary>
    /// <param name="mouseEvent">The delegated mouse event payload.</param>
    Task HandleDelegatedClickAsync(BradixDelegatedMouseEvent mouseEvent);
}
