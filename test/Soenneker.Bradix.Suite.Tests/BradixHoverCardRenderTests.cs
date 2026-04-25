using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixHoverCardRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixHoverCardRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("updateDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("registerDismissableLayerBranch", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayerBranch", _ => true).SetVoidResult();
        _module.SetupVoid("registerPopperContent", _ => true).SetVoidResult();
        _module.SetupVoid("updatePopperContent", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterPopperContent", _ => true).SetVoidResult();
        _module.SetupVoid("registerPresence", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterPresence", _ => true).SetVoidResult();
        _module.SetupVoid("mountPortal", _ => true).SetVoidResult();
        _module.SetupVoid("unmountPortal", _ => true).SetVoidResult();
        _module.SetupVoid("disableHoverCardContentTabNavigation", _ => true).SetVoidResult();
        _module.SetupVoid("registerHoverCardSelectionContainment", _ => true).SetVoidResult();
        _module.SetupVoid("beginHoverCardSelectionContainment", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterHoverCardSelectionContainment", _ => true).SetVoidResult();
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Default_open_hover_card_renders_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateHoverCard(defaultOpen: true));

        await Assert.That(cut.Markup).Contains("Hover card body");
    }

    [Test]
    public async Task Trigger_uses_non_submitting_button_semantics()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateHoverCard());

        await Assert.That(cut.Find("button").GetAttribute("type")).IsEqualTo("button");
    }

    [Test]
    public async Task Pointer_down_outside_dismisses_hover_card()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateHoverCard(defaultOpen: true));
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutside());

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).DoesNotContain("Hover card body");
            await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "unregisterHoverCardSelectionContainment")).IsTrue();
        });
    }

    [Test]
    public async Task Interact_outside_can_prevent_pointer_outside_dismissal()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateHoverCard(defaultOpen: true, onInteractOutsideDetailed: args => args.PreventDefault()));
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutside());

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).Contains("Hover card body");
        });
    }

    [Test]
    public async Task Document_pointerup_tracks_selection_state_and_preserves_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateHoverCard(defaultOpen: true));
        BradixHoverCardContent content = cut.FindComponent<BradixHoverCardContent>().Instance;

        await cut.Find("[data-state='open'] > div").TriggerEventAsync("onpointerdown", new Microsoft.AspNetCore.Components.Web.PointerEventArgs());
        await content.HandleDocumentPointerUp(true);

        await cut.Find("[data-state='open'] > div").TriggerEventAsync("onpointerleave", new Microsoft.AspNetCore.Components.Web.PointerEventArgs());

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).Contains("Hover card body");
        });
    }

    [Test]
    public async Task Focus_outside_does_not_dismiss_hover_card()
    {
        BradixFocusOutsideEventArgs? focusOutsideArgs = null;
        IRenderedComponent<ContainerFragment> cut = Render(CreateHoverCard(defaultOpen: true,
            onFocusOutsideDetailed: args => focusOutsideArgs = args));
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandleFocusOutside());

        await Assert.That(cut.Markup).Contains("Hover card body");
        await Assert.That(focusOutsideArgs?.DefaultPrevented).IsEqualTo(true);
    }

    [Test]
    public async Task Touch_pointer_leave_does_not_schedule_hover_card_close()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateHoverCard(openDelay: 0));
        IElement trigger = cut.Find("button");

        await trigger.TriggerEventAsync("onpointerenter", new Microsoft.AspNetCore.Components.Web.PointerEventArgs { PointerType = "mouse" });
        await Task.Delay(20, global::TUnit.Core.TestContext.Current.Execution.CancellationToken);
        await Assert.That(cut.Markup).Contains("Hover card body");

        await trigger.TriggerEventAsync("onpointerleave", new Microsoft.AspNetCore.Components.Web.PointerEventArgs { PointerType = "touch" });
        await Task.Delay(350, global::TUnit.Core.TestContext.Current.Execution.CancellationToken);

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).Contains("Hover card body");
        });
    }

    [Test]
    public async Task Default_open_hover_card_renders_arrow()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateHoverCard(defaultOpen: true, includeArrow: true));

        await Assert.That(cut.FindAll(".tooltip-arrow-shape")).HasSingleItem();
    }

    [Test]
    public async Task Content_forwards_popper_collision_boundary_selectors_and_sticky()
    {
        _ = Render(CreateHoverCard(defaultOpen: true, configureContent: content =>
        {
            content.AddAttribute(20, nameof(BradixHoverCardContent.CollisionBoundarySelector), "#hover-card-boundary-a");
            content.AddAttribute(21, nameof(BradixHoverCardContent.CollisionBoundarySelectors), new[] { "#hover-card-boundary-b", "#hover-card-boundary-a" });
            content.AddAttribute(22, nameof(BradixHoverCardContent.Sticky), "always");
            content.AddAttribute(23, nameof(BradixHoverCardContent.HideWhenDetached), true);
        }));

        JSRuntimeInvocation invocation = _module.Invocations.Single(call => call.Identifier == "registerPopperContent");
        object? options = invocation.Arguments[4];
        var selectors = (string[]?)options?.GetType().GetProperty("collisionBoundarySelectors")?.GetValue(options);
        var sticky = options?.GetType().GetProperty("sticky")?.GetValue(options)?.ToString();
        var hideWhenDetached = (bool?)options?.GetType().GetProperty("hideWhenDetached")?.GetValue(options);

        await Assert.That(selectors).IsEquivalentTo(["#hover-card-boundary-a", "#hover-card-boundary-b"]);
        await Assert.That(sticky).IsEqualTo("always");
        await Assert.That(hideWhenDetached).IsTrue();
    }

    private RenderFragment CreateHoverCard(bool defaultOpen = false, int openDelay = 0, bool includeArrow = false,
        Action<BradixInteractOutsideEventArgs>? onInteractOutsideDetailed = null,
        Action<BradixFocusOutsideEventArgs>? onFocusOutsideDetailed = null,
        Action<RenderTreeBuilder>? configureContent = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixHoverCard>(0);
            builder.AddAttribute(1, nameof(BradixHoverCard.DefaultOpen), defaultOpen);
            builder.AddAttribute(2, nameof(BradixHoverCard.OpenDelay), openDelay);
            builder.AddAttribute(3, nameof(BradixHoverCard.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixHoverCardTrigger>(0);
                content.AddAttribute(1, nameof(BradixHoverCardTrigger.ChildContent), (RenderFragment)(trigger =>
                {
                    trigger.AddContent(0, "Preview");
                }));
                content.CloseComponent();

                content.OpenComponent<BradixHoverCardPortal>(2);
                content.AddAttribute(3, nameof(BradixHoverCardPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixHoverCardContent>(0);
                    portal.AddAttribute(1, nameof(BradixHoverCardContent.Class), "hover-card-content");
                    configureContent?.Invoke(portal);
                    if (onInteractOutsideDetailed is not null)
                    {
                        portal.AddAttribute(2, nameof(BradixHoverCardContent.OnInteractOutsideDetailed),
                            EventCallback.Factory.Create<BradixInteractOutsideEventArgs>(this, onInteractOutsideDetailed));
                    }

                    if (onFocusOutsideDetailed is not null)
                    {
                        portal.AddAttribute(4, nameof(BradixHoverCardContent.OnFocusOutsideDetailed),
                            EventCallback.Factory.Create<BradixFocusOutsideEventArgs>(this, onFocusOutsideDetailed));
                    }

                    portal.AddAttribute(3, nameof(BradixHoverCardContent.ChildContent), (RenderFragment)(hoverContent =>
                    {
                        hoverContent.AddContent(0, "Hover card body");

                        if (includeArrow)
                        {
                            hoverContent.OpenComponent<BradixHoverCardArrow>(1);
                            hoverContent.AddAttribute(2, nameof(BradixHoverCardArrow.ChildContent), (RenderFragment)(arrow =>
                            {
                                arrow.OpenElement(0, "span");
                                arrow.AddAttribute(1, "class", "tooltip-arrow-shape");
                                arrow.CloseElement();
                            }));
                            hoverContent.CloseComponent();
                        }
                    }));
                    portal.CloseComponent();
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }
}
