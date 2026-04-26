using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IToastInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask RegisterToastViewport(ElementReference wrapper, ElementReference viewport, ElementReference headProxy, ElementReference tailProxy,
        IReadOnlyList<string> hotkey, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterToastViewport(ElementReference viewport, CancellationToken cancellationToken = default);

    ValueTask<bool> IsToastFocused(ElementReference toast, CancellationToken cancellationToken = default);

    ValueTask<string[]> GetToastAnnounceText(ElementReference element, CancellationToken cancellationToken = default);
}