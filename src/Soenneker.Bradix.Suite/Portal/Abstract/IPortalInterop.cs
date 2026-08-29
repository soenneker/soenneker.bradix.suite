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
    /// Initializes the Portal so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Mounts portal.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="containerSelector">Container Selector for the mount portal operation.</param>
    /// <param name="container">Element that will contain the rendered component.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the mount portal operation is complete.</returns>
    ValueTask MountPortal(ElementReference element, string? containerSelector = null, ElementReference container = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unmounts portal.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the unmount portal operation is complete.</returns>
    ValueTask UnmountPortal(ElementReference element, CancellationToken cancellationToken = default);
}
