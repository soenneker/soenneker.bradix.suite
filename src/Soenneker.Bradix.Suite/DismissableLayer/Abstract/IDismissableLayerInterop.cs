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
    /// Initializes the Dismissable Layer so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers dismissable Layer.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="disableOutsidePointerEvents">Whether disable outside pointer events.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the dismissable layer registration is complete.</returns>
    ValueTask RegisterDismissableLayer(ElementReference element, object dotNetReference, bool disableOutsidePointerEvents, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates dismissable layer.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="disableOutsidePointerEvents">Whether disable outside pointer events.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the dismissable layer update is complete.</returns>
    ValueTask UpdateDismissableLayer(ElementReference element, bool disableOutsidePointerEvents, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters dismissable Layer for the Dismissable Layer.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the dismissable layer registration has been removed.</returns>
    ValueTask UnregisterDismissableLayer(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers dismissable Layer Branch for the Dismissable Layer.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the dismissable layer branch registration is complete.</returns>
    ValueTask RegisterDismissableLayerBranch(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters dismissable Layer Branch for the Dismissable Layer.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the dismissable layer branch registration has been removed.</returns>
    ValueTask UnregisterDismissableLayerBranch(ElementReference element, CancellationToken cancellationToken = default);
}
