using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixDialogRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixDialogRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("updateFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("registerDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("updateDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("registerFocusGuards", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusGuards", _ => true).SetVoidResult();
        _module.SetupVoid("registerPresence", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterPresence", _ => true).SetVoidResult();
        _module.SetupVoid("registerHideOthers", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterHideOthers", _ => true).SetVoidResult();
        _module.SetupVoid("registerRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("mountPortal", _ => true).SetVoidResult();
        _module.SetupVoid("unmountPortal", _ => true).SetVoidResult();
        _module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDelegatedInteraction", _ => true).SetVoidResult();
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
    }

    [Fact]
    public void Trigger_click_opens_dialog_and_links_title_and_description()
    {
        var cut = Render(CreateDialog());

        cut.Find("button[aria-haspopup='dialog']").Click();

        var dialog = cut.Find("[role='dialog']");
        var title = cut.Find("h2");
        var description = cut.Find("p");

        Assert.Equal(dialog.Id, cut.Find("button[aria-haspopup='dialog']").GetAttribute("aria-controls"));
        Assert.Equal(title.Id, dialog.GetAttribute("aria-labelledby"));
        Assert.Equal(description.Id, dialog.GetAttribute("aria-describedby"));
    }

    [Fact]
    public void Dialog_without_title_or_description_does_not_emit_orphaned_relationship_ids()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixDialog>(0);
            builder.AddAttribute(1, nameof(BradixDialog.DefaultOpen), true);
            builder.AddAttribute(2, nameof(BradixDialog.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixDialogTrigger>(0);
                content.AddAttribute(1, nameof(BradixDialogTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, "Open")));
                content.CloseComponent();

                content.OpenComponent<BradixDialogContent>(2);
                content.AddAttribute(3, nameof(BradixDialogContent.ChildContent), (RenderFragment)(dialogContent => dialogContent.AddContent(0, "Body")));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });

        var dialog = cut.Find("[role='dialog']");
        Assert.Null(dialog.GetAttribute("aria-labelledby"));
        Assert.Null(dialog.GetAttribute("aria-describedby"));
    }

    [Fact]
    public void Modal_dialog_renders_overlay_and_sets_aria_modal()
    {
        var cut = Render(CreateDialog(defaultOpen: true, modal: true));

        Assert.Single(cut.FindAll(".dialog-overlay"));
        Assert.Equal("true", cut.Find("[role='dialog']").GetAttribute("aria-modal"));
    }

    [Fact]
    public void Non_modal_dialog_does_not_render_overlay()
    {
        var cut = Render(CreateDialog(defaultOpen: true, modal: false));

        Assert.Empty(cut.FindAll(".dialog-overlay"));
    }

    [Fact]
    public async Task Pointer_down_outside_closes_dialog()
    {
        var cut = Render(CreateDialog(defaultOpen: true));
        var layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutsideAsync());

        Assert.Equal("false", cut.Find("button[aria-haspopup='dialog']").GetAttribute("aria-expanded"));
    }

    [Fact]
    public async Task Pointer_down_on_trigger_does_not_dismiss_non_modal_dialog()
    {
        var cut = Render(CreateDialog(defaultOpen: true, modal: false));
        var layer = cut.FindComponent<BradixDismissableLayer>();
        var trigger = cut.Find("button[aria-haspopup='dialog']");
        string triggerId = Assert.IsType<string>(trigger.Id);

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutsideAsync(new BradixDelegatedMouseEvent
        {
            AncestorIds = [triggerId]
        }));

        Assert.Equal("true", trigger.GetAttribute("aria-expanded"));
    }

    [Fact]
    public async Task Detailed_pointer_down_outside_can_prevent_dialog_dismiss()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixDialog>(0);
            builder.AddAttribute(1, nameof(BradixDialog.DefaultOpen), true);
            builder.AddAttribute(2, nameof(BradixDialog.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixDialogTrigger>(0);
                content.AddAttribute(1, nameof(BradixDialogTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, "Open")));
                content.CloseComponent();

                content.OpenComponent<BradixDialogContent>(2);
                content.AddAttribute(3, nameof(BradixDialogContent.OnPointerDownOutsideDetailed), EventCallback.Factory.Create<BradixPointerDownOutsideEventArgs>(this, args => args.PreventDefault()));
                content.AddAttribute(4, nameof(BradixDialogContent.ChildContent), (RenderFragment)(dialogContent => dialogContent.AddContent(0, "Body")));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });

        var layer = cut.FindComponent<BradixDismissableLayer>();
        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutsideAsync());

        Assert.Equal("true", cut.Find("button[aria-haspopup='dialog']").GetAttribute("aria-expanded"));
    }

    [Fact]
    public async Task Close_button_keeps_content_mounted_until_exit_animation_finishes()
    {
        var cut = Render(CreateDialog(defaultOpen: true));

        var closeButton = cut.Find("button[data-dialog-close='true']");
        Assert.Equal("Close", closeButton.GetAttribute("aria-label"));
        closeButton.Click();

        Assert.Single(cut.FindAll("[role='dialog']"));

        var presence = cut.FindComponent<BradixPresence>();
        await cut.InvokeAsync(() => presence.Instance.HandleAnimationEndAsync("fade-out"));

        Assert.Empty(cut.FindAll("[role='dialog']"));
    }

    [Fact]
    public void Modal_dialog_registers_hide_others()
    {
        Render(CreateDialog(defaultOpen: true));

        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "registerHideOthers");
        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "registerRemoveScroll");
    }

    private static RenderFragment CreateDialog(bool defaultOpen = false, bool modal = true)
    {
        return builder =>
        {
            builder.OpenComponent<BradixDialog>(0);
            builder.AddAttribute(1, nameof(BradixDialog.DefaultOpen), defaultOpen);
            builder.AddAttribute(2, nameof(BradixDialog.Modal), modal);
            builder.AddAttribute(3, nameof(BradixDialog.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixDialogTrigger>(0);
                content.AddAttribute(1, nameof(BradixDialogTrigger.ChildContent), (RenderFragment)(trigger =>
                {
                    trigger.AddContent(0, "Open");
                }));
                content.CloseComponent();

                content.OpenComponent<BradixDialogPortal>(2);
                content.AddAttribute(3, nameof(BradixDialogPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixDialogOverlay>(0);
                    portal.AddAttribute(1, nameof(BradixDialogOverlay.Class), "dialog-overlay");
                    portal.CloseComponent();

                    portal.OpenComponent<BradixDialogContent>(2);
                    portal.AddAttribute(3, nameof(BradixDialogContent.Class), "dialog-content");
                    portal.AddAttribute(4, nameof(BradixDialogContent.ChildContent), (RenderFragment)(dialogContent =>
                    {
                        dialogContent.OpenComponent<BradixDialogTitle>(0);
                        dialogContent.AddAttribute(1, nameof(BradixDialogTitle.ChildContent), (RenderFragment)(title =>
                        {
                            title.AddContent(0, "Title");
                        }));
                        dialogContent.CloseComponent();

                        dialogContent.OpenComponent<BradixDialogDescription>(2);
                        dialogContent.AddAttribute(3, nameof(BradixDialogDescription.ChildContent), (RenderFragment)(description =>
                        {
                            description.AddContent(0, "Description");
                        }));
                        dialogContent.CloseComponent();

                        dialogContent.OpenComponent<BradixDialogClose>(4);
                        dialogContent.AddAttribute(5, nameof(BradixDialogClose.AdditionalAttributes), new Dictionary<string, object>
                        {
                            ["data-dialog-close"] = "true"
                        });
                        dialogContent.AddAttribute(6, nameof(BradixDialogClose.ChildContent), (RenderFragment)(close =>
                        {
                            close.AddContent(0, "Close");
                        }));
                        dialogContent.CloseComponent();
                    }));
                    portal.CloseComponent();
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }
}
