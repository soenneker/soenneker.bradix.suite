using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Bunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixDelayedStateTests : BunitContext
{
    public BradixDelayedStateTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddBradixTestInterops();
    }

    [Test]
    public async Task Canceling_a_completed_hover_delay_before_dispatch_prevents_opening()
    {
        var cut = Render<BradixHoverCard>(p => p.Add(c => c.OpenDelay, 10));
        await RunCanceledContinuation(cut.Instance, "OpenAfterDelay");
        await Assert.That(cut.Instance.CurrentOpen).IsFalse();
    }

    [Test]
    public async Task Canceling_a_completed_tooltip_delay_before_dispatch_prevents_opening()
    {
        var cut = Render<BradixTooltip>(p => p.Add(c => c.DelayDuration, 10));
        await RunCanceledContinuation(cut.Instance, "OpenAfterDelay");
        await Assert.That(cut.Instance.CurrentOpen).IsFalse();
    }

    [Test]
    public async Task Canceling_a_completed_toast_timer_before_dispatch_preserves_the_toast()
    {
        var cut = Render<BradixToast>(p => p.Add(c => c.Duration, int.MaxValue));
        await RunCanceledContinuation(cut.Instance, "RunCloseTimer", 10d);
        var open = typeof(BradixToast).GetProperty("CurrentOpen", BindingFlags.Instance | BindingFlags.NonPublic)!;
        await Assert.That(open.GetValue(cut.Instance)).IsEqualTo(true);
    }

    [Test]
    public async Task Disposed_hover_card_ignores_late_focus_callbacks()
    {
        var cut = Render<BradixHoverCard>();
        await cut.Instance.DisposeAsync();
        await cut.InvokeAsync(cut.Instance.HandleTriggerFocus);
        await Assert.That(cut.Instance.CurrentOpen).IsFalse();
    }

    private async Task RunCanceledContinuation(object component, string methodName, params object[] leadingArguments)
    {
        using var cancellation = new CancellationTokenSource();
        var queued = new DelayedContinuationContext();
        var previous = SynchronizationContext.Current;
        Task operation;
        try
        {
            SynchronizationContext.SetSynchronizationContext(queued);
            var method = component.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!;
            object[] arguments = [.. leadingArguments, cancellation.Token];
            operation = (Task)method.Invoke(component, arguments)!;
        }
        finally { SynchronizationContext.SetSynchronizationContext(previous); }
        await queued.Posted.WaitAsync(TimeSpan.FromSeconds(5));
        cancellation.Cancel();
        queued.Drain();
        await operation.WaitAsync(TimeSpan.FromSeconds(5));
    }
}
