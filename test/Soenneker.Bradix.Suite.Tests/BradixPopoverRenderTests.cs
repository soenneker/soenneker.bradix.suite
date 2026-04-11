using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

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
        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
    }

    [Fact]
    public void Trigger_click_opens_content_and_links_it_to_trigger()
    {
        var cut = Render(CreatePopover());

        var trigger = cut.Find("button[aria-haspopup='dialog']");
        trigger.Click();

        var dialog = cut.Find("[role='dialog']");
        Assert.Equal("true", trigger.GetAttribute("aria-expanded"));
        Assert.Equal(dialog.Id, trigger.GetAttribute("aria-controls"));
        Assert.Equal("open", dialog.GetAttribute("data-state"));
    }

    [Fact]
    public void Modal_popover_marks_content_as_modal()
    {
        var cut = Render(CreatePopover(defaultOpen: true, modal: true));

        var dialog = cut.Find("[role='dialog']");
        Assert.Equal("true", dialog.GetAttribute("aria-modal"));
        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "registerRemoveScroll");
        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "registerHideOthers");
    }

    [Fact]
    public async Task Pointer_down_outside_closes_non_modal_popover()
    {
        var cut = Render(CreatePopover(defaultOpen: true));

        var layer = cut.FindComponent<BradixDismissableLayer>();
        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutsideAsync());

        var trigger = cut.Find("button[aria-haspopup='dialog']");
        Assert.Equal("false", trigger.GetAttribute("aria-expanded"));
    }

    [Fact]
    public async Task Close_button_keeps_content_mounted_until_exit_animation_finishes()
    {
        var cut = Render(CreatePopover(defaultOpen: true));

        cut.Find("button[data-popover-close='true']").Click();

        Assert.Single(cut.FindAll("[role='dialog']"));

        var presence = cut.FindComponent<BradixPresence>();
        await cut.InvokeAsync(() => presence.Instance.HandleAnimationEndAsync("fade-out"));

        Assert.Empty(cut.FindAll("[role='dialog']"));
    }

    [Fact]
    public void Custom_anchor_keeps_single_trigger_and_opens_content()
    {
        var cut = Render(CreatePopover(customAnchor: true));

        Assert.Single(cut.FindAll("button[aria-haspopup='dialog']"));

        cut.Find("button[aria-haspopup='dialog']").Click();

        Assert.Single(cut.FindAll("[role='dialog']"));
    }

    private static RenderFragment CreatePopover(bool defaultOpen = false, bool modal = false, bool customAnchor = false)
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
                content.AddAttribute(8, nameof(BradixPopoverContent.ChildContent), (RenderFragment)(popoverContent =>
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
