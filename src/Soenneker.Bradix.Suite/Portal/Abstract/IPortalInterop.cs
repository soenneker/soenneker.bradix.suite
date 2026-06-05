using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the portal interop contract.
/// </summary>
public interface IPortalInterop : IAsyncDisposable
{
    /// <summary>
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the mount portal operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="containerSelector">The container selector.</param>
    /// <param name="container">The container.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask MountPortal(ElementReference element, string? containerSelector = null, ElementReference container = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unmount portal operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnmountPortal(ElementReference element, CancellationToken cancellationToken = default);
}