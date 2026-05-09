using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface INavigationMenuInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask RegisterNavigationMenuTriggerInteraction(ElementReference element, object dotNetReference,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterNavigationMenuTriggerInteraction(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterNavigationMenuIndicator(ElementReference indicator, ElementReference activeTrigger, ElementReference track,
        object dotNetReference, string orientation, CancellationToken cancellationToken = default);

    ValueTask UpdateNavigationMenuIndicator(ElementReference indicator, ElementReference activeTrigger, ElementReference track,
        string orientation, CancellationToken cancellationToken = default);

    ValueTask UnregisterNavigationMenuIndicator(ElementReference indicator, CancellationToken cancellationToken = default);

    ValueTask RegisterNavigationMenuContentFocusBridge(ElementReference content, ElementReference trigger, ElementReference startProxy,
        ElementReference endProxy, CancellationToken cancellationToken = default);

    ValueTask UpdateNavigationMenuContentFocusBridge(ElementReference content, ElementReference trigger, ElementReference startProxy,
        ElementReference endProxy, CancellationToken cancellationToken = default);

    ValueTask<bool> FocusNavigationMenuContent(ElementReference content, CancellationToken cancellationToken = default);

    ValueTask UnregisterNavigationMenuContentFocusBridge(ElementReference content, CancellationToken cancellationToken = default);

    ValueTask RegisterNavigationMenuViewport(ElementReference viewport, ElementReference content, object dotNetReference,
        CancellationToken cancellationToken = default);

    ValueTask UpdateNavigationMenuViewport(ElementReference viewport, ElementReference content, CancellationToken cancellationToken = default);

    ValueTask UnregisterNavigationMenuViewport(ElementReference viewport, CancellationToken cancellationToken = default);
}
