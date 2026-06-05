using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the presence overlay interop contract.
/// </summary>
public interface IPresenceOverlayInterop : IAsyncDisposable
{
    /// <summary>
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register presence operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterPresence(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets presence state.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<BradixPresenceSnapshot> GetPresenceState(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister presence operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterPresence(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register focus guards operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterFocusGuards(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister focus guards operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterFocusGuards(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register hide others operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterHideOthers(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister hide others operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterHideOthers(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register remove scroll operation.
    /// </summary>
    /// <param name="allowPinchZoom">The allow pinch zoom.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterRemoveScroll(bool allowPinchZoom = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register remove scroll operation.
    /// </summary>
    /// <param name="registrationId">The registration id.</param>
    /// <param name="allowPinchZoom">The allow pinch zoom.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterRemoveScroll(string registrationId, bool allowPinchZoom = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister remove scroll operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterRemoveScroll(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister remove scroll operation.
    /// </summary>
    /// <param name="registrationId">The registration id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterRemoveScroll(string registrationId, CancellationToken cancellationToken = default);
}
