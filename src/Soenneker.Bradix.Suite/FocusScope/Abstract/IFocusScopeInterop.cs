using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IFocusScopeInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask RegisterFocusScope(ElementReference element, DotNetObjectReference<object> dotNetReference, bool loop, bool trapped,
        bool preventMountAutoFocus, bool preventUnmountAutoFocus, CancellationToken cancellationToken = default);

    ValueTask UpdateFocusScope(ElementReference element, bool loop, bool trapped, bool preventMountAutoFocus, bool preventUnmountAutoFocus,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterFocusScope(ElementReference element, bool unmountAutoFocusPrevented = false, CancellationToken cancellationToken = default);
}