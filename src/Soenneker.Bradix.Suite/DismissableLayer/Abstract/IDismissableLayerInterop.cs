using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IDismissableLayerInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask RegisterDismissableLayer(ElementReference element, object dotNetReference, bool disableOutsidePointerEvents, CancellationToken cancellationToken = default);

    ValueTask UpdateDismissableLayer(ElementReference element, bool disableOutsidePointerEvents, CancellationToken cancellationToken = default);

    ValueTask UnregisterDismissableLayer(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterDismissableLayerBranch(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask UnregisterDismissableLayerBranch(ElementReference element, CancellationToken cancellationToken = default);
}