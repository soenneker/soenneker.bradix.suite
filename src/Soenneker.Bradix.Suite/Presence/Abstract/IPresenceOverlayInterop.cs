using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IPresenceOverlayInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask RegisterPresence(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask<BradixPresenceSnapshot> GetPresenceState(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask UnregisterPresence(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterFocusGuards(CancellationToken cancellationToken = default);

    ValueTask UnregisterFocusGuards(CancellationToken cancellationToken = default);

    ValueTask RegisterHideOthers(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask UnregisterHideOthers(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterRemoveScroll(bool allowPinchZoom = false, CancellationToken cancellationToken = default);

    ValueTask UnregisterRemoveScroll(CancellationToken cancellationToken = default);
}