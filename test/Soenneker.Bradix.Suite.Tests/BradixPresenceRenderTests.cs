using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixPresenceRenderTests : BunitContext
{
    public BradixPresenceRenderTests()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerPresence", _ => true).SetVoidResult();
        module.SetupVoid("unregisterPresence", _ => true).SetVoidResult();
        module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-in", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Present_presence_renders_content()
    {
        bool present = true;
        IRenderedComponent<ContainerFragment> cut = Render(CreatePresenceHost(() => present));

        await Assert.That(cut.Markup).Contains("Content");
    }

    [Test]
    public async Task Non_present_presence_does_not_render_initially()
    {
        bool present = false;
        IRenderedComponent<ContainerFragment> cut = Render(CreatePresenceHost(() => present));

        await Assert.That(cut.Markup).DoesNotContain("Content");
    }

    [Test]
    public async Task Exit_animation_keeps_content_mounted_until_animation_end()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreatePresenceHost(() => true));
        IRenderedComponent<BradixPresence> presence = cut.FindComponent<BradixPresence>();

        await cut.InvokeAsync(() => presence.Instance.SetParametersAsync(ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(BradixPresence.Present)] = false
        })));

        await Assert.That(cut.Markup).Contains("Content");

        await cut.InvokeAsync(() => presence.Instance.HandleAnimationEnd("fade-out"));

        await Assert.That(cut.Markup).DoesNotContain("Content");
    }

    [Test]
    public async Task Exit_complete_callback_runs_after_animation_end()
    {
        int exitCompleteCount = 0;

        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixPresence>(0);
            builder.AddAttribute(1, nameof(BradixPresence.Present), true);
            builder.AddAttribute(2, nameof(BradixPresence.OnExitComplete), EventCallback.Factory.Create(this, () => exitCompleteCount++));
            builder.AddAttribute(3, nameof(BradixPresence.ChildContent), (RenderFragment)(content =>
            {
                content.AddContent(0, "Content");
            }));
            builder.CloseComponent();
        });

        IRenderedComponent<BradixPresence> presence = cut.FindComponent<BradixPresence>();
        await cut.InvokeAsync(() => presence.Instance.SetParametersAsync(ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(BradixPresence.Present)] = false
        })));

        await cut.InvokeAsync(() => presence.Instance.HandleAnimationEnd("fade-out"));

        await Assert.That(exitCompleteCount).IsEqualTo(1);
    }

    [Test]
    public async Task Exit_animation_ignores_non_current_animation_end_events()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreatePresenceHost(() => true));
        IRenderedComponent<BradixPresence> presence = cut.FindComponent<BradixPresence>();

        await cut.InvokeAsync(() => presence.Instance.SetParametersAsync(ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(BradixPresence.Present)] = false
        })));

        await cut.InvokeAsync(() => presence.Instance.HandleAnimationEnd("fade-in", "fade-out"));

        await Assert.That(cut.Markup).Contains("Content");

        await cut.InvokeAsync(() => presence.Instance.HandleAnimationEnd("fade-out", "fade-out"));

        await Assert.That(cut.Markup).DoesNotContain("Content");
    }

    private static RenderFragment CreatePresenceHost(System.Func<bool> presentAccessor)
    {
        return builder =>
        {
            builder.OpenComponent<BradixPresence>(0);
            builder.AddAttribute(1, nameof(BradixPresence.Present), presentAccessor());
            builder.AddAttribute(2, nameof(BradixPresence.ChildContent), (RenderFragment)(content =>
            {
                content.AddContent(0, "Content");
            }));
            builder.CloseComponent();
        };
    }
}