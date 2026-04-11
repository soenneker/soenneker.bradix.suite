using System;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

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
        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
    }

    [Fact]
    public void Default_open_hover_card_renders_content()
    {
        var cut = Render(CreateHoverCard(defaultOpen: true));

        Assert.Contains("Hover card body", cut.Markup);
    }

    [Fact]
    public void Trigger_uses_non_submitting_button_semantics()
    {
        var cut = Render(CreateHoverCard());

        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public async Task Pointer_down_outside_dismisses_hover_card()
    {
        var cut = Render(CreateHoverCard(defaultOpen: true));
        var layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutsideAsync());

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Hover card body", cut.Markup);
            Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "unregisterHoverCardSelectionContainment");
        });
    }

    [Fact]
    public async Task Interact_outside_can_prevent_pointer_outside_dismissal()
    {
        var cut = Render(CreateHoverCard(defaultOpen: true, onInteractOutsideDetailed: args => args.PreventDefault()));
        var layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutsideAsync());

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Hover card body", cut.Markup);
        });
    }

    [Fact]
    public async Task Document_pointerup_tracks_selection_state_and_preserves_content()
    {
        var cut = Render(CreateHoverCard(defaultOpen: true));
        var content = cut.FindComponent<BradixHoverCardContent>().Instance;

        cut.Find("[data-state='open'] > div").TriggerEvent("onpointerdown", new Microsoft.AspNetCore.Components.Web.PointerEventArgs());
        await content.HandleDocumentPointerUpAsync(true);

        cut.Find("[data-state='open'] > div").TriggerEvent("onpointerleave", new Microsoft.AspNetCore.Components.Web.PointerEventArgs());

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Hover card body", cut.Markup);
        });
    }

    [Fact]
    public async Task Focus_outside_does_not_dismiss_hover_card()
    {
        var cut = Render(CreateHoverCard(defaultOpen: true));
        var layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandleFocusOutsideAsync());

        Assert.Contains("Hover card body", cut.Markup);
    }

    [Fact]
    public async Task Touch_pointer_leave_does_not_schedule_hover_card_close()
    {
        var cut = Render(CreateHoverCard(openDelay: 0));
        var trigger = cut.Find("button");

        trigger.TriggerEvent("onpointerenter", new Microsoft.AspNetCore.Components.Web.PointerEventArgs { PointerType = "mouse" });
        await Task.Delay(20, Xunit.TestContext.Current.CancellationToken);
        cut.WaitForAssertion(() => Assert.Contains("Hover card body", cut.Markup));

        trigger.TriggerEvent("onpointerleave", new Microsoft.AspNetCore.Components.Web.PointerEventArgs { PointerType = "touch" });
        await Task.Delay(350, Xunit.TestContext.Current.CancellationToken);

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Hover card body", cut.Markup);
        });
    }

    [Fact]
    public void Default_open_hover_card_renders_arrow()
    {
        var cut = Render(CreateHoverCard(defaultOpen: true, includeArrow: true));

        Assert.Single(cut.FindAll(".tooltip-arrow-shape"));
    }

    private RenderFragment CreateHoverCard(bool defaultOpen = false, int openDelay = 0, bool includeArrow = false,
        Action<BradixInteractOutsideEventArgs>? onInteractOutsideDetailed = null)
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
                    if (onInteractOutsideDetailed is not null)
                    {
                        portal.AddAttribute(2, nameof(BradixHoverCardContent.OnInteractOutsideDetailed),
                            EventCallback.Factory.Create<BradixInteractOutsideEventArgs>(this, onInteractOutsideDetailed));
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
