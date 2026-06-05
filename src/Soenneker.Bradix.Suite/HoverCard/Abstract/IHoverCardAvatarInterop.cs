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
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the disable hover card content tab navigation operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask DisableHoverCardContentTabNavigation(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register hover card selection containment operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterHoverCardSelectionContainment(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the begin hover card selection containment operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask BeginHoverCardSelectionContainment(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister hover card selection containment operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterHoverCardSelectionContainment(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register avatar image loading status operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <param name="crossOrigin">The cross origin.</param>
    /// <param name="referrerPolicy">The referrer policy.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterAvatarImageLoadingStatus(string? src, string? crossOrigin, string? referrerPolicy,
        DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister avatar image loading status operation.
    /// </summary>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterAvatarImageLoadingStatus(DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);
}