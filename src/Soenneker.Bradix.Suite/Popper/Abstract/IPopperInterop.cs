using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the popper interop contract.
/// </summary>
public interface IPopperInterop : IAsyncDisposable
{
    /// <summary>
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register popper content operation.
    /// </summary>
    /// <param name="anchor">The anchor.</param>
    /// <param name="content">The content.</param>
    /// <param name="arrow">The arrow.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterPopperContent(ElementReference anchor, ElementReference content, ElementReference arrow, DotNetObjectReference<object> dotNetReference, object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register popper content by selector operation.
    /// </summary>
    /// <param name="anchorSelector">The anchor selector.</param>
    /// <param name="content">The content.</param>
    /// <param name="arrow">The arrow.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterPopperContentBySelector(string anchorSelector, ElementReference content, ElementReference arrow,
        DotNetObjectReference<object> dotNetReference, object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register virtual popper content operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="arrow">The arrow.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterVirtualPopperContent(ElementReference content, ElementReference arrow, DotNetObjectReference<object> dotNetReference, double x, double y, object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates popper content.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="arrow">The arrow.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdatePopperContent(ElementReference content, ElementReference arrow, object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates virtual popper content.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="arrow">The arrow.</param>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateVirtualPopperContent(ElementReference content, ElementReference arrow, double x, double y, object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister popper content operation.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterPopperContent(ElementReference content, CancellationToken cancellationToken = default);
}
