using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixPopoverRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixPopoverRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("updateFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusScope", _ => true).SetVoidResult();
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
        _module.SetupVoid("registerFocusGuards", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusGuards", _ => true).SetVoidResult();
        _module.SetupVoid("registerHideOthers", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterHideOthers", _ => true).SetVoidResult();
        _module.SetupVoid("registerRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDelegatedInteraction", _ => true).SetVoidResult();
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Trigger_click_opens_content_and_links_it_to_trigger()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreatePopover());

        IElement trigger = cut.Find("button[aria-haspopup='dialog']");
        await trigger.ClickAsync();

        IElement dialog = cut.Find("[role='dialog']");
        await Assert.That(trigger.GetAttribute("aria-expanded")).IsEqualTo("true");
        await Assert.That(trigger.GetAttribute("aria-controls")).IsEqualTo(dialog.Id);
        await Assert.That(dialog.GetAttribute("data-state")).IsEqualTo("open");
    }

    [Test]
    public async Task Modal_popover_registers_modal_infra_and_sets_aria_modal()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreatePopover(defaultOpen: true, modal: true));

        IElement dialog = cut.Find("[role='dialog']");
        await Assert.That(dialog.GetAttribute("aria-modal")).IsEqualTo("true");
        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerRemoveScroll")).IsTrue();
        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerHideOthers")).IsTrue();
    }

    [Test]
    public async Task Pointer_down_outside_closes_non_modal_popover()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreatePopover(defaultOpen: true));

        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();
        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutside());

        IElement trigger = cut.Find("button[aria-haspopup='dialog']");
        await Assert.That(trigger.GetAttribute("aria-expanded")).IsEqualTo("false");
    }

    [Test]
    public async Task Pointer_down_on_trigger_does_not_dismiss_non_modal_popover()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreatePopover(defaultOpen: true));
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();
        IElement trigger = cut.Find("button[aria-haspopup='dialog']");
        string triggerId = await Assert.That(trigger.Id).IsTypeOf<string>();

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutside(new BradixDelegatedMouseEvent
        {
            AncestorIds = [triggerId]
        }));

        await Assert.That(trigger.GetAttribute("aria-expanded")).IsEqualTo("true");
    }

    [Test]
    public async Task Right_click_outside_does_not_dismiss_non_modal_popover()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreatePopover(defaultOpen: true));
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutside(new BradixDelegatedMouseEvent
        {
            Button = 2
        }));

        await Assert.That(cut.Find("button[aria-haspopup='dialog']").GetAttribute("aria-expanded")).IsEqualTo("true");
    }

    [Test]
    public async Task Detailed_pointer_down_outside_can_prevent_popover_dismiss()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixPopover>(0);
            builder.AddAttribute(1, nameof(BradixPopover.DefaultOpen), true);
            builder.AddAttribute(2, nameof(BradixPopover.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixPopoverTrigger>(0);
                content.AddAttribute(1, nameof(BradixPopoverTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, "Toggle")));
                content.CloseComponent();

                content.OpenComponent<BradixPopoverContent>(2);
                content.AddAttribute(3, nameof(BradixPopoverContent.OnPointerDownOutsideDetailed), EventCallback.Factory.Create<BradixPointerDownOutsideEventArgs>(this, args => args.PreventDefault()));
                content.AddAttribute(4, nameof(BradixPopoverContent.ChildContent), (RenderFragment)(popoverContent => popoverContent.AddContent(0, "Body")));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });

        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();
        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutside());

        await Assert.That(cut.Find("button[aria-haspopup='dialog']").GetAttribute("aria-expanded")).IsEqualTo("true");
    }

    [Test]
    public async Task Close_on_escape_can_be_disabled_from_popover_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreatePopover(defaultOpen: true, closeOnEscapeKeyDown: false));
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandleEscapeKeyDown());

        await Assert.That(cut.Find("button[aria-haspopup='dialog']").GetAttribute("aria-expanded")).IsEqualTo("true");
    }

    [Test]
    public async Task Close_button_keeps_content_mounted_until_exit_animation_finishes()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreatePopover(defaultOpen: true));

        await cut.Find("button[data-popover-close='true']").ClickAsync();

        await Assert.That(cut.FindAll("[role='dialog']")).HasSingleItem();

        IRenderedComponent<BradixPresence> presence = cut.FindComponent<BradixPresence>();
        await cut.InvokeAsync(() => presence.Instance.HandleAnimationEnd("fade-out"));

        await Assert.That(cut.FindAll("[role='dialog']")).IsEmpty();
    }

    [Test]
    public async Task Custom_anchor_keeps_single_trigger_and_opens_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreatePopover(customAnchor: true));

        await Assert.That(cut.FindAll("button[aria-haspopup='dialog']")).HasSingleItem();

        await cut.Find("button[aria-haspopup='dialog']").ClickAsync();

        await Assert.That(cut.FindAll("[role='dialog']")).HasSingleItem();
    }

    private static RenderFragment CreatePopover(bool defaultOpen = false, bool modal = false, bool customAnchor = false, bool closeOnEscapeKeyDown = true)
    {
        return builder =>
        {
            builder.OpenComponent<BradixPopover>(0);
            builder.AddAttribute(1, nameof(BradixPopover.DefaultOpen), defaultOpen);
            builder.AddAttribute(2, nameof(BradixPopover.Modal), modal);
            builder.AddAttribute(3, nameof(BradixPopover.ChildContent), (RenderFragment)(content =>
            {
                if (customAnchor)
                {
                    content.OpenComponent<BradixPopoverAnchor>(0);
                    content.AddAttribute(1, nameof(BradixPopoverAnchor.ChildContent), (RenderFragment)(anchor =>
                    {
                        anchor.OpenElement(0, "div");
                        anchor.AddContent(1, "Anchor");
                        anchor.OpenComponent<BradixPopoverTrigger>(2);
                        anchor.AddAttribute(3, nameof(BradixPopoverTrigger.ChildContent), (RenderFragment)(trigger =>
                        {
                            trigger.AddContent(0, "Toggle");
                        }));
                        anchor.CloseComponent();
                        anchor.CloseElement();
                    }));
                    content.CloseComponent();
                }
                else
                {
                    content.OpenComponent<BradixPopoverTrigger>(4);
                    content.AddAttribute(5, nameof(BradixPopoverTrigger.ChildContent), (RenderFragment)(trigger =>
                    {
                        trigger.AddContent(0, "Toggle");
                    }));
                    content.CloseComponent();
                }

                content.OpenComponent<BradixPopoverContent>(6);
                content.AddAttribute(7, nameof(BradixPopoverContent.Class), "popover-content");
                content.AddAttribute(8, nameof(BradixPopoverContent.CloseOnEscapeKeyDown), closeOnEscapeKeyDown);
                content.AddAttribute(9, nameof(BradixPopoverContent.ChildContent), (RenderFragment)(popoverContent =>
                {
                    popoverContent.OpenElement(0, "div");
                    popoverContent.AddContent(1, "Body");
                    popoverContent.OpenComponent<BradixPopoverClose>(2);
                    popoverContent.AddAttribute(3, nameof(BradixPopoverClose.AdditionalAttributes), new Dictionary<string, object>
                    {
                        ["data-popover-close"] = "true"
                    });
                    popoverContent.AddAttribute(4, nameof(BradixPopoverClose.ChildContent), (RenderFragment)(close =>
                    {
                        close.AddContent(0, "Close");
                    }));
                    popoverContent.CloseComponent();
                    popoverContent.CloseElement();
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }
}