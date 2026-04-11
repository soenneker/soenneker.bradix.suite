using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixToastRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixToastRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerToastViewport", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterToastViewport", _ => true).SetVoidResult();
        _module.SetupVoid("mountPortal", _ => true).SetVoidResult();
        _module.SetupVoid("unmountPortal", _ => true).SetVoidResult();
        _module.SetupVoid("capturePointer", _ => true).SetVoidResult();
        _module.SetupVoid("releasePointer", _ => true).SetVoidResult();
        _module.SetupVoid("suppressNextClick", _ => true).SetVoidResult();
        _module.SetupVoid("registerPresence", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterPresence", _ => true).SetVoidResult();
        _module.SetupVoid("focusElementById", _ => true).SetVoidResult();
        _module.Setup<bool>("isToastFocused", _ => true).SetResult(false);
        _module.Setup<string[]>("getToastAnnounceText", _ => true)
            .SetResult(["Upload complete", "Your asset is ready.", "Open uploads"]);
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "toast-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
    }

    [Fact]
    public void Viewport_renders_region_label_and_open_toast_metadata()
    {
        var cut = RenderToast();

        var region = cut.Find("[role='region']");
        var toast = cut.Find("li[data-radix-toast-root]");

        Assert.Equal("Notifications (F8)", region.GetAttribute("aria-label"));
        Assert.Equal("open", toast.GetAttribute("data-state"));
        Assert.Equal("right", toast.GetAttribute("data-swipe-direction"));
        Assert.Null(toast.GetAttribute("role"));
        Assert.Null(toast.GetAttribute("aria-live"));
    }

    [Fact]
    public async Task Close_button_keeps_toast_mounted_until_exit_animation_finishes()
    {
        var cut = RenderToast();

        cut.Find("button[data-toast-close='true']").Click();
        Assert.Single(cut.FindAll("li[data-radix-toast-root]"));

        var presence = cut.FindComponent<BradixPresence>();
        await cut.InvokeAsync(() => presence.Instance.HandleAnimationEndAsync("toast-out"));

        Assert.Empty(cut.FindAll("li[data-radix-toast-root]"));
    }

    [Fact]
    public async Task Viewport_pause_and_resume_invoke_toast_callbacks()
    {
        int pauseCount = 0;
        int resumeCount = 0;

        var cut = RenderToast(onPause: () => pauseCount++, onResume: () => resumeCount++);
        var viewport = cut.FindComponent<BradixToastViewport>();

        await cut.InvokeAsync(() => viewport.Instance.HandlePauseAsync());
        await cut.InvokeAsync(() => viewport.Instance.HandleResumeAsync());

        Assert.Equal(1, pauseCount);
        Assert.Equal(1, resumeCount);
    }

    [Fact]
    public void Viewport_reregisters_after_toast_proxies_mount()
    {
        _ = RenderToast();

        Assert.True(_module.Invocations.Count(invocation => invocation.Identifier == "registerToastViewport") >= 2);
    }

    [Fact]
    public void Toast_renders_hidden_announcement_with_alt_text_excluding_close_label()
    {
        var cut = RenderToast();

        cut.WaitForAssertion(() =>
        {
            var announce = cut.Find("span[role='status']");
            Assert.Equal("assertive", announce.GetAttribute("aria-live"));
            Assert.Contains("Notification", announce.TextContent);
            Assert.Contains("Upload complete", announce.TextContent);
            Assert.Contains("Your asset is ready.", announce.TextContent);
            Assert.Contains("Open uploads", announce.TextContent);
            Assert.DoesNotContain("Dismiss", announce.TextContent);
        });
    }

    [Fact]
    public void Action_requires_non_empty_alt_text()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Render(builder =>
            {
                builder.OpenComponent<BradixToastProvider>(0);
                builder.AddAttribute(1, nameof(BradixToastProvider.ChildContent), (RenderFragment)(content =>
                {
                    content.OpenComponent<BradixToastViewport>(0);
                    content.CloseComponent();

                    content.OpenComponent<BradixToast>(1);
                    content.AddAttribute(2, nameof(BradixToast.ChildContent), (RenderFragment)(toast =>
                    {
                        toast.OpenComponent<BradixToastAction>(0);
                        toast.AddAttribute(1, nameof(BradixToastAction.ChildContent), (RenderFragment)(action =>
                        {
                            action.AddContent(0, "Undo");
                        }));
                        toast.CloseComponent();
                    }));
                    content.CloseComponent();
                }));
                builder.CloseComponent();
            });
        });
    }

    [Fact]
    public void Swipe_sets_end_state_before_close()
    {
        var cut = RenderToast(swipeThreshold: 10);
        var toast = cut.Find("li[data-radix-toast-root]");

        toast.TriggerEvent("onpointerdown", new PointerEventArgs { Button = 0, ClientX = 0, ClientY = 0, PointerId = 7 });
        toast.TriggerEvent("onpointermove", new PointerEventArgs { Button = 0, ClientX = 20, ClientY = 0, PointerId = 7 });
        toast.TriggerEvent("onpointerup", new PointerEventArgs { Button = 0, ClientX = 20, ClientY = 0, PointerId = 7 });

        Assert.Equal("end", cut.Find("li[data-radix-toast-root]").GetAttribute("data-swipe"));
        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "capturePointer");
        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "releasePointer");
        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "suppressNextClick");
    }

    [Fact]
    public void Swipe_must_exceed_threshold_before_closing()
    {
        var cut = RenderToast(swipeThreshold: 10);
        var toast = cut.Find("li[data-radix-toast-root]");

        toast.TriggerEvent("onpointerdown", new PointerEventArgs { Button = 0, ClientX = 0, ClientY = 0, PointerId = 9 });
        toast.TriggerEvent("onpointermove", new PointerEventArgs { Button = 0, ClientX = 10, ClientY = 0, PointerId = 9 });
        toast.TriggerEvent("onpointerup", new PointerEventArgs { Button = 0, ClientX = 10, ClientY = 0, PointerId = 9 });

        cut.WaitForAssertion(() =>
        {
            var updatedToast = cut.Find("li[data-radix-toast-root]");
            Assert.Equal("open", updatedToast.GetAttribute("data-state"));
            Assert.Equal("cancel", updatedToast.GetAttribute("data-swipe"));
        });

        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "suppressNextClick");
    }

    [Fact]
    public void Pointer_cancel_after_swipe_move_sets_cancel_state_without_closing()
    {
        var cut = RenderToast(swipeThreshold: 10);
        var toast = cut.Find("li[data-radix-toast-root]");

        toast.TriggerEvent("onpointerdown", new PointerEventArgs { Button = 0, ClientX = 0, ClientY = 0, PointerId = 11 });
        toast.TriggerEvent("onpointermove", new PointerEventArgs { Button = 0, ClientX = 20, ClientY = 0, PointerId = 11 });
        toast.TriggerEvent("onpointercancel", new PointerEventArgs { Button = 0, ClientX = 20, ClientY = 0, PointerId = 11 });

        cut.WaitForAssertion(() =>
        {
            var updatedToast = cut.Find("li[data-radix-toast-root]");
            Assert.Equal("open", updatedToast.GetAttribute("data-state"));
            Assert.Equal("cancel", updatedToast.GetAttribute("data-swipe"));
        });

        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "releasePointer");
    }

    private IRenderedComponent<ContainerFragment> RenderToast(Action? onPause = null, Action? onResume = null, double swipeThreshold = 50)
    {
        return Render(builder =>
        {
            builder.OpenComponent<BradixToastProvider>(0);
            builder.AddAttribute(1, nameof(BradixToastProvider.SwipeThreshold), swipeThreshold);
            builder.AddAttribute(2, nameof(BradixToastProvider.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixToastViewport>(0);
                content.CloseComponent();

                content.OpenComponent<BradixToast>(1);
                content.AddAttribute(2, nameof(BradixToast.Class), "toast-root");
                content.AddAttribute(3, nameof(BradixToast.OnPause), EventCallback.Factory.Create(this, () => onPause?.Invoke()));
                content.AddAttribute(4, nameof(BradixToast.OnResume), EventCallback.Factory.Create(this, () => onResume?.Invoke()));
                content.AddAttribute(5, nameof(BradixToast.ChildContent), (RenderFragment)(toast =>
                {
                    toast.OpenComponent<BradixToastTitle>(0);
                    toast.AddAttribute(1, nameof(BradixToastTitle.ChildContent), (RenderFragment)(title =>
                    {
                        title.AddContent(0, "Upload complete");
                    }));
                    toast.CloseComponent();

                    toast.OpenComponent<BradixToastDescription>(2);
                    toast.AddAttribute(3, nameof(BradixToastDescription.ChildContent), (RenderFragment)(description =>
                    {
                        description.AddContent(0, "Your asset is ready.");
                    }));
                    toast.CloseComponent();

                    toast.OpenComponent<BradixToastAction>(4);
                    toast.AddAttribute(5, nameof(BradixToastAction.AltText), "Open uploads");
                    toast.AddAttribute(6, nameof(BradixToastAction.ChildContent), (RenderFragment)(action =>
                    {
                        action.AddContent(0, "Open");
                    }));
                    toast.CloseComponent();

                    toast.OpenComponent<BradixToastClose>(7);
                    toast.AddAttribute(8, nameof(BradixToastClose.AdditionalAttributes), new System.Collections.Generic.Dictionary<string, object>
                    {
                        ["data-toast-close"] = "true"
                    });
                    toast.AddAttribute(9, nameof(BradixToastClose.ChildContent), (RenderFragment)(close =>
                    {
                        close.AddContent(0, "Dismiss");
                    }));
                    toast.CloseComponent();
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });
    }
}
