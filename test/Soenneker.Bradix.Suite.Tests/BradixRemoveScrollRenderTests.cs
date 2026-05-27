using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.JSInterop;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixRemoveScrollRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixRemoveScrollRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterRemoveScroll", _ => true).SetVoidResult();

        Services.AddBradixTestInterops();
    }

    [Test]
    public async Task Remove_scroll_renders_child_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixRemoveScroll>(0);
            builder.AddAttribute(1, nameof(BradixRemoveScroll.ChildContent), (RenderFragment)(content =>
            {
                content.OpenElement(0, "div");
                content.AddContent(1, "Locked content");
                content.CloseElement();
            }));
            builder.CloseComponent();
        });

        await Assert.That(cut.Markup).Contains("Locked content");
        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerRemoveScroll")).IsTrue();
    }

    [Test]
    public async Task Remove_scroll_forwards_allow_pinch_zoom_to_interop()
    {
        Render(builder =>
        {
            builder.OpenComponent<BradixRemoveScroll>(0);
            builder.AddAttribute(1, nameof(BradixRemoveScroll.AllowPinchZoom), true);
            builder.CloseComponent();
        });

        await Assert.That(_module.Invocations.Any(invocation =>
            invocation.Identifier == "registerRemoveScroll" &&
            invocation.Arguments.Count > 1 &&
            invocation.Arguments[0] is string registrationId &&
            !string.IsNullOrWhiteSpace(registrationId) &&
            invocation.Arguments[1] is bool allowPinchZoom &&
            allowPinchZoom)).IsTrue();
    }

    [Test]
    public async Task Remove_scroll_unregisters_interop_on_dispose()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixRemoveScroll>(0);
            builder.CloseComponent();
        });

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerRemoveScroll")).IsTrue();

        await cut.InvokeAsync(() => cut.FindComponent<BradixRemoveScroll>().Instance.DisposeAsync().AsTask());

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "unregisterRemoveScroll")).IsTrue();
        await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "registerRemoveScroll")).IsEqualTo(1);
        await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "unregisterRemoveScroll")).IsEqualTo(1);

        var registerInvocation = _module.Invocations.Single(invocation => invocation.Identifier == "registerRemoveScroll");
        var unregisterInvocation = _module.Invocations.Single(invocation => invocation.Identifier == "unregisterRemoveScroll");
        await Assert.That(unregisterInvocation.Arguments[0]).IsEqualTo(registerInvocation.Arguments[0]);
    }

    [Test]
    public async Task Remove_scroll_unregisters_when_disposed_before_register_finishes()
    {
        var interop = new DelayedPresenceOverlayInterop();
        Services.RemoveAll<IPresenceOverlayInterop>();
        Services.AddSingleton<IPresenceOverlayInterop>(interop);

        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixRemoveScroll>(0);
            builder.CloseComponent();
        });

        await interop.RegisterStarted.Task.WaitAsync(TimeSpan.FromSeconds(1));

        await cut.InvokeAsync(() => cut.FindComponent<BradixRemoveScroll>().Instance.DisposeAsync().AsTask());

        await Assert.That(interop.UnregisteredIds).IsEmpty();

        interop.AllowRegister.SetResult();

        string unregisteredId = await interop.Unregistered.Task.WaitAsync(TimeSpan.FromSeconds(1));
        await Assert.That(unregisteredId).IsEqualTo(interop.RegisteredIds.Single());
    }

    private sealed class DelayedPresenceOverlayInterop : IPresenceOverlayInterop
    {
        public TaskCompletionSource RegisterStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource AllowRegister { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<string> Unregistered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public List<string> RegisteredIds { get; } = [];

        public List<string> UnregisteredIds { get; } = [];

        public ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public ValueTask RegisterPresence(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public ValueTask<BradixPresenceSnapshot> GetPresenceState(ElementReference element, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public ValueTask UnregisterPresence(ElementReference element, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public ValueTask RegisterFocusGuards(CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public ValueTask UnregisterFocusGuards(CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public ValueTask RegisterHideOthers(ElementReference element, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public ValueTask UnregisterHideOthers(ElementReference element, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public ValueTask RegisterRemoveScroll(bool allowPinchZoom = false, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

        public async ValueTask RegisterRemoveScroll(string registrationId, bool allowPinchZoom = false, CancellationToken cancellationToken = default)
        {
            RegisteredIds.Add(registrationId);
            RegisterStarted.TrySetResult();
            await AllowRegister.Task;
        }

        public ValueTask UnregisterRemoveScroll(CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

        public ValueTask UnregisterRemoveScroll(string registrationId, CancellationToken cancellationToken = default)
        {
            UnregisteredIds.Add(registrationId);
            Unregistered.TrySetResult(registrationId);
            return ValueTask.CompletedTask;
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
