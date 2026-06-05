using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the dismissable layer interop contract.
/// </summary>
public interface IDismissableLayerInterop : IAsyncDisposable
{
    /// <summary>
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register dismissable layer operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="disableOutsidePointerEvents">The disable outside pointer events.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterDismissableLayer(ElementReference element, object dotNetReference, bool disableOutsidePointerEvents, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates dismissable layer.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="disableOutsidePointerEvents">The disable outside pointer events.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateDismissableLayer(ElementReference element, bool disableOutsidePointerEvents, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister dismissable layer operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterDismissableLayer(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register dismissable layer branch operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterDismissableLayerBranch(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister dismissable layer branch operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterDismissableLayerBranch(ElementReference element, CancellationToken cancellationToken = default);
}