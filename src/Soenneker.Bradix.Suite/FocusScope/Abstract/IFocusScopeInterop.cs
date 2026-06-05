using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the focus scope interop contract.
/// </summary>
public interface IFocusScopeInterop : IAsyncDisposable
{
    /// <summary>
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register focus scope operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="loop">The loop.</param>
    /// <param name="trapped">The trapped.</param>
    /// <param name="preventMountAutoFocus">The prevent mount auto focus.</param>
    /// <param name="preventUnmountAutoFocus">The prevent unmount auto focus.</param>
    /// <param name="invokeMountAutoFocus">The invoke mount auto focus.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterFocusScope(ElementReference element, DotNetObjectReference<object> dotNetReference, bool loop, bool trapped,
        bool preventMountAutoFocus, bool preventUnmountAutoFocus, bool invokeMountAutoFocus, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates focus scope.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="loop">The loop.</param>
    /// <param name="trapped">The trapped.</param>
    /// <param name="preventMountAutoFocus">The prevent mount auto focus.</param>
    /// <param name="preventUnmountAutoFocus">The prevent unmount auto focus.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateFocusScope(ElementReference element, bool loop, bool trapped, bool preventMountAutoFocus, bool preventUnmountAutoFocus,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister focus scope operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="unmountAutoFocusPrevented">The unmount auto focus prevented.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterFocusScope(ElementReference element, bool unmountAutoFocusPrevented = false, CancellationToken cancellationToken = default);
}
