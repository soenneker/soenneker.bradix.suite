using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the label interop contract.
/// </summary>
public interface ILabelInterop : IAsyncDisposable
{
    /// <summary>
    /// Initializes the Label so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers label Text Selection Guard for the Label.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the label text selection guard registration is complete.</returns>
    ValueTask RegisterLabelTextSelectionGuard(ElementReference element, DotNetObjectReference<object>? dotNetReference = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters label Text Selection Guard for the Label.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the label text selection guard registration has been removed.</returns>
    ValueTask UnregisterLabelTextSelectionGuard(ElementReference element, CancellationToken cancellationToken = default);
}
