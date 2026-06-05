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
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register select item aligned position operation.
    /// </summary>
    /// <param name="wrapper">The wrapper.</param>
    /// <param name="content">The content.</param>
    /// <param name="viewport">The viewport.</param>
    /// <param name="trigger">The trigger.</param>
    /// <param name="valueNode">The value node.</param>
    /// <param name="selectedItem">The selected item.</param>
    /// <param name="selectedItemText">The selected item text.</param>
    /// <param name="dir">The dir.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterSelectItemAlignedPosition(ElementReference wrapper, ElementReference content, ElementReference viewport, ElementReference trigger,
        ElementReference valueNode, ElementReference selectedItem, ElementReference selectedItemText, string dir, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates select item aligned position.
    /// </summary>
    /// <param name="wrapper">The wrapper.</param>
    /// <param name="content">The content.</param>
    /// <param name="viewport">The viewport.</param>
    /// <param name="trigger">The trigger.</param>
    /// <param name="valueNode">The value node.</param>
    /// <param name="selectedItem">The selected item.</param>
    /// <param name="selectedItemText">The selected item text.</param>
    /// <param name="dir">The dir.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateSelectItemAlignedPosition(ElementReference wrapper, ElementReference content, ElementReference viewport, ElementReference trigger,
        ElementReference valueNode, ElementReference selectedItem, ElementReference selectedItemText, string dir, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister select item aligned position operation.
    /// </summary>
    /// <param name="wrapper">The wrapper.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterSelectItemAlignedPosition(ElementReference wrapper, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register select viewport operation.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="content">The content.</param>
    /// <param name="wrapper">The wrapper.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterSelectViewport(ElementReference viewport, ElementReference content, ElementReference wrapper,
        DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister select viewport operation.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterSelectViewport(ElementReference viewport, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register select content keyboard operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterSelectContentKeyboard(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister select content keyboard operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterSelectContentKeyboard(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the scroll select viewport by item operation.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="item">The item.</param>
    /// <param name="upward">The upward.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ScrollSelectViewportByItem(ElementReference viewport, ElementReference item, bool upward, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register select content pointer tracker operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="pageX">The page x.</param>
    /// <param name="pageY">The page y.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterSelectContentPointerTracker(ElementReference content, DotNetObjectReference<object> dotNetReference, double pageX, double pageY,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister select content pointer tracker operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterSelectContentPointerTracker(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register select window dismiss operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterSelectWindowDismiss(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister select window dismiss operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterSelectWindowDismiss(ElementReference content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets select option value at point.
    /// </summary>
    /// <param name="clientX">The client x.</param>
    /// <param name="clientY">The client y.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<string?> GetSelectOptionValueAtPoint(double clientX, double clientY, CancellationToken cancellationToken = default);
}
