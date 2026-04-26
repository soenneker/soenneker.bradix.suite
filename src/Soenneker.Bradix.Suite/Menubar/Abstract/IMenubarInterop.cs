using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IMenubarInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask RegisterMenubarDocumentDismiss(ElementReference element, object dotNetReference, string menubarId,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterMenubarDocumentDismiss(ElementReference element, CancellationToken cancellationToken = default);
}