using System;
using System.Threading.Tasks;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixAvatarRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixAvatarRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerAvatarImageLoadingStatus", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterAvatarImageLoadingStatus", _ => true).SetVoidResult();

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Fallback_renders_while_image_is_not_loaded()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateAvatar(delayMs: null));

        await Assert.That(cut.Markup).Contains("JD");
        await Assert.That(cut.FindAll("img")).IsEmpty();
    }

    [Test]
    public async Task Loaded_status_renders_image_and_hides_fallback()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateAvatar(delayMs: null));
        BradixAvatarImage image = cut.FindComponent<BradixAvatarImage>().Instance;

        await image.HandleImageLoadingStatusChanged("loaded");

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.FindAll("img")).HasSingleItem();
            await Assert.That(cut.Markup).DoesNotContain("JD");
        });
    }

    [Test]
    public async Task Loaded_image_without_alt_renders_empty_alt_attribute()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixAvatar>(0);
            builder.AddAttribute(1, nameof(BradixAvatar.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixAvatarImage>(0);
                content.AddAttribute(1, nameof(BradixAvatarImage.Src), "https://example.com/avatar.png");
                content.CloseComponent();

                content.OpenComponent<BradixAvatarFallback>(2);
                content.AddAttribute(3, nameof(BradixAvatarFallback.ChildContent), (RenderFragment)(fallback => fallback.AddContent(0, "JD")));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });
        BradixAvatarImage image = cut.FindComponent<BradixAvatarImage>().Instance;

        await image.HandleImageLoadingStatusChanged("loaded");

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("img").GetAttribute("alt")).IsEqualTo(string.Empty);
        });
    }

    [Test]
    public async Task Delayed_fallback_waits_before_rendering()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateAvatar(delayMs: 150));

        await Assert.That(cut.Markup).DoesNotContain("JD");

        await Task.Delay(250, global::TUnit.Core.TestContext.Current.Execution.CancellationToken);

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).Contains("JD");
        }, TimeSpan.FromSeconds(2));
    }

    [Test]
    public async Task Error_status_keeps_fallback_visible()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateAvatar(delayMs: null));
        BradixAvatarImage image = cut.FindComponent<BradixAvatarImage>().Instance;

        await image.HandleImageLoadingStatusChanged("error");

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).Contains("JD");
            await Assert.That(cut.FindAll("img")).IsEmpty();
        });
    }

    [Test]
    public async Task Image_loading_status_callback_fires_before_avatar_context_updates()
    {
        var reported = new System.Collections.Generic.List<string>();
        IRenderedComponent<ContainerFragment> cut = Render(CreateAvatar(delayMs: null, status => reported.Add(status.Value)));
        BradixAvatarImage image = cut.FindComponent<BradixAvatarImage>().Instance;

        await image.HandleImageLoadingStatusChanged("loading");
        await image.HandleImageLoadingStatusChanged("loaded");

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(reported).IsEquivalentTo(["loading", "loaded"]);
            await Assert.That(cut.FindAll("img")).HasSingleItem();
            await Assert.That(cut.Markup).DoesNotContain("JD");
        });
    }

    private static RenderFragment CreateAvatar(int? delayMs, Action<BradixAvatarImageLoadingStatus>? onLoadingStatusChange = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixAvatar>(0);
            builder.AddAttribute(1, nameof(BradixAvatar.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixAvatarImage>(0);
                content.AddAttribute(1, nameof(BradixAvatarImage.Src), "https://example.com/avatar.png");
                content.AddAttribute(2, nameof(BradixAvatarImage.Alt), "Jane Doe");
                if (onLoadingStatusChange is not null)
                    content.AddAttribute(6, nameof(BradixAvatarImage.OnLoadingStatusChange),
                        EventCallback.Factory.Create<BradixAvatarImageLoadingStatus>(new object(), onLoadingStatusChange));
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
