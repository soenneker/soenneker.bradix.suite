using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IDelegatedInteractionInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask RegisterDelegatedInteraction(ElementReference element, object dotNetReference, object options,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterDelegatedInteraction(ElementReference element, CancellationToken cancellationToken = default);
}