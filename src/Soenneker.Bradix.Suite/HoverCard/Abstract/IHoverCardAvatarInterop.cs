using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the hover card avatar interop contract.
/// </summary>
public interface IHoverCardAvatarInterop : IAsyncDisposable
{
    /// <summary>
    /// Initializes the Hover Card Avatar so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables hover Card Content Tab Navigation.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the disable hover card content tab navigation operation is complete.</returns>
    ValueTask DisableHoverCardContentTabNavigation(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers hover Card Selection Containment for the Hover Card Avatar.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the hover card selection containment registration is complete.</returns>
    ValueTask RegisterHoverCardSelectionContainment(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins hover Card Selection Containment.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the begin hover card selection containment operation is complete.</returns>
    ValueTask BeginHoverCardSelectionContainment(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters hover Card Selection Containment for the Hover Card Avatar.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the hover card selection containment registration has been removed.</returns>
    ValueTask UnregisterHoverCardSelectionContainment(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers avatar Image Loading Status.
    /// </summary>
    /// <param name="src">Src for the register avatar image loading status operation.</param>
    /// <param name="crossOrigin">CORS mode assigned to the script element.</param>
    /// <param name="referrerPolicy">Referrer Policy for the register avatar image loading status operation.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the avatar image loading status registration is complete.</returns>
    ValueTask RegisterAvatarImageLoadingStatus(string? src, string? crossOrigin, string? referrerPolicy,
        DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters avatar Image Loading Status for the Hover Card Avatar.
    /// </summary>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the avatar image loading status registration has been removed.</returns>
    ValueTask UnregisterAvatarImageLoadingStatus(DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);
}
