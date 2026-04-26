using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IScrollAreaInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask RegisterScrollAreaRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterScrollAreaRoot(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterScrollAreaViewport(ElementReference viewport, ElementReference content, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterScrollAreaViewport(ElementReference viewport, CancellationToken cancellationToken = default);

    ValueTask RegisterScrollAreaScrollbar(ElementReference scrollbar, ElementReference thumb, ElementReference viewport, string orientation, string dir, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterScrollAreaScrollbar(ElementReference scrollbar, CancellationToken cancellationToken = default);
}