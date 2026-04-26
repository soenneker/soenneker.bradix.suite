using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface ILabelInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask RegisterLabelTextSelectionGuard(ElementReference element, DotNetObjectReference<object>? dotNetReference = null,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterLabelTextSelectionGuard(ElementReference element, CancellationToken cancellationToken = default);
}