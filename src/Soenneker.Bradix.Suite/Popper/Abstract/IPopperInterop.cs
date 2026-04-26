using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IPopperInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask RegisterPopperContent(ElementReference anchor, ElementReference content, ElementReference arrow, DotNetObjectReference<object> dotNetReference, object options, CancellationToken cancellationToken = default);

    ValueTask RegisterPopperContentBySelector(string anchorSelector, ElementReference content, ElementReference arrow,
        DotNetObjectReference<object> dotNetReference, object options, CancellationToken cancellationToken = default);

    ValueTask RegisterVirtualPopperContent(ElementReference content, ElementReference arrow, DotNetObjectReference<object> dotNetReference, double x, double y, object options, CancellationToken cancellationToken = default);

    ValueTask UpdatePopperContent(ElementReference content, ElementReference arrow, object options, CancellationToken cancellationToken = default);

    ValueTask UpdateVirtualPopperContent(ElementReference content, ElementReference arrow, double x, double y, object options, CancellationToken cancellationToken = default);

    ValueTask UnregisterPopperContent(ElementReference content, CancellationToken cancellationToken = default);
}