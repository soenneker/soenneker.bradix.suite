using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IMenuInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask<bool> BeginMenuSubmenuPointerGrace(ElementReference trigger, ElementReference content, double clientX, double clientY,
        DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask CancelMenuSubmenuPointerGrace(ElementReference trigger, CancellationToken cancellationToken = default);
}