using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the radio group interop contract.
/// </summary>
public interface IRadioGroupInterop : IAsyncDisposable
{
    /// <summary>
    /// Initializes the Radio Group so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers radio Group Item Keys for the Radio Group.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the radio group item keys registration is complete.</returns>
    ValueTask RegisterRadioGroupItemKeys(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters radio Group Item Keys for the Radio Group.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the radio group item keys registration has been removed.</returns>
    ValueTask UnregisterRadioGroupItemKeys(ElementReference element, CancellationToken cancellationToken = default);
}
