using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.Tests;

internal sealed class DelayedContinuationContext : SynchronizationContext
{
    private readonly ConcurrentQueue<(SendOrPostCallback Callback, object? State)> _callbacks = new();
    private readonly TaskCompletionSource _posted = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public Task Posted => _posted.Task;

    public override void Post(SendOrPostCallback callback, object? state)
    {
        _callbacks.Enqueue((callback, state));
        _posted.TrySetResult();
    }

    public void Drain()
    {
        while (_callbacks.TryDequeue(out var pending)) pending.Callback(pending.State);
    }
}
