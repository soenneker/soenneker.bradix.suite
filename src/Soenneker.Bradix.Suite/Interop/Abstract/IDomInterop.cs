using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the dom interop contract.
/// </summary>
public interface IDomInterop : IAsyncDisposable
{
    /// <summary>
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets text content.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<string> GetTextContent(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets text content excluding.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="excludeSelector">The exclude selector.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<string> GetTextContentExcluding(ElementReference element, string excludeSelector, CancellationToken cancellationToken = default);
}
