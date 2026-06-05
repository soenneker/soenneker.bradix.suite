using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the tooltip interop contract.
/// </summary>
public interface ITooltipInterop : IAsyncDisposable
{
    /// <summary>
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register tooltip trigger operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterTooltipTrigger(ElementReference element, object dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister tooltip trigger operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterTooltipTrigger(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register tooltip content operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="trigger">The trigger.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="contentId">The content id.</param>
    /// <param name="hoverableContent">The hoverable content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterTooltipContent(ElementReference content, ElementReference trigger, object dotNetReference, string contentId,
        bool hoverableContent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister tooltip content operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterTooltipContent(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the dispatch tooltip open operation.
    /// </summary>
    /// <param name="contentId">The content id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask DispatchTooltipOpen(string contentId, CancellationToken cancellationToken = default);
}