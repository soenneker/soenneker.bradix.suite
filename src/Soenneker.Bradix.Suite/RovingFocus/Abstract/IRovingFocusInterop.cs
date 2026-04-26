using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IRovingFocusInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask RegisterRovingFocusNavigationKeys(ElementReference element, object? dotNetReference = null,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterRovingFocusNavigationKeys(ElementReference element, CancellationToken cancellationToken = default);
}