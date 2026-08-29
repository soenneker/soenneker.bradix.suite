using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the collapsible interop contract.
/// </summary>
public interface ICollapsibleInterop : IAsyncDisposable
{
    /// <summary>
    /// Initializes the Collapsible so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Observes collapsible Content.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the observe collapsible content operation is complete.</returns>
    ValueTask ObserveCollapsibleContent(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops observing collapsible Content.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the unobserve collapsible content operation is complete.</returns>
    ValueTask UnobserveCollapsibleContent(ElementReference element, CancellationToken cancellationToken = default);
}
