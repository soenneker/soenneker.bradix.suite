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
    /// Initializes the Tooltip so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers tooltip Trigger for the Tooltip.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the tooltip trigger registration is complete.</returns>
    ValueTask RegisterTooltipTrigger(ElementReference element, object dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters tooltip Trigger for the Tooltip.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the tooltip trigger registration has been removed.</returns>
    ValueTask UnregisterTooltipTrigger(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers tooltip Content.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="trigger">Pointer event that initiated the interaction.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="contentId">Identifier of the content to target.</param>
    /// <param name="hoverableContent">Whether hoverable content.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the tooltip content registration is complete.</returns>
    ValueTask RegisterTooltipContent(ElementReference content, ElementReference trigger, object dotNetReference, string contentId,
        bool hoverableContent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters tooltip Content for the Tooltip.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the tooltip content registration has been removed.</returns>
    ValueTask UnregisterTooltipContent(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches tooltip Open.
    /// </summary>
    /// <param name="contentId">Identifier of the content to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the dispatch tooltip open operation is complete.</returns>
    ValueTask DispatchTooltipOpen(string contentId, CancellationToken cancellationToken = default);
}
