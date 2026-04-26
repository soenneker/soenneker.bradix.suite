using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IPortalInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask MountPortal(ElementReference element, string? containerSelector = null, ElementReference container = default,
        CancellationToken cancellationToken = default);

    ValueTask UnmountPortal(ElementReference element, CancellationToken cancellationToken = default);
}