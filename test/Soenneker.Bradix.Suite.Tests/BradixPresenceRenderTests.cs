using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixPresenceRenderTests : BunitContext
{
    public BradixPresenceRenderTests()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerPresence", _ => true).SetVoidResult();
        module.SetupVoid("unregisterPresence", _ => true).SetVoidResult();
        module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-in", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
    }

    [Fact]
    public void Present_presence_renders_content()
    {
        bool present = true;
        var cut = Render(CreatePresenceHost(() => present));

        Assert.Contains("Content", cut.Markup);
    }

    [Fact]
    public void Non_present_presence_does_not_render_initially()
    {
        bool present = false;
        var cut = Render(CreatePresenceHost(() => present));

        Assert.DoesNotContain("Content", cut.Markup);
    }

    [Fact]
    public async Task Exit_animation_keeps_content_mounted_until_animation_end()
    {
        var cut = Render(CreatePresenceHost(() => true));
        var presence = cut.FindComponent<BradixPresence>();

        await cut.InvokeAsync(() => presence.Instance.SetParametersAsync(ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(BradixPresence.Present)] = false
        })));

        Assert.Contains("Content", cut.Markup);

        await cut.InvokeAsync(() => presence.Instance.HandleAnimationEndAsync("fade-out"));

        cut.WaitForAssertion(() => Assert.DoesNotContain("Content", cut.Markup));
    }

    [Fact]
    public async Task Exit_complete_callback_runs_after_animation_end()
    {
        int exitCompleteCount = 0;

        var cut = Render(builder =>
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

        var presence = cut.FindComponent<BradixPresence>();
        await cut.InvokeAsync(() => presence.Instance.SetParametersAsync(ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(BradixPresence.Present)] = false
        })));

        await cut.InvokeAsync(() => presence.Instance.HandleAnimationEndAsync("fade-out"));

        cut.WaitForAssertion(() => Assert.Equal(1, exitCompleteCount));
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
