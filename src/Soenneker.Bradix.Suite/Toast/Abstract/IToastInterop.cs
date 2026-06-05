using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the toast interop contract.
/// </summary>
public interface IToastInterop : IAsyncDisposable
{
    /// <summary>
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register toast viewport operation.
    /// </summary>
    /// <param name="wrapper">The wrapper.</param>
    /// <param name="viewport">The viewport.</param>
    /// <param name="headProxy">The head proxy.</param>
    /// <param name="tailProxy">The tail proxy.</param>
    /// <param name="hotkey">The hotkey.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterToastViewport(ElementReference wrapper, ElementReference viewport, ElementReference headProxy, ElementReference tailProxy,
        IReadOnlyList<string> hotkey, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister toast viewport operation.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterToastViewport(ElementReference viewport, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the is toast focused operation.
    /// </summary>
    /// <param name="toast">The toast.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<bool> IsToastFocused(ElementReference toast, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets toast announce text.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<string[]> GetToastAnnounceText(ElementReference element, CancellationToken cancellationToken = default);
}