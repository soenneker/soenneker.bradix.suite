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
    /// Initializes the Presence Overlay so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers presence for the Presence Overlay.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the presence registration is complete.</returns>
    ValueTask RegisterPresence(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets presence state.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested bradix Presence Snapshot.</returns>
    ValueTask<BradixPresenceSnapshot> GetPresenceState(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters presence for the Presence Overlay.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the presence registration has been removed.</returns>
    ValueTask UnregisterPresence(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers focus Guards for the Presence Overlay.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the focus guards registration is complete.</returns>
    ValueTask RegisterFocusGuards(CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters focus Guards for the Presence Overlay.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the focus guards registration has been removed.</returns>
    ValueTask UnregisterFocusGuards(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers hide Others for the Presence Overlay.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the hide others registration is complete.</returns>
    ValueTask RegisterHideOthers(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters hide Others for the Presence Overlay.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the hide others registration has been removed.</returns>
    ValueTask UnregisterHideOthers(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers remove Scroll.
    /// </summary>
    /// <param name="allowPinchZoom">Whether allow pinch zoom.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the remove scroll registration is complete.</returns>
    ValueTask RegisterRemoveScroll(bool allowPinchZoom = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers remove Scroll.
    /// </summary>
    /// <param name="registrationId">Identifier of the registration to target.</param>
    /// <param name="allowPinchZoom">Whether allow pinch zoom.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the remove scroll registration is complete.</returns>
    ValueTask RegisterRemoveScroll(string registrationId, bool allowPinchZoom = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters remove Scroll for the Presence Overlay.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the remove scroll registration has been removed.</returns>
    ValueTask UnregisterRemoveScroll(CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters remove Scroll for the Presence Overlay.
    /// </summary>
    /// <param name="registrationId">Identifier of the registration to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the remove scroll registration has been removed.</returns>
    ValueTask UnregisterRemoveScroll(string registrationId, CancellationToken cancellationToken = default);
}
