using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

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
        _module.SetupVoid("registerDismissableLayerBranch", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayerBranch", _ => true).SetVoidResult();
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
    }

    [Test]
    public async Task Focus_opens_tooltip_and_links_trigger_to_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateTooltip());

        IElement trigger = cut.Find("button");
        await Assert.That(trigger.GetAttribute("type")).IsNull();
        await trigger.FocusAsync();

        await cut.WaitForAssertionAsync(async  () =>
        {
            IElement tooltip = cut.Find("[role='tooltip']");
            await Assert.That(trigger.GetAttribute("aria-describedby")).IsEqualTo(tooltip.Id);
            await Assert.That(trigger.GetAttribute("data-state")).IsEqualTo("instant-open");
            await Assert.That(cut.Find(".tooltip-content").Attributes.Select(attribute => attribute.Name)).DoesNotContain("role");
        });
    }

    [Test]
    public async Task Provider_opening_second_tooltip_closes_first()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
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

        IReadOnlyList<IElement> triggers = cut.FindAll("button");
        await triggers[0].FocusAsync();
        await Assert.That(cut.Markup).Contains("First tooltip");

        await triggers[1].FocusAsync();
        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).DoesNotContain("First tooltip");
            await Assert.That(cut.Markup).Contains("Second tooltip");
            await Assert.That(cut.FindAll("[role='tooltip']")).HasSingleItem();
        });
    }

    [Test]
    public async Task Pointer_down_outside_closes_tooltip()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateTooltip(defaultOpen: true));
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutside());

        await Assert.That(cut.FindAll("[role='tooltip']")).IsEmpty();
    }

    [Test]
    public async Task Pointer_down_outside_can_be_prevented_by_detailed_callback()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateTooltip(defaultOpen: true, onPointerDownOutsideDetailed: args => args.PreventDefault()));
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutside());

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.FindAll("[role='tooltip']")).HasSingleItem();
            await Assert.That(cut.Markup).Contains("Tooltip body");
        });
    }

    [Test]
    public async Task Hoverable_content_pointer_leave_does_not_close_before_grace_area_exit()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateTooltip(defaultOpen: true));

        await cut.Find(".tooltip-content > div").TriggerEventAsync("onpointerleave", new Microsoft.AspNetCore.Components.Web.PointerEventArgs
        {
            PointerType = "mouse"
        });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.FindAll("[role='tooltip']")).HasSingleItem();
            await Assert.That(cut.Markup).Contains("Tooltip body");
        });
    }

    [Test]
    public async Task Default_open_tooltip_renders_arrow()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateTooltip(defaultOpen: true, includeArrow: true));

        await Assert.That(cut.FindAll(".tooltip-arrow-shape")).HasSingleItem();
    }

    [Test]
    public async Task Aria_label_is_rendered_in_hidden_tooltip_node()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateTooltip(ariaLabel: "Accessible tooltip"));

        await cut.Find("button").FocusAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement tooltip = cut.Find("[role='tooltip']");
            await Assert.That(tooltip.TextContent.Trim()).IsEqualTo("Accessible tooltip");
            await Assert.That(cut.Find(".tooltip-content").GetAttribute("aria-label")).IsNull();
        });
    }

    [Test]
    public async Task Toggling_disable_hoverable_content_re_registers_tooltip_content_bridge()
    {
        IRenderedComponent<TooltipHoverableToggleHost> cut = Render<TooltipHoverableToggleHost>();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(_module.Invocations).HasSingleItem();
            await Assert.That(cut.FindAll("[role='tooltip']")).HasSingleItem();
        });

        await cut.Find("button[data-toggle-hoverable='true']").ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "registerTooltipContent")).IsEqualTo(2);
            await Assert.That(cut.FindAll("[role='tooltip']")).HasSingleItem();
        });
    }

    private RenderFragment CreateTooltip(string triggerText = "Trigger", string contentText = "Tooltip body", bool defaultOpen = false, bool includeArrow = false,
        string? ariaLabel = null, Action<BradixPointerDownOutsideEventArgs>? onPointerDownOutsideDetailed = null)
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
                    if (onPointerDownOutsideDetailed is not null)
                    {
                        portal.AddAttribute(3, nameof(BradixTooltipContent.OnPointerDownOutsideDetailed),
                            EventCallback.Factory.Create<BradixPointerDownOutsideEventArgs>(this, onPointerDownOutsideDetailed));
                    }

                    portal.AddAttribute(4, nameof(BradixTooltipContent.ChildContent), (RenderFragment)(tooltipContent =>
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

    private sealed class TooltipHoverableToggleHost : ComponentBase
    {
        private bool _disableHoverableContent;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");

            builder.OpenElement(1, "button");
            builder.AddAttribute(2, "type", "button");
            builder.AddAttribute(3, "data-toggle-hoverable", "true");
            builder.AddAttribute(4, "onclick", EventCallback.Factory.Create(this, () => _disableHoverableContent = !_disableHoverableContent));
            builder.AddContent(5, "Toggle hoverable");
            builder.CloseElement();

            builder.OpenComponent<BradixTooltip>(6);
            builder.AddAttribute(7, nameof(BradixTooltip.DefaultOpen), true);
            builder.AddAttribute(8, nameof(BradixTooltip.DelayDuration), 0);
            builder.AddAttribute(9, nameof(BradixTooltip.DisableHoverableContent), _disableHoverableContent);
            builder.AddAttribute(10, nameof(BradixTooltip.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixTooltipTrigger>(0);
                content.AddAttribute(1, nameof(BradixTooltipTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, "Trigger")));
                content.CloseComponent();

                content.OpenComponent<BradixTooltipPortal>(2);
                content.AddAttribute(3, nameof(BradixTooltipPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixTooltipContent>(0);
                    portal.AddAttribute(1, nameof(BradixTooltipContent.Class), "tooltip-content");
                    portal.AddAttribute(2, nameof(BradixTooltipContent.ChildContent), (RenderFragment)(tooltipContent => tooltipContent.AddContent(0, "Tooltip body")));
                    portal.CloseComponent();
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();

            builder.CloseElement();
        }
    }
}
