using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the scroll area interop contract.
/// </summary>
public interface IScrollAreaInterop : IAsyncDisposable
{
    /// <summary>
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register scroll area root operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterScrollAreaRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister scroll area root operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterScrollAreaRoot(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register scroll area viewport operation.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="content">The content.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterScrollAreaViewport(ElementReference viewport, ElementReference content, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister scroll area viewport operation.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterScrollAreaViewport(ElementReference viewport, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register scroll area scrollbar operation.
    /// </summary>
    /// <param name="scrollbar">The scrollbar.</param>
    /// <param name="thumb">The thumb.</param>
    /// <param name="viewport">The viewport.</param>
    /// <param name="orientation">The orientation.</param>
    /// <param name="dir">The dir.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterScrollAreaScrollbar(ElementReference scrollbar, ElementReference thumb, ElementReference viewport, string orientation, string dir, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister scroll area scrollbar operation.
    /// </summary>
    /// <param name="scrollbar">The scrollbar.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterScrollAreaScrollbar(ElementReference scrollbar, CancellationToken cancellationToken = default);
}