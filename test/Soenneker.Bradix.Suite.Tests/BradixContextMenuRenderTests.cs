using System.Linq;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixContextMenuRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixContextMenuRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("updateDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("registerDismissableLayerBranch", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayerBranch", _ => true).SetVoidResult();
        _module.SetupVoid("registerPopperContent", _ => true).SetVoidResult();
        _module.SetupVoid("updatePopperContent", _ => true).SetVoidResult();
        _module.SetupVoid("registerVirtualPopperContent", _ => true).SetVoidResult();
        _module.SetupVoid("updateVirtualPopperContent", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterPopperContent", _ => true).SetVoidResult();
        _module.SetupVoid("registerPresence", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterPresence", _ => true).SetVoidResult();
        _module.SetupVoid("mountPortal", _ => true).SetVoidResult();
        _module.SetupVoid("unmountPortal", _ => true).SetVoidResult();
        _module.SetupVoid("registerFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("updateFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("registerFocusGuards", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusGuards", _ => true).SetVoidResult();
        _module.SetupVoid("registerHideOthers", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterHideOthers", _ => true).SetVoidResult();
        _module.SetupVoid("registerRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDelegatedInteraction", _ => true).SetVoidResult();
        _module.Setup<string>("getTextContent", _ => true).SetResult("Share");
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
    }

    [Fact]
    public void Right_click_opens_context_menu_and_registers_virtual_anchor()
    {
        var cut = Render(CreateContextMenu());
        var trigger = cut.Find("[data-state='closed']");

        trigger.TriggerEvent("oncontextmenu", new MouseEventArgs { ClientX = 120, ClientY = 40, Button = 2 });

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("open", cut.Find("[role='menu']").GetAttribute("data-state"));
        });
    }

    [Fact]
    public void Checkbox_and_radio_wrappers_render_checked_state()
    {
        var cut = Render(CreateContextMenu());
        cut.Find("[data-state='closed']").TriggerEvent("oncontextmenu", new MouseEventArgs { ClientX = 120, ClientY = 40, Button = 2 });

        cut.WaitForAssertion(() =>
        {
            var radioItems = cut.FindAll("[role='menuitemradio']");

            Assert.Equal("mixed", cut.Find("[role='menuitemcheckbox']").GetAttribute("aria-checked"));
            Assert.Contains(radioItems, item => item.GetAttribute("aria-checked") == "true");
            Assert.Contains(radioItems, item => item.GetAttribute("aria-checked") == "false");
        });
    }

    [Fact]
    public void Submenu_wrapper_opens_from_sub_trigger()
    {
        var cut = Render(CreateContextMenu());
        cut.Find("[data-state='closed']").TriggerEvent("oncontextmenu", new MouseEventArgs { ClientX = 120, ClientY = 40, Button = 2 });
        var subTrigger = cut.Find("[aria-haspopup='menu'][role='menuitem']");

        subTrigger.KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("true", cut.Find("[aria-haspopup='menu'][role='menuitem']").GetAttribute("aria-expanded"));
            Assert.Contains("Copy link", cut.Markup);
        });
    }

    [Fact]
    public void Selecting_item_closes_context_menu_root()
    {
        var cut = Render(CreateContextMenu());
        cut.Find("[data-state='closed']").TriggerEvent("oncontextmenu", new MouseEventArgs { ClientX = 120, ClientY = 40, Button = 2 });

        cut.FindAll("[role='menuitem']").First().Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("closed", cut.Find("[role='menu']").GetAttribute("data-state"));
        });
    }

    private static RenderFragment CreateContextMenu()
    {
        return builder =>
        {
            builder.OpenComponent<BradixContextMenu>(0);
            builder.AddAttribute(1, nameof(BradixContextMenu.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixContextMenuTrigger>(0);
                content.AddAttribute(1, nameof(BradixContextMenuTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, "Area")));
                content.CloseComponent();

                content.OpenComponent<BradixContextMenuPortal>(2);
                content.AddAttribute(3, nameof(BradixContextMenuPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixContextMenuContent>(0);
                    portal.AddAttribute(1, nameof(BradixContextMenuContent.ChildContent), (RenderFragment)(menuContent =>
                    {
                        menuContent.OpenComponent<BradixContextMenuItem>(0);
                        menuContent.AddAttribute(1, nameof(BradixContextMenuItem.TextValue), "Open");
                        menuContent.AddAttribute(2, nameof(BradixContextMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Open")));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixContextMenuCheckboxItem>(3);
                        menuContent.AddAttribute(4, nameof(BradixContextMenuCheckboxItem.DefaultChecked), BradixCheckboxCheckedState.Indeterminate);
                        menuContent.AddAttribute(5, nameof(BradixContextMenuCheckboxItem.CloseOnSelect), false);
                        menuContent.AddAttribute(6, nameof(BradixContextMenuCheckboxItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Show bookmarks")));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixContextMenuRadioGroup>(7);
                        menuContent.AddAttribute(8, nameof(BradixContextMenuRadioGroup.DefaultValue), "name");
                        menuContent.AddAttribute(9, nameof(BradixContextMenuRadioGroup.ChildContent), (RenderFragment)(group =>
                        {
                            group.OpenComponent<BradixContextMenuRadioItem>(0);
                            group.AddAttribute(1, nameof(BradixContextMenuRadioItem.Value), "name");
                            group.AddAttribute(2, nameof(BradixContextMenuRadioItem.CloseOnSelect), false);
                            group.AddAttribute(3, nameof(BradixContextMenuRadioItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Name")));
                            group.CloseComponent();

                            group.OpenComponent<BradixContextMenuRadioItem>(4);
                            group.AddAttribute(5, nameof(BradixContextMenuRadioItem.Value), "date");
                            group.AddAttribute(6, nameof(BradixContextMenuRadioItem.CloseOnSelect), false);
                            group.AddAttribute(7, nameof(BradixContextMenuRadioItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Date modified")));
                            group.CloseComponent();
                        }));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixContextMenuSub>(10);
                        menuContent.AddAttribute(11, nameof(BradixContextMenuSub.ChildContent), (RenderFragment)(sub =>
                        {
                            sub.OpenComponent<BradixContextMenuSubTrigger>(0);
                            sub.AddAttribute(1, nameof(BradixContextMenuSubTrigger.TextValue), "Share");
                            sub.AddAttribute(2, nameof(BradixContextMenuSubTrigger.ChildContent), (RenderFragment)(item => item.AddContent(0, "Share")));
                            sub.CloseComponent();

                            sub.OpenComponent<BradixContextMenuPortal>(3);
                            sub.AddAttribute(4, nameof(BradixContextMenuPortal.ChildContent), (RenderFragment)(subPortal =>
                            {
                                subPortal.OpenComponent<BradixContextMenuSubContent>(0);
                                subPortal.AddAttribute(1, nameof(BradixContextMenuSubContent.ChildContent), (RenderFragment)(submenu =>
                                {
                                    submenu.OpenComponent<BradixContextMenuItem>(0);
                                    submenu.AddAttribute(1, nameof(BradixContextMenuItem.TextValue), "Copy link");
                                    submenu.AddAttribute(2, nameof(BradixContextMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Copy link")));
                                    submenu.CloseComponent();
                                }));
                                subPortal.CloseComponent();
                            }));
                            sub.CloseComponent();
                        }));
                        menuContent.CloseComponent();
                    }));
                    portal.CloseComponent();
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }
}
