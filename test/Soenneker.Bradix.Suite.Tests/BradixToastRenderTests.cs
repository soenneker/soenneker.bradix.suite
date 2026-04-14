using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

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
        _module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDelegatedInteraction", _ => true).SetVoidResult();
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
    }

    [Test]
    public async Task Viewport_renders_region_label_and_open_toast_metadata()
    {
        IRenderedComponent<ContainerFragment> cut = RenderToast();

        IElement region = cut.Find("[role='region']");
        IElement toast = cut.Find("li[data-radix-toast-root]");

        await Assert.That(region.GetAttribute("aria-label")).IsEqualTo("Notifications (F8)");
        await Assert.That(toast.GetAttribute("data-state")).IsEqualTo("open");
        await Assert.That(toast.GetAttribute("data-swipe-direction")).IsEqualTo("right");
        await Assert.That(toast.GetAttribute("role")).IsNull();
        await Assert.That(toast.GetAttribute("aria-live")).IsNull();
    }

    [Test]
    public async Task Close_button_keeps_toast_mounted_until_exit_animation_finishes()
    {
        IRenderedComponent<ContainerFragment> cut = RenderToast();

        await Assert.That(cut.Find("button[data-toast-close='true']").GetAttribute("aria-label")).IsEqualTo("Close");
        await cut.Find("button[data-toast-close='true']").ClickAsync();
        await Assert.That(cut.FindAll("li[data-radix-toast-root]")).HasSingleItem();

        IRenderedComponent<BradixPresence> presence = cut.FindComponent<BradixPresence>();
        await cut.InvokeAsync(() => presence.Instance.HandleAnimationEnd("toast-out"));

        await Assert.That(cut.FindAll("li[data-radix-toast-root]")).IsEmpty();
    }

    [Test]
    public async Task Viewport_pause_and_resume_invoke_toast_callbacks()
    {
        int pauseCount = 0;
        int resumeCount = 0;

        IRenderedComponent<ContainerFragment> cut = RenderToast(onPause: () => pauseCount++, onResume: () => resumeCount++);
        IRenderedComponent<BradixToastViewport> viewport = cut.FindComponent<BradixToastViewport>();

        await cut.InvokeAsync(() => viewport.Instance.HandlePause());
        await cut.InvokeAsync(() => viewport.Instance.HandleResume());

        await Assert.That(pauseCount).IsEqualTo(1);
        await Assert.That(resumeCount).IsEqualTo(1);
    }

    [Test]
    public async Task Viewport_reregisters_after_toast_proxies_mount()
    {
        _ = RenderToast();

        await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "registerToastViewport") >= 2).IsTrue();
    }

    [Test]
    public async Task Toast_renders_hidden_announcement_with_alt_text_excluding_close_label()
    {
        IRenderedComponent<ContainerFragment> cut = RenderToast();

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement announce = cut.Find("span[role='status']");
            await Assert.That(announce.GetAttribute("aria-live")).IsEqualTo("assertive");
            await Assert.That(announce.TextContent).Contains("Notification");
            await Assert.That(announce.TextContent).Contains("Upload complete");
            await Assert.That(announce.TextContent).Contains("Your asset is ready.");
            await Assert.That(announce.TextContent).Contains("Open uploads");
            await Assert.That(announce.TextContent).DoesNotContain("Dismiss");
        });
    }

    [Test]
    public async Task Action_requires_non_empty_alt_text()
    {
        await Assert.That(() =>
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
        }).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task Swipe_sets_end_state_before_close()
    {
        IRenderedComponent<ContainerFragment> cut = RenderToast(swipeThreshold: 10);
        IElement toast = cut.Find("li[data-radix-toast-root]");

        await toast.TriggerEventAsync("onpointerdown", new PointerEventArgs { Button = 0, ClientX = 0, ClientY = 0, PointerId = 7 });
        await toast.TriggerEventAsync("onpointermove", new PointerEventArgs { Button = 0, ClientX = 20, ClientY = 0, PointerId = 7 });
        await toast.TriggerEventAsync("onpointerup", new PointerEventArgs { Button = 0, ClientX = 20, ClientY = 0, PointerId = 7 });

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement li = cut.Find("li[data-radix-toast-root]");
            await Assert.That(li.GetAttribute("data-swipe")).IsEqualTo("end");
        });
        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "capturePointer")).IsTrue();
        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "releasePointer")).IsTrue();
        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "suppressNextClick")).IsTrue();
    }

    [Test]
    public async Task Swipe_must_exceed_threshold_before_closing()
    {
        IRenderedComponent<ContainerFragment> cut = RenderToast(swipeThreshold: 10);
        IElement toast = cut.Find("li[data-radix-toast-root]");

        await toast.TriggerEventAsync("onpointerdown", new PointerEventArgs { Button = 0, ClientX = 0, ClientY = 0, PointerId = 9 });
        await toast.TriggerEventAsync("onpointermove", new PointerEventArgs { Button = 0, ClientX = 10, ClientY = 0, PointerId = 9 });
        await toast.TriggerEventAsync("onpointerup", new PointerEventArgs { Button = 0, ClientX = 10, ClientY = 0, PointerId = 9 });

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement updatedToast = cut.Find("li[data-radix-toast-root]");
            await Assert.That(updatedToast.GetAttribute("data-state")).IsEqualTo("open");
            await Assert.That(updatedToast.GetAttribute("data-swipe")).IsEqualTo("cancel");
        });

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "suppressNextClick")).IsTrue();
    }

    [Test]
    public async Task Pointer_cancel_after_swipe_move_sets_cancel_state_without_closing()
    {
        IRenderedComponent<ContainerFragment> cut = RenderToast(swipeThreshold: 10);
        IElement toast = cut.Find("li[data-radix-toast-root]");

        await toast.TriggerEventAsync("onpointerdown", new PointerEventArgs { Button = 0, ClientX = 0, ClientY = 0, PointerId = 11 });
        await toast.TriggerEventAsync("onpointermove", new PointerEventArgs { Button = 0, ClientX = 20, ClientY = 0, PointerId = 11 });
        await toast.TriggerEventAsync("onpointercancel", new PointerEventArgs { Button = 0, ClientX = 20, ClientY = 0, PointerId = 11 });

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement updatedToast = cut.Find("li[data-radix-toast-root]");
            await Assert.That(updatedToast.GetAttribute("data-state")).IsEqualTo("open");
            await Assert.That(updatedToast.GetAttribute("data-swipe")).IsEqualTo("cancel");
        });

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "releasePointer")).IsTrue();
    }

    [Test]
    public async Task Escape_closes_toast_through_delegated_keydown()
    {
        IRenderedComponent<ContainerFragment> cut = RenderToast();
        IRenderedComponent<BradixToast> toast = cut.FindComponent<BradixToast>();

        await cut.InvokeAsync(() => toast.Instance.HandleDelegatedKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "Escape"
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("li[data-radix-toast-root]").GetAttribute("data-state")).IsEqualTo("closed");
        });
    }

    [Test]
    public async Task Escape_can_be_prevented_by_detailed_callback()
    {
        IRenderedComponent<ContainerFragment> cut = RenderToast(onEscapeKeyDownDetailed: args => args.PreventDefault());
        IRenderedComponent<BradixToast> toast = cut.FindComponent<BradixToast>();

        await cut.InvokeAsync(() => toast.Instance.HandleDelegatedKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "Escape"
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("li[data-radix-toast-root]").GetAttribute("data-state")).IsEqualTo("open");
        });
    }

    private IRenderedComponent<ContainerFragment> RenderToast(Action? onPause = null, Action? onResume = null, double swipeThreshold = 50,
        Action<BradixEscapeKeyDownEventArgs>? onEscapeKeyDownDetailed = null)
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
                if (onEscapeKeyDownDetailed is not null)
                {
                    content.AddAttribute(5, nameof(BradixToast.OnEscapeKeyDownDetailed),
                        EventCallback.Factory.Create<BradixEscapeKeyDownEventArgs>(this, onEscapeKeyDownDetailed));
                }

                content.AddAttribute(6, nameof(BradixToast.ChildContent), (RenderFragment)(toast =>
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