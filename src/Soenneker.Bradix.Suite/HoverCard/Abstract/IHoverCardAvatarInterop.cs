using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IHoverCardAvatarInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask DisableHoverCardContentTabNavigation(ElementReference content, CancellationToken cancellationToken = default);

    ValueTask RegisterHoverCardSelectionContainment(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    ValueTask BeginHoverCardSelectionContainment(ElementReference content, CancellationToken cancellationToken = default);

    ValueTask UnregisterHoverCardSelectionContainment(ElementReference content, CancellationToken cancellationToken = default);

    ValueTask RegisterAvatarImageLoadingStatus(string? src, string? crossOrigin, string? referrerPolicy,
        DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterAvatarImageLoadingStatus(DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);
}