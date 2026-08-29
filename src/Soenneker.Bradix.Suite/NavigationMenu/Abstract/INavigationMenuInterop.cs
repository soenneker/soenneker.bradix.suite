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
    /// Initializes the Navigation Menu so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers navigation Menu Trigger Interaction for the Navigation Menu.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the navigation menu trigger interaction registration is complete.</returns>
    ValueTask RegisterNavigationMenuTriggerInteraction(ElementReference element, object dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters navigation Menu Trigger Interaction for the Navigation Menu.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the navigation menu trigger interaction registration has been removed.</returns>
    ValueTask UnregisterNavigationMenuTriggerInteraction(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers navigation Menu Indicator.
    /// </summary>
    /// <param name="indicator">Indicator for the register navigation menu indicator operation.</param>
    /// <param name="activeTrigger">Active Trigger for the register navigation menu indicator operation.</param>
    /// <param name="track">Track for the register navigation menu indicator operation.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="orientation">Layout orientation to apply.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the navigation menu indicator registration is complete.</returns>
    ValueTask RegisterNavigationMenuIndicator(ElementReference indicator, ElementReference activeTrigger, ElementReference track,
        object dotNetReference, string orientation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates navigation menu indicator.
    /// </summary>
    /// <param name="indicator">Indicator for the update navigation menu indicator operation.</param>
    /// <param name="activeTrigger">Active Trigger for the update navigation menu indicator operation.</param>
    /// <param name="track">Track for the update navigation menu indicator operation.</param>
    /// <param name="orientation">Layout orientation to apply.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the navigation menu indicator update is complete.</returns>
    ValueTask UpdateNavigationMenuIndicator(ElementReference indicator, ElementReference activeTrigger, ElementReference track,
        string orientation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters navigation Menu Indicator.
    /// </summary>
    /// <param name="indicator">Indicator for the unregister navigation menu indicator operation.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the navigation menu indicator registration has been removed.</returns>
    ValueTask UnregisterNavigationMenuIndicator(ElementReference indicator, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers navigation Menu Content Focus Bridge.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="trigger">Pointer event that initiated the interaction.</param>
    /// <param name="startProxy">Start Proxy for the register navigation menu content focus bridge operation.</param>
    /// <param name="endProxy">End Proxy for the register navigation menu content focus bridge operation.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the navigation menu content focus bridge registration is complete.</returns>
    ValueTask RegisterNavigationMenuContentFocusBridge(ElementReference content, ElementReference trigger, ElementReference startProxy,
        ElementReference endProxy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates navigation menu content focus bridge.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="trigger">Pointer event that initiated the interaction.</param>
    /// <param name="startProxy">Start Proxy for the update navigation menu content focus bridge operation.</param>
    /// <param name="endProxy">End Proxy for the update navigation menu content focus bridge operation.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the navigation menu content focus bridge update is complete.</returns>
    ValueTask UpdateNavigationMenuContentFocusBridge(ElementReference content, ElementReference trigger, ElementReference startProxy,
        ElementReference endProxy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Focuses navigation Menu Content.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>true if focuses navigation Menu Content; otherwise, false.</returns>
    ValueTask<bool> FocusNavigationMenuContent(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters navigation Menu Content Focus Bridge for the Navigation Menu.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the navigation menu content focus bridge registration has been removed.</returns>
    ValueTask UnregisterNavigationMenuContentFocusBridge(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers navigation Menu Viewport for the Navigation Menu.
    /// </summary>
    /// <param name="viewport">Viewport element or dimensions to use.</param>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the navigation menu viewport registration is complete.</returns>
    ValueTask RegisterNavigationMenuViewport(ElementReference viewport, ElementReference content, object dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates navigation menu viewport.
    /// </summary>
    /// <param name="viewport">Viewport element or dimensions to use.</param>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the navigation menu viewport update is complete.</returns>
    ValueTask UpdateNavigationMenuViewport(ElementReference viewport, ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters navigation Menu Viewport for the Navigation Menu.
    /// </summary>
    /// <param name="viewport">Viewport element or dimensions to use.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the navigation menu viewport registration has been removed.</returns>
    ValueTask UnregisterNavigationMenuViewport(ElementReference viewport, CancellationToken cancellationToken = default);
}
