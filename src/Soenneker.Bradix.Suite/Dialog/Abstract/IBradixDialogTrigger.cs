using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixDialogTrigger"/>.
/// </summary>
public interface IBradixDialogTrigger
{
    /// <summary>
    /// Gets or sets whether the trigger is disabled.
    /// </summary>
    bool Disabled { get; set; }

    /// <summary>
    /// Releases resources used by delegated interaction registration.
    /// </summary>
    ValueTask DisposeAsync();

    /// <summary>
    /// Called when delegated interaction handling is ready on the trigger.
    /// </summary>
    Task HandleDelegatedInteractionReadyAsync();

    /// <summary>
    /// Handles a delegated click routed from JavaScript.
    /// </summary>
    /// <param name="mouseEvent">The delegated mouse event payload.</param>
    Task HandleDelegatedClickAsync(BradixDelegatedMouseEvent mouseEvent);
}
