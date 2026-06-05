using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the navigation menu interop contract.
/// </summary>
public interface INavigationMenuInterop : IAsyncDisposable
{
    /// <summary>
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register navigation menu trigger interaction operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterNavigationMenuTriggerInteraction(ElementReference element, object dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister navigation menu trigger interaction operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterNavigationMenuTriggerInteraction(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register navigation menu indicator operation.
    /// </summary>
    /// <param name="indicator">The indicator.</param>
    /// <param name="activeTrigger">The active trigger.</param>
    /// <param name="track">The track.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="orientation">The orientation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterNavigationMenuIndicator(ElementReference indicator, ElementReference activeTrigger, ElementReference track,
        object dotNetReference, string orientation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates navigation menu indicator.
    /// </summary>
    /// <param name="indicator">The indicator.</param>
    /// <param name="activeTrigger">The active trigger.</param>
    /// <param name="track">The track.</param>
    /// <param name="orientation">The orientation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateNavigationMenuIndicator(ElementReference indicator, ElementReference activeTrigger, ElementReference track,
        string orientation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister navigation menu indicator operation.
    /// </summary>
    /// <param name="indicator">The indicator.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterNavigationMenuIndicator(ElementReference indicator, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register navigation menu content focus bridge operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="trigger">The trigger.</param>
    /// <param name="startProxy">The start proxy.</param>
    /// <param name="endProxy">The end proxy.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterNavigationMenuContentFocusBridge(ElementReference content, ElementReference trigger, ElementReference startProxy,
        ElementReference endProxy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates navigation menu content focus bridge.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="trigger">The trigger.</param>
    /// <param name="startProxy">The start proxy.</param>
    /// <param name="endProxy">The end proxy.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateNavigationMenuContentFocusBridge(ElementReference content, ElementReference trigger, ElementReference startProxy,
        ElementReference endProxy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the focus navigation menu content operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<bool> FocusNavigationMenuContent(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister navigation menu content focus bridge operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterNavigationMenuContentFocusBridge(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register navigation menu viewport operation.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="content">The content.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterNavigationMenuViewport(ElementReference viewport, ElementReference content, object dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates navigation menu viewport.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="content">The content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateNavigationMenuViewport(ElementReference viewport, ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister navigation menu viewport operation.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterNavigationMenuViewport(ElementReference viewport, CancellationToken cancellationToken = default);
}
