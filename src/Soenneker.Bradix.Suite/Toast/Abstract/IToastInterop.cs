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
    /// Initializes the Toast so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers toast Viewport.
    /// </summary>
    /// <param name="wrapper">Wrapper instance to initialize or invoke.</param>
    /// <param name="viewport">Viewport element or dimensions to use.</param>
    /// <param name="headProxy">Head Proxy for the register toast viewport operation.</param>
    /// <param name="tailProxy">Tail Proxy for the register toast viewport operation.</param>
    /// <param name="hotkey">hotkey to process.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the toast viewport registration is complete.</returns>
    ValueTask RegisterToastViewport(ElementReference wrapper, ElementReference viewport, ElementReference headProxy, ElementReference tailProxy,
        IReadOnlyList<string> hotkey, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters toast Viewport for the Toast.
    /// </summary>
    /// <param name="viewport">Viewport element or dimensions to use.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the toast viewport registration has been removed.</returns>
    ValueTask UnregisterToastViewport(ElementReference viewport, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the Toast toast Focused.
    /// </summary>
    /// <param name="toast">Toast for the is toast focused operation.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>true if the Toast toast Focused; otherwise, false.</returns>
    ValueTask<bool> IsToastFocused(ElementReference toast, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets toast announce text.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested string[].</returns>
    ValueTask<string[]> GetToastAnnounceText(ElementReference element, CancellationToken cancellationToken = default);
}
