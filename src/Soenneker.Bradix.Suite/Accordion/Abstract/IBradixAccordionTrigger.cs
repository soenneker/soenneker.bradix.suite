using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAccordionTrigger"/>.
/// </summary>
public interface IBradixAccordionTrigger
{
    /// <summary>
    /// Gets or sets the callback invoked when a key is pressed on the trigger.
    /// </summary>
    EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

    /// <summary>
    /// Releases resources used by the trigger registration.
    /// </summary>
    void Dispose();

    /// <summary>
    /// Releases resources used by the trigger registration asynchronously.
    /// </summary>
    ValueTask DisposeAsync();

    /// <summary>
    /// Called when delegated keyboard handling is ready on the host element.
    /// </summary>
    Task HandleDelegatedInteractionReadyAsync();

    /// <summary>
    /// Handles a delegated keydown event forwarded from JavaScript.
    /// </summary>
    /// <param name="args">The delegated keyboard event payload.</param>
    Task HandleDelegatedKeyDownAsync(BradixDelegatedKeyboardEvent args);
}
