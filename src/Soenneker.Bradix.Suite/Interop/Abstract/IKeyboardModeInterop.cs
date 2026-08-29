using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the keyboard mode interop contract.
/// </summary>
public interface IKeyboardModeInterop : IAsyncDisposable
{
    /// <summary>
    /// Initializes the Keyboard Mode so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the Keyboard Mode keyboard Interaction Mode.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>true if the Keyboard Mode keyboard Interaction Mode; otherwise, false.</returns>
    ValueTask<bool> IsKeyboardInteractionMode(CancellationToken cancellationToken = default);
}
