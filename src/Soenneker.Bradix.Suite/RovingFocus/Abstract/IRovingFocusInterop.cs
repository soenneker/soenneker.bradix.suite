using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the roving focus interop contract.
/// </summary>
public interface IRovingFocusInterop : IAsyncDisposable
{
    /// <summary>
    /// Initializes the Roving Focus so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers roving Focus Navigation Keys for the Roving Focus.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the roving focus navigation keys registration is complete.</returns>
    ValueTask RegisterRovingFocusNavigationKeys(ElementReference element, object? dotNetReference = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters roving Focus Navigation Keys for the Roving Focus.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the roving focus navigation keys registration has been removed.</returns>
    ValueTask UnregisterRovingFocusNavigationKeys(ElementReference element, CancellationToken cancellationToken = default);
}
