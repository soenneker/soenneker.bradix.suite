using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the delegated interaction interop contract.
/// </summary>
public interface IDelegatedInteractionInterop : IAsyncDisposable
{
    /// <summary>
    /// Initializes the Delegated Interaction so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers delegated Interaction for the Delegated Interaction.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="options">Options to configure for the Delegated Interaction.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the delegated interaction registration is complete.</returns>
    ValueTask RegisterDelegatedInteraction(ElementReference element, object dotNetReference, object options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters delegated Interaction for the Delegated Interaction.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the delegated interaction registration has been removed.</returns>
    ValueTask UnregisterDelegatedInteraction(ElementReference element, CancellationToken cancellationToken = default);
}
