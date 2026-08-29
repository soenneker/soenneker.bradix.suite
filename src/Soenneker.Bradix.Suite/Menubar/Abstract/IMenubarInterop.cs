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
    /// Initializes the Menubar so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers menubar Document Dismiss for the Menubar.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="menubarId">Identifier of the menubar to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the menubar document dismiss registration is complete.</returns>
    ValueTask RegisterMenubarDocumentDismiss(ElementReference element, object dotNetReference, string menubarId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters menubar Document Dismiss for the Menubar.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the menubar document dismiss registration has been removed.</returns>
    ValueTask UnregisterMenubarDocumentDismiss(ElementReference element, CancellationToken cancellationToken = default);
}
