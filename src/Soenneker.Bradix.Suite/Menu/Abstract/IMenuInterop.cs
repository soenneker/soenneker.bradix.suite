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
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the begin menu submenu pointer grace operation.
    /// </summary>
    /// <param name="trigger">The trigger.</param>
    /// <param name="content">The content.</param>
    /// <param name="clientX">The client x.</param>
    /// <param name="clientY">The client y.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<bool> BeginMenuSubmenuPointerGrace(ElementReference trigger, ElementReference content, double clientX, double clientY,
        DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the cancel menu submenu pointer grace operation.
    /// </summary>
    /// <param name="trigger">The trigger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask CancelMenuSubmenuPointerGrace(ElementReference trigger, CancellationToken cancellationToken = default);
}