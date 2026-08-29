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
    /// Initializes the Dom so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets text content.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by get Text Content.</returns>
    ValueTask<string> GetTextContent(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets text content excluding.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="excludeSelector">Exclude Selector for the get text content excluding operation.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by get Text Content Excluding.</returns>
    ValueTask<string> GetTextContentExcluding(ElementReference element, string excludeSelector, CancellationToken cancellationToken = default);
}
