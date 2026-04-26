using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface ISelectInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask RegisterSelectItemAlignedPosition(ElementReference wrapper, ElementReference content, ElementReference viewport, ElementReference trigger,
        ElementReference valueNode, ElementReference selectedItem, ElementReference selectedItemText, string dir, CancellationToken cancellationToken = default);

    ValueTask UpdateSelectItemAlignedPosition(ElementReference wrapper, ElementReference content, ElementReference viewport, ElementReference trigger,
        ElementReference valueNode, ElementReference selectedItem, ElementReference selectedItemText, string dir, CancellationToken cancellationToken = default);

    ValueTask UnregisterSelectItemAlignedPosition(ElementReference wrapper, CancellationToken cancellationToken = default);

    ValueTask RegisterSelectViewport(ElementReference viewport, ElementReference content, ElementReference wrapper,
        DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterSelectViewport(ElementReference viewport, CancellationToken cancellationToken = default);

    ValueTask ScrollSelectViewportByItem(ElementReference viewport, ElementReference item, bool upward, CancellationToken cancellationToken = default);

    ValueTask RegisterSelectContentPointerTracker(ElementReference content, DotNetObjectReference<object> dotNetReference, double pageX, double pageY,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterSelectContentPointerTracker(ElementReference content, CancellationToken cancellationToken = default);

    ValueTask RegisterSelectWindowDismiss(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterSelectWindowDismiss(ElementReference content, CancellationToken cancellationToken = default);
}