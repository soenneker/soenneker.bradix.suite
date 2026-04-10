using Bunit;
using Bunit.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Bradix.Suite.Avatar;
using Soenneker.Bradix.Suite.Id;
using Soenneker.Bradix.Suite.Interop;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixAvatarRenderTests : Bunit.BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixAvatarRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerAvatarImageLoadingStatus", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterAvatarImageLoadingStatus", _ => true).SetVoidResult();

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
    }

    [Fact]
    public void Fallback_renders_while_image_is_not_loaded()
    {
        var cut = Render(CreateAvatar(delayMs: null));

        Assert.Contains("JD", cut.Markup);
        Assert.Empty(cut.FindAll("img"));
    }

    [Fact]
    public async Task Loaded_status_renders_image_and_hides_fallback()
    {
        var cut = Render(CreateAvatar(delayMs: null));
        var image = cut.FindComponent<BradixAvatarImage>().Instance;

        await image.HandleImageLoadingStatusChangedAsync("loaded");

        cut.WaitForAssertion(() =>
        {
            Assert.Single(cut.FindAll("img"));
            Assert.DoesNotContain("JD", cut.Markup);
        });
    }

    [Fact]
    public async Task Delayed_fallback_waits_before_rendering()
    {
        var cut = Render(CreateAvatar(delayMs: 150));

        Assert.DoesNotContain("JD", cut.Markup);

        await Task.Delay(250, Xunit.TestContext.Current.CancellationToken);

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("JD", cut.Markup);
        }, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task Error_status_keeps_fallback_visible()
    {
        var cut = Render(CreateAvatar(delayMs: null));
        var image = cut.FindComponent<BradixAvatarImage>().Instance;

        await image.HandleImageLoadingStatusChangedAsync("error");

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("JD", cut.Markup);
            Assert.Empty(cut.FindAll("img"));
        });
    }

    private static RenderFragment CreateAvatar(int? delayMs)
    {
        return builder =>
        {
            builder.OpenComponent<BradixAvatar>(0);
            builder.AddAttribute(1, nameof(BradixAvatar.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixAvatarImage>(0);
                content.AddAttribute(1, nameof(BradixAvatarImage.Src), "https://example.com/avatar.png");
                content.AddAttribute(2, nameof(BradixAvatarImage.Alt), "Jane Doe");
                content.CloseComponent();

                content.OpenComponent<BradixAvatarFallback>(3);
                if (delayMs.HasValue)
                    content.AddAttribute(4, nameof(BradixAvatarFallback.DelayMs), delayMs.Value);

                content.AddAttribute(5, nameof(BradixAvatarFallback.ChildContent), (RenderFragment)(fallback =>
                {
                    fallback.AddContent(0, "JD");
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }
}
