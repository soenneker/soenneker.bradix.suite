using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IKeyboardModeInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask<bool> IsKeyboardInteractionMode(CancellationToken cancellationToken = default);
}