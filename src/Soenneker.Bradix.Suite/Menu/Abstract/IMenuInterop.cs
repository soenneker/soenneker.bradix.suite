using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the menu interop contract.
/// </summary>
public interface IMenuInterop : IAsyncDisposable
{
    /// <summary>
    /// Initializes the Menu so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins menu Submenu Pointer Grace.
    /// </summary>
    /// <param name="trigger">Pointer event that initiated the interaction.</param>
    /// <param name="content">Content to render, store, or send.</param>
    /// <param name="clientX">client X used to communicate with the external service.</param>
    /// <param name="clientY">client Y used to communicate with the external service.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>true if retrieves begin menu submenu pointer grace from the Menu; otherwise, false.</returns>
    ValueTask<bool> BeginMenuSubmenuPointerGrace(ElementReference trigger, ElementReference content, double clientX, double clientY,
        DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels menu Submenu Pointer Grace.
    /// </summary>
    /// <param name="trigger">Pointer event that initiated the interaction.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the cancel menu submenu pointer grace operation is complete.</returns>
    ValueTask CancelMenuSubmenuPointerGrace(ElementReference trigger, CancellationToken cancellationToken = default);
}
