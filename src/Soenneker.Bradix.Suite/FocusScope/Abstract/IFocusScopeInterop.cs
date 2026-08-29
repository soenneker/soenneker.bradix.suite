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
    /// Initializes the Focus Scope so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers focus Scope.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="loop">Whether loop.</param>
    /// <param name="trapped">Whether trapped.</param>
    /// <param name="preventMountAutoFocus">Whether prevent mount auto focus.</param>
    /// <param name="preventUnmountAutoFocus">Whether prevent unmount auto focus.</param>
    /// <param name="invokeMountAutoFocus">Whether invoke mount auto focus.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the focus scope registration is complete.</returns>
    ValueTask RegisterFocusScope(ElementReference element, DotNetObjectReference<object> dotNetReference, bool loop, bool trapped,
        bool preventMountAutoFocus, bool preventUnmountAutoFocus, bool invokeMountAutoFocus, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates focus scope.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="loop">Whether loop.</param>
    /// <param name="trapped">Whether trapped.</param>
    /// <param name="preventMountAutoFocus">Whether prevent mount auto focus.</param>
    /// <param name="preventUnmountAutoFocus">Whether prevent unmount auto focus.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the focus scope update is complete.</returns>
    ValueTask UpdateFocusScope(ElementReference element, bool loop, bool trapped, bool preventMountAutoFocus, bool preventUnmountAutoFocus,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters focus Scope.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="unmountAutoFocusPrevented">Whether unmount auto focus prevented.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the focus scope registration has been removed.</returns>
    ValueTask UnregisterFocusScope(ElementReference element, bool unmountAutoFocusPrevented = false, CancellationToken cancellationToken = default);
}
