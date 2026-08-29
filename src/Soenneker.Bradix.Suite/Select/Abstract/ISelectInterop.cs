using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the select interop contract.
/// </summary>
public interface ISelectInterop : IAsyncDisposable
{
    /// <summary>
    /// Initializes the Select so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers select Item Aligned Position.
    /// </summary>
    /// <param name="wrapper">Wrapper instance to initialize or invoke.</param>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="viewport">Viewport element or dimensions to use.</param>
    /// <param name="trigger">Pointer event that initiated the interaction.</param>
    /// <param name="valueNode">Value Node for the register select item aligned position operation.</param>
    /// <param name="selectedItem">Selected Item for the register select item aligned position operation.</param>
    /// <param name="selectedItemText">Selected Item Text for the register select item aligned position operation.</param>
    /// <param name="dir">Dir for the register select item aligned position operation.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the select item aligned position registration is complete.</returns>
    ValueTask RegisterSelectItemAlignedPosition(ElementReference wrapper, ElementReference content, ElementReference viewport, ElementReference trigger,
        ElementReference valueNode, ElementReference selectedItem, ElementReference selectedItemText, string dir, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates select item aligned position.
    /// </summary>
    /// <param name="wrapper">Wrapper instance to initialize or invoke.</param>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="viewport">Viewport element or dimensions to use.</param>
    /// <param name="trigger">Pointer event that initiated the interaction.</param>
    /// <param name="valueNode">Value Node for the update select item aligned position operation.</param>
    /// <param name="selectedItem">Selected Item for the update select item aligned position operation.</param>
    /// <param name="selectedItemText">Selected Item Text for the update select item aligned position operation.</param>
    /// <param name="dir">Dir for the update select item aligned position operation.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the select item aligned position update is complete.</returns>
    ValueTask UpdateSelectItemAlignedPosition(ElementReference wrapper, ElementReference content, ElementReference viewport, ElementReference trigger,
        ElementReference valueNode, ElementReference selectedItem, ElementReference selectedItemText, string dir, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters select Item Aligned Position for the Select.
    /// </summary>
    /// <param name="wrapper">Wrapper instance to initialize or invoke.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the select item aligned position registration has been removed.</returns>
    ValueTask UnregisterSelectItemAlignedPosition(ElementReference wrapper, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers select Viewport for the Select.
    /// </summary>
    /// <param name="viewport">Viewport element or dimensions to use.</param>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="wrapper">Wrapper instance to initialize or invoke.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the select viewport registration is complete.</returns>
    ValueTask RegisterSelectViewport(ElementReference viewport, ElementReference content, ElementReference wrapper,
        DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters select Viewport for the Select.
    /// </summary>
    /// <param name="viewport">Viewport element or dimensions to use.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the select viewport registration has been removed.</returns>
    ValueTask UnregisterSelectViewport(ElementReference viewport, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers select Content Keyboard for the Select.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the select content keyboard registration is complete.</returns>
    ValueTask RegisterSelectContentKeyboard(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters select Content Keyboard for the Select.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the select content keyboard registration has been removed.</returns>
    ValueTask UnregisterSelectContentKeyboard(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Scrolls select Viewport By Item.
    /// </summary>
    /// <param name="viewport">Viewport element or dimensions to use.</param>
    /// <param name="item">Receives the entry when the key is found.</param>
    /// <param name="upward">Whether upward.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the scroll select viewport by item operation is complete.</returns>
    ValueTask ScrollSelectViewportByItem(ElementReference viewport, ElementReference item, bool upward, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers select Content Pointer Tracker.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="pageX">Page X for the register select content pointer tracker operation.</param>
    /// <param name="pageY">Page Y for the register select content pointer tracker operation.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the select content pointer tracker registration is complete.</returns>
    ValueTask RegisterSelectContentPointerTracker(ElementReference content, DotNetObjectReference<object> dotNetReference, double pageX, double pageY,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters select Content Pointer Tracker for the Select.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the select content pointer tracker registration has been removed.</returns>
    ValueTask UnregisterSelectContentPointerTracker(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers select Window Dismiss for the Select.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the select window dismiss registration is complete.</returns>
    ValueTask RegisterSelectWindowDismiss(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters select Window Dismiss for the Select.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the select window dismiss registration has been removed.</returns>
    ValueTask UnregisterSelectWindowDismiss(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets select option value at point.
    /// </summary>
    /// <param name="clientX">client X used to communicate with the external service.</param>
    /// <param name="clientY">client Y used to communicate with the external service.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by get Select Option Value At Point.</returns>
    ValueTask<string?> GetSelectOptionValueAtPoint(double clientX, double clientY, CancellationToken cancellationToken = default);
}
