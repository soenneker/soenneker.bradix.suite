using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixMenuRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixMenuRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
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
        _module.SetupVoid("registerFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("updateFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("registerFocusGuards", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusGuards", _ => true).SetVoidResult();
        _module.SetupVoid("registerHideOthers", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterHideOthers", _ => true).SetVoidResult();
        _module.SetupVoid("registerRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("focusElementPreventScroll", _ => true).SetVoidResult();
        _module.SetupVoid("registerDismissableLayerBranch", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayerBranch", _ => true).SetVoidResult();
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
    }

    [Fact]
    public void Default_open_menu_renders_content_and_arrow()
    {
        var cut = Render(CreateMenu(includeArrow: true));

        var menu = cut.Find("[role='menu']");

        Assert.Equal("open", menu.GetAttribute("data-state"));
        Assert.Equal("vertical", menu.GetAttribute("aria-orientation"));
        Assert.Single(cut.FindAll(".menu-arrow-shape"));
    }

    [Fact]
    public void First_enabled_item_has_tab_stop_and_arrow_navigation_updates_current_item()
    {
        var cut = Render(CreateMenu(disableFirstItem: true));
        var items = cut.FindAll("[role='menuitem']");

        Assert.Equal("-1", items[0].GetAttribute("tabindex"));
        Assert.Equal("0", items[1].GetAttribute("tabindex"));

        items[1].KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowDown" });

        cut.WaitForAssertion(() =>
        {
            var updatedItems = cut.FindAll("[role='menuitem']");
            Assert.Equal("-1", updatedItems[1].GetAttribute("tabindex"));
            Assert.Equal("0", updatedItems[2].GetAttribute("tabindex"));
        });
    }

    [Fact]
    public void Character_key_typeahead_moves_to_next_match()
    {
        var cut = Render(CreateMenu());
        var items = cut.FindAll("[role='menuitem']");

        items[0].KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "b" });

        cut.WaitForAssertion(() =>
        {
            var updatedItems = cut.FindAll("[role='menuitem']");
            Assert.Equal("0", updatedItems[2].GetAttribute("tabindex"));
        });
    }

    [Fact]
    public void Touch_pointer_move_does_not_change_menu_tab_stop()
    {
        var cut = Render(CreateMenu());
        var items = cut.FindAll("[role='menuitem']");

        Assert.Equal("0", items[0].GetAttribute("tabindex"));
        items[2].TriggerEvent("onpointermove", new PointerEventArgs { PointerType = "touch" });

        cut.WaitForAssertion(() =>
        {
            var updatedItems = cut.FindAll("[role='menuitem']");
            Assert.Equal("0", updatedItems[0].GetAttribute("tabindex"));
            Assert.Equal("-1", updatedItems[2].GetAttribute("tabindex"));
        });
    }

    [Fact]
    public void Disabled_item_pointer_move_does_not_take_tab_stop()
    {
        var cut = Render(CreateMenu(disableFirstItem: true));
        var items = cut.FindAll("[role='menuitem']");

        items[0].TriggerEvent("onpointermove", new PointerEventArgs { PointerType = "mouse" });

        cut.WaitForAssertion(() =>
        {
            var updatedItems = cut.FindAll("[role='menuitem']");
            Assert.Equal("-1", updatedItems[0].GetAttribute("tabindex"));
            Assert.Equal("-1", updatedItems[1].GetAttribute("tabindex"));
        });
    }

    [Fact]
    public void Pointer_leave_clears_current_menu_item_tab_stop()
    {
        var cut = Render(CreateMenu());
        var items = cut.FindAll("[role='menuitem']");

        items[2].TriggerEvent("onpointermove", new PointerEventArgs { PointerType = "mouse" });

        cut.WaitForAssertion(() =>
        {
            var updatedItems = cut.FindAll("[role='menuitem']");
            Assert.Equal("0", updatedItems[2].GetAttribute("tabindex"));
        });

        cut.FindAll("[role='menuitem']")[2].TriggerEvent("onpointerleave", new PointerEventArgs { PointerType = "mouse" });

        cut.WaitForAssertion(() =>
        {
            var updatedItems = cut.FindAll("[role='menuitem']");
            Assert.All(updatedItems, item => Assert.Equal("-1", item.GetAttribute("tabindex")));
        });
    }

    [Fact]
    public void Selecting_item_closes_menu_and_invokes_callback()
    {
        var selected = false;
        var cut = Render(CreateMenu(onSelect: () => selected = true));

        cut.FindAll("[role='menuitem']")[2].Click();

        cut.WaitForAssertion(() =>
        {
            Assert.True(selected);
            Assert.Equal("closed", cut.Find("[role='menu']").GetAttribute("data-state"));
        });
    }

    [Fact]
    public void Modal_menu_registers_scroll_lock_and_hide_others()
    {
        _ = Render(CreateMenu());

        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "registerRemoveScroll");
        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "registerHideOthers");
    }

    [Fact]
    public void Checkbox_item_renders_checked_semantics_and_indicator()
    {
        var cut = Render(CreateSelectionMenu());

        var checkboxItem = cut.Find("[role='menuitemcheckbox']");

        Assert.Equal("mixed", checkboxItem.GetAttribute("aria-checked"));
        Assert.Equal("indeterminate", checkboxItem.GetAttribute("data-state"));
        Assert.Equal("indeterminate", cut.Find(".menu-indicator").GetAttribute("data-state"));
    }

    [Fact]
    public void Radio_group_can_change_selected_item_without_closing_when_configured()
    {
        var cut = Render(CreateSelectionMenu());
        var radioItems = cut.FindAll("[role='menuitemradio']");

        radioItems[1].Click();

        cut.WaitForAssertion(() =>
        {
            var updated = cut.FindAll("[role='menuitemradio']");
            Assert.Equal("false", updated[0].GetAttribute("aria-checked"));
            Assert.Equal("true", updated[1].GetAttribute("aria-checked"));
            Assert.Equal("checked", updated[1].GetAttribute("data-state"));
        });
    }

    [Fact]
    public void Sub_trigger_opens_submenu_and_exposes_expanded_state()
    {
        var cut = Render(CreateSubmenuMenu());
        var trigger = cut.Find("[aria-haspopup='menu']");

        trigger.KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowRight" });

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("true", cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded"));
            Assert.Contains("Copy link", cut.Markup);
        });
    }

    [Fact]
    public async Task Sub_trigger_pointer_move_opens_after_delay()
    {
        var cut = Render(CreateSubmenuMenu());
        var trigger = cut.Find("[aria-haspopup='menu']");

        trigger.TriggerEvent("onpointermove", new PointerEventArgs { PointerType = "mouse" });
        Assert.Equal("false", cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded"));

        await Task.Delay(150, Xunit.TestContext.Current.CancellationToken);

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("true", cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded"));
        });
    }

    [Fact]
    public void Submenu_close_key_closes_submenu_and_returns_trigger_state()
    {
        var cut = Render(CreateSubmenuMenu());
        var trigger = cut.Find("[aria-haspopup='menu']");

        trigger.KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowRight" });
        cut.WaitForAssertion(() => Assert.Equal("true", cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded")));

        cut.FindAll("[role='menuitem']").Last().KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowLeft" });

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("false", cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded"));
        });
    }

    private static RenderFragment CreateMenu(bool includeArrow = false, bool disableFirstItem = false, Action? onSelect = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixMenu>(0);
            builder.AddAttribute(1, nameof(BradixMenu.DefaultOpen), true);
            builder.AddAttribute(2, nameof(BradixMenu.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixMenuAnchor>(0);
                content.AddAttribute(1, nameof(BradixMenuAnchor.ChildContent), (RenderFragment)(anchor =>
                {
                    anchor.OpenElement(0, "button");
                    anchor.AddAttribute(1, "type", "button");
                    anchor.AddContent(2, "Trigger");
                    anchor.CloseElement();
                }));
                content.CloseComponent();

                content.OpenComponent<BradixMenuPortal>(2);
                content.AddAttribute(3, nameof(BradixMenuPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixMenuContent>(0);
                    portal.AddAttribute(1, nameof(BradixMenuContent.ChildContent), (RenderFragment)(menuContent =>
                    {
                        menuContent.OpenComponent<BradixMenuItem>(0);
                        menuContent.AddAttribute(1, nameof(BradixMenuItem.TextValue), "Alpha");
                        menuContent.AddAttribute(2, nameof(BradixMenuItem.Disabled), disableFirstItem);
                        menuContent.AddAttribute(3, nameof(BradixMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Alpha")));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixMenuItem>(4);
                        menuContent.AddAttribute(5, nameof(BradixMenuItem.TextValue), "Amber");
                        menuContent.AddAttribute(6, nameof(BradixMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Amber")));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixMenuItem>(7);
                        menuContent.AddAttribute(8, nameof(BradixMenuItem.TextValue), "Beta");
                        menuContent.AddAttribute(9, nameof(BradixMenuItem.OnSelect), EventCallback.Factory.Create(new object(), (Action)(() => onSelect?.Invoke())));
                        menuContent.AddAttribute(10, nameof(BradixMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Beta")));
                        menuContent.CloseComponent();

                        if (includeArrow)
                        {
                            menuContent.OpenComponent<BradixMenuArrow>(11);
                            menuContent.AddAttribute(12, nameof(BradixMenuArrow.ChildContent), (RenderFragment)(arrow =>
                            {
                                arrow.OpenElement(0, "span");
                                arrow.AddAttribute(1, "class", "menu-arrow-shape");
                                arrow.CloseElement();
                            }));
                            menuContent.CloseComponent();
                        }
                    }));
                    portal.CloseComponent();
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }

    private static RenderFragment CreateSelectionMenu()
    {
        return builder =>
        {
            builder.OpenComponent<BradixMenu>(0);
            builder.AddAttribute(1, nameof(BradixMenu.DefaultOpen), true);
            builder.AddAttribute(2, nameof(BradixMenu.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixMenuAnchor>(0);
                content.AddAttribute(1, nameof(BradixMenuAnchor.ChildContent), (RenderFragment)(anchor =>
                {
                    anchor.OpenElement(0, "button");
                    anchor.AddAttribute(1, "type", "button");
                    anchor.AddContent(2, "Trigger");
                    anchor.CloseElement();
                }));
                content.CloseComponent();

                content.OpenComponent<BradixMenuPortal>(2);
                content.AddAttribute(3, nameof(BradixMenuPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixMenuContent>(0);
                    portal.AddAttribute(1, nameof(BradixMenuContent.ChildContent), (RenderFragment)(menuContent =>
                    {
                        menuContent.OpenComponent<BradixMenuCheckboxItem>(0);
                        menuContent.AddAttribute(1, nameof(BradixMenuCheckboxItem.DefaultChecked), BradixCheckboxCheckedState.Indeterminate);
                        menuContent.AddAttribute(2, nameof(BradixMenuCheckboxItem.TextValue), "Show bookmarks");
                        menuContent.AddAttribute(3, nameof(BradixMenuCheckboxItem.CloseOnSelect), false);
                        menuContent.AddAttribute(4, nameof(BradixMenuCheckboxItem.ChildContent), (RenderFragment)(item =>
                        {
                            item.AddContent(0, "Show bookmarks");
                            item.OpenComponent<BradixMenuItemIndicator>(1);
                            item.AddAttribute(2, nameof(BradixMenuItemIndicator.Class), "menu-indicator");
                            item.AddAttribute(3, nameof(BradixMenuItemIndicator.ChildContent), (RenderFragment)(indicator => indicator.AddContent(0, "x")));
                            item.CloseComponent();
                        }));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixMenuRadioGroup>(5);
                        menuContent.AddAttribute(6, nameof(BradixMenuRadioGroup.DefaultValue), "name");
                        menuContent.AddAttribute(7, nameof(BradixMenuRadioGroup.ChildContent), (RenderFragment)(group =>
                        {
                            group.OpenComponent<BradixMenuRadioItem>(0);
                            group.AddAttribute(1, nameof(BradixMenuRadioItem.Value), "name");
                            group.AddAttribute(2, nameof(BradixMenuRadioItem.TextValue), "Name");
                            group.AddAttribute(3, nameof(BradixMenuRadioItem.CloseOnSelect), false);
                            group.AddAttribute(4, nameof(BradixMenuRadioItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Name")));
                            group.CloseComponent();

                            group.OpenComponent<BradixMenuRadioItem>(5);
                            group.AddAttribute(6, nameof(BradixMenuRadioItem.Value), "date");
                            group.AddAttribute(7, nameof(BradixMenuRadioItem.TextValue), "Date modified");
                            group.AddAttribute(8, nameof(BradixMenuRadioItem.CloseOnSelect), false);
                            group.AddAttribute(9, nameof(BradixMenuRadioItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Date modified")));
                            group.CloseComponent();
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

    private static RenderFragment CreateSubmenuMenu()
    {
        return builder =>
        {
            builder.OpenComponent<BradixMenu>(0);
            builder.AddAttribute(1, nameof(BradixMenu.DefaultOpen), true);
            builder.AddAttribute(2, nameof(BradixMenu.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixMenuAnchor>(0);
                content.AddAttribute(1, nameof(BradixMenuAnchor.ChildContent), (RenderFragment)(anchor =>
                {
                    anchor.OpenElement(0, "button");
                    anchor.AddAttribute(1, "type", "button");
                    anchor.AddContent(2, "Trigger");
                    anchor.CloseElement();
                }));
                content.CloseComponent();

                content.OpenComponent<BradixMenuPortal>(2);
                content.AddAttribute(3, nameof(BradixMenuPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixMenuContent>(0);
                    portal.AddAttribute(1, nameof(BradixMenuContent.ChildContent), (RenderFragment)(menuContent =>
                    {
                        menuContent.OpenComponent<BradixMenuItem>(0);
                        menuContent.AddAttribute(1, nameof(BradixMenuItem.TextValue), "Alpha");
                        menuContent.AddAttribute(2, nameof(BradixMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Alpha")));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixMenuSub>(3);
                        menuContent.AddAttribute(4, nameof(BradixMenuSub.ChildContent), (RenderFragment)(sub =>
                        {
                            sub.OpenComponent<BradixMenuSubTrigger>(0);
                            sub.AddAttribute(1, nameof(BradixMenuSubTrigger.TextValue), "Share");
                            sub.AddAttribute(2, nameof(BradixMenuSubTrigger.ChildContent), (RenderFragment)(item => item.AddContent(0, "Share")));
                            sub.CloseComponent();

                            sub.OpenComponent<BradixMenuPortal>(3);
                            sub.AddAttribute(4, nameof(BradixMenuPortal.ChildContent), (RenderFragment)(portalContent =>
                            {
                                portalContent.OpenComponent<BradixMenuSubContent>(0);
                                portalContent.AddAttribute(1, nameof(BradixMenuSubContent.ChildContent), (RenderFragment)(submenu =>
                                {
                                    submenu.OpenComponent<BradixMenuItem>(0);
                                    submenu.AddAttribute(1, nameof(BradixMenuItem.TextValue), "Copy link");
                                    submenu.AddAttribute(2, nameof(BradixMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Copy link")));
                                    submenu.CloseComponent();

                                    submenu.OpenComponent<BradixMenuItem>(3);
                                    submenu.AddAttribute(4, nameof(BradixMenuItem.TextValue), "Email");
                                    submenu.AddAttribute(5, nameof(BradixMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Email")));
                                    submenu.CloseComponent();
                                }));
                                portalContent.CloseComponent();
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
