using System;
using System.Threading.Tasks;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixDialogTrigger"/>.
/// </summary>
public interface IBradixDialogTrigger : IAsyncDisposable {
    /// <summary>
    /// Gets or sets whether the trigger is disabled.
    /// </summary>
    bool Disabled { get; set; }


    /// <summary>
    /// Called when delegated interaction handling is ready on the trigger.
    /// </summary>
    Task HandleDelegatedInteractionReady();

    /// <summary>
    /// Handles a delegated click routed from JavaScript.
    /// </summary>
    /// <param name="mouseEvent">The delegated mouse event payload.</param>
    Task HandleDelegatedClick(BradixDelegatedMouseEvent mouseEvent);
}
