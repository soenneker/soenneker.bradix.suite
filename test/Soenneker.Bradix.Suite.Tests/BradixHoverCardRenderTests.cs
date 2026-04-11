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
    public void Trigger_does_not_add_synthetic_tabindex_when_href_is_missing()
    {
        var cut = Render(CreateHoverCard());

        Assert.Null(cut.Find("a").GetAttribute("tabindex"));
    }

    [Fact]
    public async Task Pointer_down_outside_dismisses_hover_card()
    {
        var cut = Render(CreateHoverCard(defaultOpen: true));
        var layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutsideAsync());

        Assert.DoesNotContain("Hover card body", cut.Markup);
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
    public void Default_open_hover_card_renders_arrow()
    {
        var cut = Render(CreateHoverCard(defaultOpen: true, includeArrow: true));

        Assert.Single(cut.FindAll(".tooltip-arrow-shape"));
    }

    private static RenderFragment CreateHoverCard(bool defaultOpen = false, int openDelay = 0, bool includeArrow = false)
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
                    portal.AddAttribute(2, nameof(BradixHoverCardContent.ChildContent), (RenderFragment)(hoverContent =>
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
