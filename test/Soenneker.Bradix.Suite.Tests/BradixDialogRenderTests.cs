using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

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
        _module.SetupVoid("registerDismissableLayerBranch", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayerBranch", _ => true).SetVoidResult();
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
    }

    [Test]
    public async Task Trigger_click_opens_dialog_and_links_title_and_description()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateDialog());

        await cut.Find("button[aria-haspopup='dialog']").ClickAsync();

        IElement dialog = cut.Find("[role='dialog']");
        IElement title = cut.Find("h2");
        IElement description = cut.Find("p");

        await Assert.That(cut.Find("button[aria-haspopup='dialog']").GetAttribute("aria-controls")).IsEqualTo(dialog.Id);
        await Assert.That(dialog.GetAttribute("aria-labelledby")).IsEqualTo(title.Id);
        await Assert.That(dialog.GetAttribute("aria-describedby")).IsEqualTo(description.Id);
    }

    [Test]
    public async Task Dialog_without_title_or_description_does_not_emit_orphaned_relationship_ids()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
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

        IElement dialog = cut.Find("[role='dialog']");
        await Assert.That(dialog.GetAttribute("aria-labelledby")).IsNull();
        await Assert.That(dialog.GetAttribute("aria-describedby")).IsNull();
    }

    [Test]
    public async Task Modal_dialog_renders_overlay_and_sets_aria_modal()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateDialog(defaultOpen: true, modal: true));

        IElement overlay = cut.Find(".dialog-overlay");
        await Assert.That(overlay.GetAttribute("style")).Contains("pointer-events:auto");
        await Assert.That(cut.Find("[role='dialog']").GetAttribute("aria-modal")).IsEqualTo("true");
    }

    [Test]
    public async Task Non_modal_dialog_does_not_render_overlay()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateDialog(defaultOpen: true, modal: false));

        await Assert.That(cut.FindAll(".dialog-overlay")).IsEmpty();
    }

    [Test]
    public async Task Modal_state_controls_focus_trap_and_outside_pointer_lock()
    {
        Render(CreateDialog(defaultOpen: true, modal: true));

        JSRuntimeInvocation focusScopeInvocation = _module.Invocations.Single(invocation => invocation.Identifier == "registerFocusScope");
        JSRuntimeInvocation dismissableLayerInvocation = _module.Invocations.Single(invocation => invocation.Identifier == "registerDismissableLayer");

        await Assert.That(focusScopeInvocation.Arguments[3]).IsEqualTo(true);
        await Assert.That(dismissableLayerInvocation.Arguments[2]).IsEqualTo(true);
    }

    [Test]
    public async Task Non_modal_state_disables_focus_trap_and_outside_pointer_lock()
    {
        Render(CreateDialog(defaultOpen: true, modal: false));

        JSRuntimeInvocation focusScopeInvocation = _module.Invocations.Single(invocation => invocation.Identifier == "registerFocusScope");
        JSRuntimeInvocation dismissableLayerInvocation = _module.Invocations.Single(invocation => invocation.Identifier == "registerDismissableLayer");

        await Assert.That(focusScopeInvocation.Arguments[3]).IsEqualTo(false);
        await Assert.That(dismissableLayerInvocation.Arguments[2]).IsEqualTo(false);
    }

    [Test]
    public async Task Pointer_down_outside_closes_dialog()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateDialog(defaultOpen: true));
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutside());

        await Assert.That(cut.Find("button[aria-haspopup='dialog']").GetAttribute("aria-expanded")).IsEqualTo("false");
    }

    [Test]
    public async Task Pointer_down_on_trigger_does_not_dismiss_non_modal_dialog()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateDialog(defaultOpen: true, modal: false));
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
    public async Task Detailed_pointer_down_outside_can_prevent_dialog_dismiss()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
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

        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();
        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutside());

        await Assert.That(cut.Find("button[aria-haspopup='dialog']").GetAttribute("aria-expanded")).IsEqualTo("true");
    }

    [Test]
    public async Task Close_button_keeps_content_mounted_until_exit_animation_finishes()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateDialog(defaultOpen: true));

        IElement closeButton = cut.Find("button[data-dialog-close='true']");
        await Assert.That(closeButton.GetAttribute("aria-label")).IsEqualTo("Close");
        await closeButton.ClickAsync();

        await Assert.That(cut.FindAll("[role='dialog']")).HasSingleItem();

        IRenderedComponent<BradixPresence> presence = cut.FindComponent<BradixPresence>();
        await cut.InvokeAsync(() => presence.Instance.HandleAnimationEnd("fade-out"));

        await Assert.That(cut.FindAll("[role='dialog']")).IsEmpty();
    }

    [Test]
    public async Task Modal_dialog_registers_hide_others()
    {
        Render(CreateDialog(defaultOpen: true));

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerHideOthers")).IsTrue();
        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerRemoveScroll")).IsTrue();
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
