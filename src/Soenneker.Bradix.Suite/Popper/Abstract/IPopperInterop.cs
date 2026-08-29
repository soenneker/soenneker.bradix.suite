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
    /// Initializes the Popper so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers popper Content.
    /// </summary>
    /// <param name="anchor">Anchor for the register popper content operation.</param>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="arrow">Arrow element positioned with the floating content.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="options">Options to configure for the Popper.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the popper content registration is complete.</returns>
    ValueTask RegisterPopperContent(ElementReference anchor, ElementReference content, ElementReference arrow, DotNetObjectReference<object> dotNetReference, object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers popper Content By Selector.
    /// </summary>
    /// <param name="anchorSelector">Anchor Selector for the register popper content by selector operation.</param>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="arrow">Arrow element positioned with the floating content.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="options">Options to configure for the Popper.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the popper content by selector registration is complete.</returns>
    ValueTask RegisterPopperContentBySelector(string anchorSelector, ElementReference content, ElementReference arrow,
        DotNetObjectReference<object> dotNetReference, object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers virtual Popper Content.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="arrow">Arrow element positioned with the floating content.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="x">Operand passed to the accumulator function.</param>
    /// <param name="y">Vertical coordinate to apply.</param>
    /// <param name="options">Options to configure for the Popper.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the virtual popper content registration is complete.</returns>
    ValueTask RegisterVirtualPopperContent(ElementReference content, ElementReference arrow, DotNetObjectReference<object> dotNetReference, double x, double y, object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates popper content.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="arrow">Arrow element positioned with the floating content.</param>
    /// <param name="options">Options to configure for the Popper.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the popper content update is complete.</returns>
    ValueTask UpdatePopperContent(ElementReference content, ElementReference arrow, object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates virtual popper content.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="arrow">Arrow element positioned with the floating content.</param>
    /// <param name="x">Operand passed to the accumulator function.</param>
    /// <param name="y">Vertical coordinate to apply.</param>
    /// <param name="options">Options to configure for the Popper.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the virtual popper content update is complete.</returns>
    ValueTask UpdateVirtualPopperContent(ElementReference content, ElementReference arrow, double x, double y, object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters popper Content for the Popper.
    /// </summary>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the popper content registration has been removed.</returns>
    ValueTask UnregisterPopperContent(ElementReference content, CancellationToken cancellationToken = default);
}
