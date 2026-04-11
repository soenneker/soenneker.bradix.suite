using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

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
    public void Trigger_click_opens_alertdialog_with_title_and_description()
    {
        var cut = Render(CreateAlertDialog());

        cut.Find("button[aria-haspopup='dialog']").Click();

        var dialog = cut.Find("[role='alertdialog']");
        var title = cut.Find("h2");
        var description = cut.Find("p");

        Assert.Equal(title.Id, dialog.GetAttribute("aria-labelledby"));
        Assert.Equal(description.Id, dialog.GetAttribute("aria-describedby"));
    }

    [Fact]
    public async Task Pointer_down_outside_does_not_close_alertdialog()
    {
        var cut = Render(CreateAlertDialog(defaultOpen: true));
        var layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutsideAsync());

        Assert.Single(cut.FindAll("[role='alertdialog']"));
    }

    [Fact]
    public void Cancel_and_action_buttons_render_and_modal_substrates_register()
    {
        var cut = Render(CreateAlertDialog(defaultOpen: true));

        Assert.Single(cut.FindAll("button[data-alert-cancel='true']"));
        Assert.Single(cut.FindAll("button[data-alert-action='true']"));
        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "registerHideOthers");
        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "registerRemoveScroll");
    }

    [Fact]
    public void Cancel_button_closes_alertdialog()
    {
        var cut = Render(CreateAlertDialog(defaultOpen: true));

        cut.Find("button[data-alert-cancel='true']").Click();

        Assert.Equal("false", cut.Find("button[aria-haspopup='dialog']").GetAttribute("aria-expanded"));
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
