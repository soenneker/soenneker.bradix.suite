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
    /// Initializes the Scroll Area so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers scroll Area Root for the Scroll Area.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the scroll area root registration is complete.</returns>
    ValueTask RegisterScrollAreaRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters scroll Area Root for the Scroll Area.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the scroll area root registration has been removed.</returns>
    ValueTask UnregisterScrollAreaRoot(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers scroll Area Viewport for the Scroll Area.
    /// </summary>
    /// <param name="viewport">Viewport element or dimensions to use.</param>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the scroll area viewport registration is complete.</returns>
    ValueTask RegisterScrollAreaViewport(ElementReference viewport, ElementReference content, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters scroll Area Viewport for the Scroll Area.
    /// </summary>
    /// <param name="viewport">Viewport element or dimensions to use.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the scroll area viewport registration has been removed.</returns>
    ValueTask UnregisterScrollAreaViewport(ElementReference viewport, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers scroll Area Scrollbar.
    /// </summary>
    /// <param name="scrollbar">Scrollbar for the register scroll area scrollbar operation.</param>
    /// <param name="thumb">Thumb for the register scroll area scrollbar operation.</param>
    /// <param name="viewport">Viewport element or dimensions to use.</param>
    /// <param name="orientation">Layout orientation to apply.</param>
    /// <param name="dir">Dir for the register scroll area scrollbar operation.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the scroll area scrollbar registration is complete.</returns>
    ValueTask RegisterScrollAreaScrollbar(ElementReference scrollbar, ElementReference thumb, ElementReference viewport, string orientation, string dir, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters scroll Area Scrollbar.
    /// </summary>
    /// <param name="scrollbar">Scrollbar for the unregister scroll area scrollbar operation.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the scroll area scrollbar registration has been removed.</returns>
    ValueTask UnregisterScrollAreaScrollbar(ElementReference scrollbar, CancellationToken cancellationToken = default);
}
