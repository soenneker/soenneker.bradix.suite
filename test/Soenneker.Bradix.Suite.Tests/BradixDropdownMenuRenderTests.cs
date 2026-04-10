using System.Linq;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixDropdownMenuRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixDropdownMenuRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
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
        _module.SetupVoid("registerFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("updateFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("registerFocusGuards", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusGuards", _ => true).SetVoidResult();
        _module.SetupVoid("registerHideOthers", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterHideOthers", _ => true).SetVoidResult();
        _module.SetupVoid("registerRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterRemoveScroll", _ => true).SetVoidResult();
        _module.Setup<string>("getTextContent", _ => true).SetResult("Share");
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
    }

    [Fact]
    public void Trigger_arrow_down_opens_content_and_links_ids()
    {
        var cut = Render(CreateDropdownMenu());
        var trigger = cut.Find("button");

        trigger.KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowDown" });

        cut.WaitForAssertion(() =>
        {
            var menu = cut.Find("[role='menu']");
            Assert.Equal("true", trigger.GetAttribute("aria-expanded"));
            Assert.Equal(menu.Id, trigger.GetAttribute("aria-controls"));
            Assert.Equal(trigger.Id, menu.GetAttribute("aria-labelledby"));
        });
    }

    [Fact]
    public void Trigger_pointer_down_opens_content()
    {
        var cut = Render(CreateDropdownMenu());
        var trigger = cut.Find("button");

        trigger.TriggerEvent("onpointerdown", new Microsoft.AspNetCore.Components.Web.PointerEventArgs
        {
            Button = 0,
            CtrlKey = false
        });

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("true", trigger.GetAttribute("aria-expanded"));
            Assert.Equal("open", cut.Find("[role='menu']").GetAttribute("data-state"));
        });
    }

    [Fact]
    public void Opening_dropdown_registers_focus_scope_with_manual_autofocus_flags()
    {
        _ = Render(CreateDropdownMenu(defaultOpen: true));

        var invocation = _module.Invocations.Single(call => call.Identifier == "registerFocusScope");

        Assert.Equal(true, invocation.Arguments[4]);
        Assert.Equal(true, invocation.Arguments[5]);
    }

    [Fact]
    public void Checkbox_and_radio_wrappers_render_checked_state()
    {
        var cut = Render(CreateDropdownMenu(defaultOpen: true));

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
        var cut = Render(CreateDropdownMenu(defaultOpen: true));
        var subTrigger = cut.Find("[aria-haspopup='menu'][role='menuitem']");

        subTrigger.KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowRight" });

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("true", cut.Find("[aria-haspopup='menu'][role='menuitem']").GetAttribute("aria-expanded"));
            Assert.Contains("Copy link", cut.Markup);
        });
    }

    [Fact]
    public void Selecting_item_closes_dropdown_root()
    {
        var cut = Render(CreateDropdownMenu(defaultOpen: true));

        cut.FindAll("[role='menuitem']").First().Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("closed", cut.Find("[role='menu']").GetAttribute("data-state"));
        });
    }

    private static RenderFragment CreateDropdownMenu(bool defaultOpen = false)
    {
        return builder =>
        {
            builder.OpenComponent<BradixDropdownMenu>(0);
            builder.AddAttribute(1, nameof(BradixDropdownMenu.DefaultOpen), defaultOpen);
            builder.AddAttribute(2, nameof(BradixDropdownMenu.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixDropdownMenuTrigger>(0);
                content.AddAttribute(1, nameof(BradixDropdownMenuTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, "Open")));
                content.CloseComponent();

                content.OpenComponent<BradixDropdownMenuPortal>(2);
                content.AddAttribute(3, nameof(BradixDropdownMenuPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixDropdownMenuContent>(0);
                    portal.AddAttribute(1, nameof(BradixDropdownMenuContent.ChildContent), (RenderFragment)(menuContent =>
                    {
                        menuContent.OpenComponent<BradixDropdownMenuItem>(0);
                        menuContent.AddAttribute(1, nameof(BradixDropdownMenuItem.TextValue), "Edit");
                        menuContent.AddAttribute(2, nameof(BradixDropdownMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Edit")));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixDropdownMenuCheckboxItem>(3);
                        menuContent.AddAttribute(4, nameof(BradixDropdownMenuCheckboxItem.DefaultChecked), BradixCheckboxCheckedState.Indeterminate);
                        menuContent.AddAttribute(5, nameof(BradixDropdownMenuCheckboxItem.CloseOnSelect), false);
                        menuContent.AddAttribute(6, nameof(BradixDropdownMenuCheckboxItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Show bookmarks")));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixDropdownMenuRadioGroup>(7);
                        menuContent.AddAttribute(8, nameof(BradixDropdownMenuRadioGroup.DefaultValue), "name");
                        menuContent.AddAttribute(9, nameof(BradixDropdownMenuRadioGroup.ChildContent), (RenderFragment)(group =>
                        {
                            group.OpenComponent<BradixDropdownMenuRadioItem>(0);
                            group.AddAttribute(1, nameof(BradixDropdownMenuRadioItem.Value), "name");
                            group.AddAttribute(2, nameof(BradixDropdownMenuRadioItem.CloseOnSelect), false);
                            group.AddAttribute(3, nameof(BradixDropdownMenuRadioItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Name")));
                            group.CloseComponent();

                            group.OpenComponent<BradixDropdownMenuRadioItem>(4);
                            group.AddAttribute(5, nameof(BradixDropdownMenuRadioItem.Value), "date");
                            group.AddAttribute(6, nameof(BradixDropdownMenuRadioItem.CloseOnSelect), false);
                            group.AddAttribute(7, nameof(BradixDropdownMenuRadioItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Date modified")));
                            group.CloseComponent();
                        }));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixDropdownMenuSub>(10);
                        menuContent.AddAttribute(11, nameof(BradixDropdownMenuSub.ChildContent), (RenderFragment)(sub =>
                        {
                            sub.OpenComponent<BradixDropdownMenuSubTrigger>(0);
                            sub.AddAttribute(1, nameof(BradixDropdownMenuSubTrigger.TextValue), "Share");
                            sub.AddAttribute(2, nameof(BradixDropdownMenuSubTrigger.ChildContent), (RenderFragment)(item => item.AddContent(0, "Share")));
                            sub.CloseComponent();

                            sub.OpenComponent<BradixDropdownMenuPortal>(3);
                            sub.AddAttribute(4, nameof(BradixDropdownMenuPortal.ChildContent), (RenderFragment)(subPortal =>
                            {
                                subPortal.OpenComponent<BradixDropdownMenuSubContent>(0);
                                subPortal.AddAttribute(1, nameof(BradixDropdownMenuSubContent.ChildContent), (RenderFragment)(submenu =>
                                {
                                    submenu.OpenComponent<BradixDropdownMenuItem>(0);
                                    submenu.AddAttribute(1, nameof(BradixDropdownMenuItem.TextValue), "Copy link");
                                    submenu.AddAttribute(2, nameof(BradixDropdownMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Copy link")));
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
