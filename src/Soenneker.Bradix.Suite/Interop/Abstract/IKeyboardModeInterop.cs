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
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the is keyboard interaction mode operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<bool> IsKeyboardInteractionMode(CancellationToken cancellationToken = default);
}