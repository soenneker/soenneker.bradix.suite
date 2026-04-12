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
        _module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("focusElementPreventScroll", _ => true).SetVoidResult();
        _module.Setup<bool>("isKeyboardInteractionMode", _ => true).SetResult(false);
        _module.Setup<bool>("beginMenuSubmenuPointerGrace", _ => true).SetResult(false);
        _module.SetupVoid("cancelMenuSubmenuPointerGrace", _ => true).SetVoidResult();
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
        Assert.NotNull(menu.GetAttribute("data-radix-menu-content"));
        Assert.Single(cut.FindAll(".menu-arrow-shape"));
    }

    [Fact]
    public void First_enabled_item_has_tab_stop_and_arrow_navigation_updates_current_item()
    {
        var cut = Render(CreateMenu(disableFirstItem: true));
        var items = cut.FindAll("[role='menuitem']");

        Assert.Equal("-1", items[0].GetAttribute("tabindex"));
        Assert.Equal("0", items[1].GetAttribute("tabindex"));

        items[1].KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });

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

        items[0].KeyDown(new KeyboardEventArgs { Key = "b" });

        cut.WaitForAssertion(() =>
        {
            var updatedItems = cut.FindAll("[role='menuitem']");
            Assert.Equal("0", updatedItems[2].GetAttribute("tabindex"));
        });
    }

    [Fact]
    public async Task Focus_outside_resets_typeahead_buffer()
    {
        var cut = Render(CreateMenu());
        var content = cut.FindComponent<BradixMenuContent>();
        string contentId = Assert.IsType<string>(cut.Find("[role='menu']").Id);

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "b",
            TargetId = contentId
        }));

        cut.WaitForAssertion(() =>
        {
            var updatedItems = cut.FindAll("[role='menuitem']");
            Assert.Equal("0", updatedItems[2].GetAttribute("tabindex"));
        });

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentFocusOut(new BradixDelegatedFocusEvent
        {
            TargetId = contentId,
            RelatedTargetId = "outside-target"
        }));

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "a",
            TargetId = contentId
        }));

        cut.WaitForAssertion(() =>
        {
            var updatedItems = cut.FindAll("[role='menuitem']");
            Assert.Equal("0", updatedItems[0].GetAttribute("tabindex"));
        });
    }

    [Fact]
    public async Task Content_root_home_and_end_keys_move_focus_to_first_and_last_items()
    {
        var cut = Render(CreateMenu(disableFirstItem: true));
        var content = cut.FindComponent<BradixMenuContent>();
        string contentId = Assert.IsType<string>(cut.Find("[role='menu']").Id);

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "End",
            TargetId = contentId
        }));

        cut.WaitForAssertion(() =>
        {
            var updatedItems = cut.FindAll("[role='menuitem']");
            Assert.Equal("0", updatedItems[2].GetAttribute("tabindex"));
        });

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "Home",
            TargetId = contentId
        }));

        cut.WaitForAssertion(() =>
        {
            var updatedItems = cut.FindAll("[role='menuitem']");
            Assert.Equal("0", updatedItems[1].GetAttribute("tabindex"));
        });
    }

    [Fact]
    public async Task Content_root_character_key_runs_typeahead_when_content_has_focus()
    {
        var cut = Render(CreateMenu());
        var content = cut.FindComponent<BradixMenuContent>();
        string contentId = Assert.IsType<string>(cut.Find("[role='menu']").Id);

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "b",
            TargetId = contentId
        }));

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
    public void Pointer_up_on_item_after_pointer_down_on_different_item_selects_it()
    {
        var selected = false;
        var cut = Render(CreateMenu(onSelect: () => selected = true));
        var items = cut.FindAll("[role='menuitem']");

        items[0].TriggerEvent("onpointerdown", new PointerEventArgs { PointerType = "mouse", Button = 0 });
        items[2].TriggerEvent("onpointerup", new PointerEventArgs { PointerType = "mouse", Button = 0 });

        cut.WaitForAssertion(() =>
        {
            Assert.True(selected);
            Assert.Equal("closed", cut.Find("[role='menu']").GetAttribute("data-state"));
        });
    }

    [Fact]
    public void Detailed_select_callback_can_prevent_menu_close()
    {
        var selected = false;
        var cut = Render(CreateMenu(
            onSelect: () => selected = true,
            onSelectDetailed: args => args.PreventDefault()));

        cut.FindAll("[role='menuitem']")[2].Click();

        cut.WaitForAssertion(() =>
        {
            Assert.True(selected);
            Assert.Equal("open", cut.Find("[role='menu']").GetAttribute("data-state"));
        });
    }

    [Fact]
    public async Task Detailed_focus_outside_can_prevent_submenu_close()
    {
        var cut = Render(CreateSubmenuMenu(
            onSubFocusOutsideDetailed: EventCallback.Factory.Create<BradixFocusOutsideEventArgs>(this, args => args.PreventDefault())));

        cut.Find("[aria-haspopup='menu'][role='menuitem']").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll("[role='menu']").Count));

        var layer = cut.FindComponents<BradixDismissableLayer>().Last();
        await cut.InvokeAsync(() => layer.Instance.HandleFocusOutside(new BradixDelegatedFocusEvent
        {
            TargetId = "outside-target"
        }));

        Assert.Equal(2, cut.FindAll("[role='menu']").Count);
    }

    [Fact]
    public async Task Submenu_content_keydown_callback_is_invoked()
    {
        string? key = null;

        var cut = Render(CreateSubmenuMenu(
            onSubContentKeyDown: EventCallback.Factory.Create<KeyboardEventArgs>(this, args => key = args.Key)));

        cut.Find("[aria-haspopup='menu'][role='menuitem']").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll("[role='menu']").Count));

        var submenuContent = cut.FindComponents<BradixMenuContent>().Last();
        string submenuContentId = Assert.IsType<string>(cut.FindAll("[role='menu']").Last().Id);

        await cut.InvokeAsync(() => submenuContent.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "x",
            TargetId = submenuContentId
        }));

        Assert.Equal("x", key);
    }

    [Fact]
    public void Modal_menu_registers_scroll_lock_and_hide_others()
    {
        var cut = Render(CreateMenu());

        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "registerRemoveScroll");
        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "registerHideOthers");
        Assert.Null(cut.Find("[role='menu']").GetAttribute("aria-modal"));
    }

    [Fact]
    public async Task Controlled_open_uses_keyboard_focus_path_when_last_input_was_keyboard()
    {
        _module.Setup<bool>("isKeyboardInteractionMode", _ => true).SetResult(true);

        var cut = Render(CreateMenu(open: true));
        int focusCountBefore = _module.Invocations.Count(invocation => invocation.Identifier == "focusElementPreventScroll");
        var focusScope = cut.FindComponent<BradixFocusScope>();
        await cut.InvokeAsync(() => focusScope.Instance.HandleMountAutoFocus());

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("open", cut.Find("[role='menu']").GetAttribute("data-state"));
            Assert.Equal(focusCountBefore + 2, _module.Invocations.Count(invocation => invocation.Identifier == "focusElementPreventScroll"));
        });
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
    public void Checkbox_item_select_can_prevent_close_while_still_updating_checked_state()
    {
        var cut = Render(CreateSelectionMenu(
            onCheckboxSelectDetailed: EventCallback.Factory.Create<BradixMenuItemSelectEventArgs>(this, args => args.PreventDefault())));

        cut.Find("[role='menuitemcheckbox']").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("open", cut.Find("[role='menu']").GetAttribute("data-state"));
            Assert.Equal("checked", cut.Find("[role='menuitemcheckbox']").GetAttribute("data-state"));
        });
    }

    [Fact]
    public void Sub_trigger_opens_submenu_and_exposes_expanded_state()
    {
        var cut = Render(CreateSubmenuMenu());
        var trigger = cut.Find("[aria-haspopup='menu']");

        trigger.KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("true", cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded"));
            Assert.Contains("Copy link", cut.Markup);
        });
    }

    [Fact]
    public async Task Keyboard_opened_submenu_focuses_content_before_first_item()
    {
        var cut = Render(CreateSubmenuMenu());
        var trigger = cut.Find("[aria-haspopup='menu']");
        int focusCountBefore = _module.Invocations.Count(invocation => invocation.Identifier == "focusElementPreventScroll");

        trigger.KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        var focusScope = cut.FindComponents<BradixFocusScope>().Last();
        await cut.InvokeAsync(() => focusScope.Instance.HandleMountAutoFocus());

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("true", cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded"));
            Assert.Equal(focusCountBefore + 2, _module.Invocations.Count(invocation => invocation.Identifier == "focusElementPreventScroll"));
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
    public async Task Space_does_not_open_submenu_while_typeahead_is_active()
    {
        var cut = Render(CreateSubmenuMenu());
        var content = cut.FindComponent<BradixMenuContent>();
        string contentId = Assert.IsType<string>(cut.Find("[role='menu']").Id);

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "s",
            TargetId = contentId
        }));

        cut.Find("[aria-haspopup='menu'][role='menuitem']").KeyDown(new KeyboardEventArgs { Key = " " });

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("false", cut.Find("[aria-haspopup='menu'][role='menuitem']").GetAttribute("aria-expanded"));
            Assert.Single(cut.FindAll("[role='menu']"));
        });
    }

    [Fact]
    public void Pointer_grace_keeps_sibling_hover_from_stealing_focus_while_moving_to_open_submenu()
    {
        _module.Setup<bool>("beginMenuSubmenuPointerGrace", _ => true).SetResult(true);

        var cut = Render(CreateSubmenuMenu());
        var alphaItem = cut.FindAll("[role='menuitem']")[0];
        var subTriggerElement = cut.Find("[aria-haspopup='menu']");

        subTriggerElement.TriggerEvent("onpointermove", new PointerEventArgs { PointerType = "mouse" });
        cut.WaitForAssertion(() =>
        {
            var items = cut.FindAll("[role='menuitem']");
            Assert.Equal("0", items[1].GetAttribute("tabindex"));
        });

        subTriggerElement.Click();
        cut.WaitForAssertion(() => Assert.Equal("true", cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded")));

        subTriggerElement.TriggerEvent("onpointerleave", new PointerEventArgs { PointerType = "mouse", ClientX = 10, ClientY = 10 });
        alphaItem.TriggerEvent("onpointermove", new PointerEventArgs { PointerType = "mouse" });

        cut.WaitForAssertion(() =>
        {
            var items = cut.FindAll("[role='menuitem']");
            Assert.Equal("-1", items[0].GetAttribute("tabindex"));
            Assert.Equal("0", items[1].GetAttribute("tabindex"));
        });
    }

    [Fact]
    public async Task Pointer_grace_exit_restores_normal_parent_menu_hover_behavior()
    {
        _module.Setup<bool>("beginMenuSubmenuPointerGrace", _ => true).SetResult(true);

        var cut = Render(CreateSubmenuMenu());
        var subTrigger = cut.FindComponent<BradixMenuSubTrigger>();
        var alphaItem = cut.FindAll("[role='menuitem']")[0];
        var subTriggerElement = cut.Find("[aria-haspopup='menu']");

        subTriggerElement.TriggerEvent("onpointermove", new PointerEventArgs { PointerType = "mouse" });
        cut.WaitForAssertion(() =>
        {
            var items = cut.FindAll("[role='menuitem']");
            Assert.Equal("0", items[1].GetAttribute("tabindex"));
        });

        subTriggerElement.Click();
        cut.WaitForAssertion(() => Assert.Equal("true", cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded")));

        subTriggerElement.TriggerEvent("onpointerleave", new PointerEventArgs { PointerType = "mouse", ClientX = 10, ClientY = 10 });
        await cut.InvokeAsync(() => subTrigger.Instance.HandlePointerGraceChanged(false));

        alphaItem.TriggerEvent("onpointermove", new PointerEventArgs { PointerType = "mouse" });

        cut.WaitForAssertion(() =>
        {
            var items = cut.FindAll("[role='menuitem']");
            Assert.Equal("0", items[0].GetAttribute("tabindex"));
        });
    }

    [Fact]
    public async Task Submenu_close_key_closes_submenu_and_returns_trigger_state()
    {
        var cut = Render(CreateSubmenuMenu());
        var trigger = cut.Find("[aria-haspopup='menu']");

        trigger.KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        cut.WaitForAssertion(() => Assert.Equal("true", cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded")));

        var submenuContent = cut.FindComponents<BradixMenuContent>().Last();
        string targetId = Assert.IsType<string>(cut.FindAll("[role='menu']").Last().Id);

        await cut.InvokeAsync(() => submenuContent.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "ArrowLeft",
            TargetId = targetId
        }));

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("false", cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded"));
        });
    }

    private RenderFragment CreateMenu(bool includeArrow = false, bool disableFirstItem = false, Action? onSelect = null,
        bool? open = null,
        bool defaultOpen = true,
        Action<BradixMenuItemSelectEventArgs>? onSelectDetailed = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixMenu>(0);
            if (open.HasValue)
                builder.AddAttribute(1, nameof(BradixMenu.Open), open.Value);
            else
                builder.AddAttribute(1, nameof(BradixMenu.DefaultOpen), defaultOpen);
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
                        if (onSelectDetailed is not null)
                        {
                            menuContent.AddAttribute(10, nameof(BradixMenuItem.OnSelectDetailed),
                                EventCallback.Factory.Create<BradixMenuItemSelectEventArgs>(this, onSelectDetailed));
                        }
                        menuContent.AddAttribute(11, nameof(BradixMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Beta")));
                        menuContent.CloseComponent();

                        if (includeArrow)
                        {
                            menuContent.OpenComponent<BradixMenuArrow>(12);
                            menuContent.AddAttribute(13, nameof(BradixMenuArrow.ChildContent), (RenderFragment)(arrow =>
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

    private RenderFragment CreateSelectionMenu(EventCallback<BradixMenuItemSelectEventArgs> onCheckboxSelectDetailed = default)
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
                        menuContent.AddAttribute(1, nameof(BradixMenuCheckboxItem.DefaultChecked), (object) BradixCheckboxCheckedState.Indeterminate);
                        menuContent.AddAttribute(2, nameof(BradixMenuCheckboxItem.TextValue), "Show bookmarks");
                        menuContent.AddAttribute(3, nameof(BradixMenuCheckboxItem.CloseOnSelect), false);
                        if (onCheckboxSelectDetailed.HasDelegate)
                            menuContent.AddAttribute(4, nameof(BradixMenuCheckboxItem.OnSelectDetailed), onCheckboxSelectDetailed);
                        menuContent.AddAttribute(5, nameof(BradixMenuCheckboxItem.ChildContent), (RenderFragment)(item =>
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

    private static RenderFragment CreateSubmenuMenu(
        EventCallback<BradixFocusOutsideEventArgs> onSubFocusOutsideDetailed = default,
        EventCallback<KeyboardEventArgs> onSubContentKeyDown = default)
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
                                if (onSubFocusOutsideDetailed.HasDelegate)
                                    portalContent.AddAttribute(1, nameof(BradixMenuSubContent.OnFocusOutsideDetailed), onSubFocusOutsideDetailed);
                                if (onSubContentKeyDown.HasDelegate)
                                    portalContent.AddAttribute(2, nameof(BradixMenuSubContent.OnContentKeyDown), onSubContentKeyDown);
                                portalContent.AddAttribute(3, nameof(BradixMenuSubContent.ChildContent), (RenderFragment)(submenu =>
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
