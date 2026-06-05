using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the menubar interop contract.
/// </summary>
public interface IMenubarInterop : IAsyncDisposable
{
    /// <summary>
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register menubar document dismiss operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="menubarId">The menubar id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterMenubarDocumentDismiss(ElementReference element, object dotNetReference, string menubarId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister menubar document dismiss operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterMenubarDocumentDismiss(ElementReference element, CancellationToken cancellationToken = default);
}