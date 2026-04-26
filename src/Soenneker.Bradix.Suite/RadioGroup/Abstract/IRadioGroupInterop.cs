using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IRadioGroupInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask RegisterRadioGroupItemKeys(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask UnregisterRadioGroupItemKeys(ElementReference element, CancellationToken cancellationToken = default);
}