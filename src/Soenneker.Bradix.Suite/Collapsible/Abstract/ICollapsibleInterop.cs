using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface ICollapsibleInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask ObserveCollapsibleContent(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask UnobserveCollapsibleContent(ElementReference element, CancellationToken cancellationToken = default);
}