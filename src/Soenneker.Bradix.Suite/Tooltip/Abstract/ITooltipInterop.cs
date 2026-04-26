using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface ITooltipInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask RegisterTooltipTrigger(ElementReference element, object dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterTooltipTrigger(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterTooltipContent(ElementReference content, ElementReference trigger, object dotNetReference, string contentId,
        bool hoverableContent, CancellationToken cancellationToken = default);

    ValueTask UnregisterTooltipContent(ElementReference content, CancellationToken cancellationToken = default);

    ValueTask DispatchTooltipOpen(string contentId, CancellationToken cancellationToken = default);
}