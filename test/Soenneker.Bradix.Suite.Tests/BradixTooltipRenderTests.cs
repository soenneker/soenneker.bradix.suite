using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixTooltipRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixTooltipRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerTooltipTrigger", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterTooltipTrigger", _ => true).SetVoidResult();
        _module.SetupVoid("registerTooltipContent", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterTooltipContent", _ => true).SetVoidResult();
        _module.SetupVoid("dispatchTooltipOpen", _ => true).SetVoidResult();
        _module.Setup<string>("getTextContent", _ => true).SetResult("Tooltip body");
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
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
    }

    [Fact]
    public void Focus_opens_tooltip_and_links_trigger_to_content()
    {
        var cut = Render(CreateTooltip());

        var trigger = cut.Find("button");
        trigger.Focus();

        cut.WaitForAssertion(() =>
        {
            var tooltip = cut.Find("[role='tooltip']");
            Assert.Equal(tooltip.Id, trigger.GetAttribute("aria-describedby"));
            Assert.Equal("instant-open", trigger.GetAttribute("data-state"));
            Assert.DoesNotContain("role", cut.Find(".tooltip-content").Attributes.Select(attribute => attribute.Name));
        });
    }

    [Fact]
    public void Provider_opening_second_tooltip_closes_first()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixTooltipProvider>(0);
            builder.AddAttribute(1, nameof(BradixTooltipProvider.DelayDuration), 0);
            builder.AddAttribute(2, nameof(BradixTooltipProvider.ChildContent), (RenderFragment)(content =>
            {
                content.AddContent(0, CreateTooltip("First trigger", "First tooltip"));
                content.AddContent(1, CreateTooltip("Second trigger", "Second tooltip"));
            }));
            builder.CloseComponent();
        });

        var triggers = cut.FindAll("button");
        triggers[0].Focus();
        cut.WaitForAssertion(() => Assert.Contains("First tooltip", cut.Markup));

        triggers[1].Focus();
        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("First tooltip", cut.Markup);
            Assert.Contains("Second tooltip", cut.Markup);
            Assert.Single(cut.FindAll("[role='tooltip']"));
        });
    }

    [Fact]
    public async Task Pointer_down_outside_closes_tooltip()
    {
        var cut = Render(CreateTooltip(defaultOpen: true));
        var layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutsideAsync());

        Assert.Empty(cut.FindAll("[role='tooltip']"));
    }

    [Fact]
    public void Hoverable_content_pointer_leave_does_not_close_before_grace_area_exit()
    {
        var cut = Render(CreateTooltip(defaultOpen: true));

        cut.Find(".tooltip-content > div").TriggerEvent("onpointerleave", new Microsoft.AspNetCore.Components.Web.PointerEventArgs
        {
            PointerType = "mouse"
        });

        cut.WaitForAssertion(() =>
        {
            Assert.Single(cut.FindAll("[role='tooltip']"));
            Assert.Contains("Tooltip body", cut.Markup);
        });
    }

    [Fact]
    public void Default_open_tooltip_renders_arrow()
    {
        var cut = Render(CreateTooltip(defaultOpen: true, includeArrow: true));

        Assert.Single(cut.FindAll(".tooltip-arrow-shape"));
    }

    [Fact]
    public void Aria_label_is_rendered_in_hidden_tooltip_node()
    {
        var cut = Render(CreateTooltip(ariaLabel: "Accessible tooltip"));

        cut.Find("button").Focus();

        cut.WaitForAssertion(() =>
        {
            var tooltip = cut.Find("[role='tooltip']");
            Assert.Equal("Accessible tooltip", tooltip.TextContent.Trim());
            Assert.Null(cut.Find(".tooltip-content").GetAttribute("aria-label"));
        });
    }

    private static RenderFragment CreateTooltip(string triggerText = "Trigger", string contentText = "Tooltip body", bool defaultOpen = false, bool includeArrow = false,
        string? ariaLabel = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixTooltip>(0);
            builder.AddAttribute(1, nameof(BradixTooltip.DefaultOpen), defaultOpen);
            builder.AddAttribute(2, nameof(BradixTooltip.DelayDuration), 0);
            builder.AddAttribute(3, nameof(BradixTooltip.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixTooltipTrigger>(0);
                content.AddAttribute(1, nameof(BradixTooltipTrigger.ChildContent), (RenderFragment)(trigger =>
                {
                    trigger.AddContent(0, triggerText);
                }));
                content.CloseComponent();

                content.OpenComponent<BradixTooltipPortal>(2);
                content.AddAttribute(3, nameof(BradixTooltipPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixTooltipContent>(0);
                    portal.AddAttribute(1, nameof(BradixTooltipContent.Class), "tooltip-content");
                    portal.AddAttribute(2, nameof(BradixTooltipContent.AriaLabel), ariaLabel);
                    portal.AddAttribute(3, nameof(BradixTooltipContent.ChildContent), (RenderFragment)(tooltipContent =>
                    {
                        tooltipContent.AddContent(0, contentText);

                        if (includeArrow)
                        {
                            tooltipContent.OpenComponent<BradixTooltipArrow>(1);
                            tooltipContent.AddAttribute(2, nameof(BradixTooltipArrow.ChildContent), (RenderFragment)(arrow =>
                            {
                                arrow.OpenElement(0, "span");
                                arrow.AddAttribute(1, "class", "tooltip-arrow-shape");
                                arrow.CloseElement();
                            }));
                            tooltipContent.CloseComponent();
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
