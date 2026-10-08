using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixRegistrationLifetimeTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixRegistrationLifetimeTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.Mode = JSRuntimeMode.Loose;
        Services.AddBradixTestInterops();
    }

    [Test]
    public async Task Tooltip_disposal_waits_for_registration_then_unregisters_once(CancellationToken cancellationToken)
    {
        var pending = _module.SetupVoid("registerTooltipTrigger", _ => true);
        var cut = Render<BradixTooltip>(p => p.AddChildContent<BradixTooltipTrigger>());
        var trigger = cut.FindComponent<BradixTooltipTrigger>();
        await cut.WaitForAssertionAsync(async () => await Assert.That(Count("registerTooltipTrigger")).IsEqualTo(1));
        Task disposal = trigger.Instance.DisposeAsync().AsTask();
        await Assert.That(disposal.IsCompleted).IsFalse();
        await Assert.That(Count("unregisterTooltipTrigger")).IsEqualTo(0);
        pending.SetVoidResult();
        await disposal;
        await trigger.Instance.DisposeAsync();
        await Assert.That(Count("unregisterTooltipTrigger")).IsEqualTo(1);
    }

    [Test]
    public async Task Otp_disposal_waits_for_registration_without_syncing_a_disposed_input(CancellationToken cancellationToken)
    {
        var pending = _module.SetupVoid("registerOneTimePasswordInput", _ => true);
        var cut = Render<BradixOneTimePasswordFieldInput>();
        await cut.WaitForAssertionAsync(async () => await Assert.That(Count("registerOneTimePasswordInput")).IsEqualTo(1));
        Task disposal = cut.Instance.DisposeAsync().AsTask();
        await Assert.That(disposal.IsCompleted).IsFalse();
        pending.SetVoidResult();
        await disposal;
        await cut.Instance.DisposeAsync();
        await Assert.That(Count("unregisterOneTimePasswordInput")).IsEqualTo(1);
        await Assert.That(Count("syncInputValue")).IsEqualTo(0);
    }

    [Test]
    public async Task Hover_card_disposal_waits_for_selection_registration(CancellationToken cancellationToken)
    {
        var pending = _module.SetupVoid("registerHoverCardSelectionContainment", _ => true);
        var cut = Render<BradixHoverCardContent>(p => p.Add(c => c.ForceMount, true));
        await cut.WaitForAssertionAsync(async () => await Assert.That(Count("registerHoverCardSelectionContainment")).IsEqualTo(1));
        Task disposal = cut.Instance.DisposeAsync().AsTask();
        await Assert.That(disposal.IsCompleted).IsFalse();
        pending.SetVoidResult();
        await disposal;
        await cut.Instance.DisposeAsync();
        await Assert.That(Count("unregisterHoverCardSelectionContainment")).IsEqualTo(1);
    }

    private int Count(string identifier) => _module.Invocations.Count(call => call.Identifier == identifier);
}
