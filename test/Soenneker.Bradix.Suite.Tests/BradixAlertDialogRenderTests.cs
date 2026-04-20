using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixAlertDialogRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixAlertDialogRenderTests()
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
        _module.SetupVoid("focusElementPreventScroll", _ => true).SetVoidResult();
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Trigger_click_opens_alertdialog_with_title_and_description()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateAlertDialog());

        await cut.Find("button[aria-haspopup='dialog']").ClickAsync();

        IElement dialog = cut.Find("[role='alertdialog']");
        IElement title = cut.Find("h2");
        IElement description = cut.Find("p");

        await Assert.That(dialog.GetAttribute("aria-labelledby")).IsEqualTo(title.Id);
        await Assert.That(dialog.GetAttribute("aria-describedby")).IsEqualTo(description.Id);
    }

    [Test]
    public async Task Pointer_down_outside_does_not_close_alertdialog()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateAlertDialog(defaultOpen: true));
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutside());

        await Assert.That(cut.FindAll("[role='alertdialog']")).HasSingleItem();
    }

    [Test]
    public async Task Cancel_and_action_buttons_render_and_modal_substrates_register()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateAlertDialog(defaultOpen: true));

        IElement cancel = cut.Find("button[data-alert-cancel='true']");
        IElement action = cut.Find("button[data-alert-action='true']");

        await Assert.That(cancel.GetAttribute("aria-label")).IsNull();
        await Assert.That(action.GetAttribute("aria-label")).IsNull();

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerHideOthers")).IsTrue();
        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerRemoveScroll")).IsTrue();
    }

    [Test]
    public async Task Cancel_button_closes_alertdialog()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateAlertDialog(defaultOpen: true));

        await cut.Find("button[data-alert-cancel='true']").ClickAsync();

        await Assert.That(cut.Find("button[aria-haspopup='dialog']").GetAttribute("aria-expanded")).IsEqualTo("false");
    }

    [Test]
    public async Task Detailed_open_auto_focus_can_prevent_alertdialog_cancel_focus()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixAlertDialog>(0);
            builder.AddAttribute(1, nameof(BradixAlertDialog.DefaultOpen), true);
            builder.AddAttribute(2, nameof(BradixAlertDialog.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixAlertDialogTrigger>(0);
                content.AddAttribute(1, nameof(BradixAlertDialogTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, "Open")));
                content.CloseComponent();

                content.OpenComponent<BradixAlertDialogPortal>(2);
                content.AddAttribute(3, nameof(BradixAlertDialogPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixAlertDialogContent>(0);
                    portal.AddAttribute(1, nameof(BradixAlertDialogContent.OnOpenAutoFocusDetailed),
                        EventCallback.Factory.Create<BradixAutoFocusEventArgs>(this, args => args.PreventDefault()));
                    portal.AddAttribute(2, nameof(BradixAlertDialogContent.ChildContent), (RenderFragment)(dialogContent =>
                    {
                        dialogContent.OpenComponent<BradixAlertDialogTitle>(0);
                        dialogContent.AddAttribute(1, nameof(BradixAlertDialogTitle.ChildContent), (RenderFragment)(title => title.AddContent(0, "Title")));
                        dialogContent.CloseComponent();

                        dialogContent.OpenComponent<BradixAlertDialogDescription>(2);
                        dialogContent.AddAttribute(3, nameof(BradixAlertDialogDescription.ChildContent), (RenderFragment)(description => description.AddContent(0, "Description")));
                        dialogContent.CloseComponent();

                        dialogContent.OpenComponent<BradixAlertDialogCancel>(4);
                        dialogContent.AddAttribute(5, nameof(BradixAlertDialogCancel.ChildContent), (RenderFragment)(cancel => cancel.AddContent(0, "Cancel")));
                        dialogContent.CloseComponent();
                    }));
                    portal.CloseComponent();
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });

        IRenderedComponent<BradixFocusScope> focusScope = cut.FindComponent<BradixFocusScope>();
        bool prevented = await cut.InvokeAsync(() => focusScope.Instance.HandleMountAutoFocus());

        await Assert.That(prevented).IsTrue();
    }

    [Test]
    public async Task Open_auto_focus_uses_prevent_scroll_for_cancel_button()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateAlertDialog(defaultOpen: true));
        IRenderedComponent<BradixFocusScope> focusScope = cut.FindComponent<BradixFocusScope>();
        int focusCountBefore = _module.Invocations.Count(invocation => invocation.Identifier == "focusElementPreventScroll");

        bool prevented = await cut.InvokeAsync(() => focusScope.Instance.HandleMountAutoFocus());

        await Assert.That(prevented).IsTrue();
        await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "focusElementPreventScroll")).IsEqualTo(focusCountBefore + 1);
    }

    private static RenderFragment CreateAlertDialog(bool defaultOpen = false)
    {
        return builder =>
        {
            builder.OpenComponent<BradixAlertDialog>(0);
            builder.AddAttribute(1, nameof(BradixAlertDialog.DefaultOpen), defaultOpen);
            builder.AddAttribute(2, nameof(BradixAlertDialog.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixAlertDialogTrigger>(0);
                content.AddAttribute(1, nameof(BradixAlertDialogTrigger.ChildContent), (RenderFragment)(trigger =>
                {
                    trigger.AddContent(0, "Open");
                }));
                content.CloseComponent();

                content.OpenComponent<BradixAlertDialogPortal>(2);
                content.AddAttribute(3, nameof(BradixAlertDialogPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixAlertDialogOverlay>(0);
                    portal.AddAttribute(1, nameof(BradixAlertDialogOverlay.Class), "dialog-overlay");
                    portal.CloseComponent();

                    portal.OpenComponent<BradixAlertDialogContent>(2);
                    portal.AddAttribute(3, nameof(BradixAlertDialogContent.Class), "dialog-content");
                    portal.AddAttribute(4, nameof(BradixAlertDialogContent.ChildContent), (RenderFragment)(dialogContent =>
                    {
                        dialogContent.OpenComponent<BradixAlertDialogTitle>(0);
                        dialogContent.AddAttribute(1, nameof(BradixAlertDialogTitle.ChildContent), (RenderFragment)(title =>
                        {
                            title.AddContent(0, "Title");
                        }));
                        dialogContent.CloseComponent();

                        dialogContent.OpenComponent<BradixAlertDialogDescription>(2);
                        dialogContent.AddAttribute(3, nameof(BradixAlertDialogDescription.ChildContent), (RenderFragment)(description =>
                        {
                            description.AddContent(0, "Description");
                        }));
                        dialogContent.CloseComponent();

                        dialogContent.OpenComponent<BradixAlertDialogCancel>(4);
                        dialogContent.AddAttribute(5, nameof(BradixAlertDialogCancel.AdditionalAttributes), new Dictionary<string, object>
                        {
                            ["data-alert-cancel"] = "true"
                        });
                        dialogContent.AddAttribute(6, nameof(BradixAlertDialogCancel.ChildContent), (RenderFragment)(cancel =>
                        {
                            cancel.AddContent(0, "Cancel");
                        }));
                        dialogContent.CloseComponent();

                        dialogContent.OpenComponent<BradixAlertDialogAction>(7);
                        dialogContent.AddAttribute(8, nameof(BradixAlertDialogAction.AdditionalAttributes), new Dictionary<string, object>
                        {
                            ["data-alert-action"] = "true"
                        });
                        dialogContent.AddAttribute(9, nameof(BradixAlertDialogAction.ChildContent), (RenderFragment)(action =>
                        {
                            action.AddContent(0, "Confirm");
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
