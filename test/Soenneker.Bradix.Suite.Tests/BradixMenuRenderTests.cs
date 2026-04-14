using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

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
    }

    [Test]
    public async Task Default_open_menu_renders_content_and_arrow()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenu(includeArrow: true));

        IElement menu = cut.Find("[role='menu']");

        await Assert.That(menu.GetAttribute("data-state")).IsEqualTo("open");
        await Assert.That(menu.GetAttribute("aria-orientation")).IsEqualTo("vertical");
        await Assert.That(menu.GetAttribute("data-radix-menu-content")).IsNotNull();
        await Assert.That(cut.FindAll(".menu-arrow-shape")).HasSingleItem();
    }

    [Test]
    public async Task First_enabled_item_has_tab_stop_and_arrow_navigation_updates_current_item()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenu(disableFirstItem: true));
        IReadOnlyList<IElement> items = cut.FindAll("[role='menuitem']");

        await Assert.That(items[0].GetAttribute("tabindex")).IsEqualTo("-1");
        await Assert.That(items[1].GetAttribute("tabindex")).IsEqualTo("0");

        await items[1].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedItems = cut.FindAll("[role='menuitem']");
            await Assert.That(updatedItems[1].GetAttribute("tabindex")).IsEqualTo("-1");
            await Assert.That(updatedItems[2].GetAttribute("tabindex")).IsEqualTo("0");
        });
    }

    [Test]
    public async Task Character_key_typeahead_moves_to_next_match()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenu());
        IReadOnlyList<IElement> items = cut.FindAll("[role='menuitem']");

        await items[0].KeyDownAsync(new KeyboardEventArgs { Key = "b" });

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedItems = cut.FindAll("[role='menuitem']");
            await Assert.That(updatedItems[2].GetAttribute("tabindex")).IsEqualTo("0");
        });
    }

    [Test]
    public async Task Focus_outside_resets_typeahead_buffer()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenu());
        IRenderedComponent<BradixMenuContent> content = cut.FindComponent<BradixMenuContent>();
        string contentId = await Assert.That(cut.Find("[role='menu']").Id).IsTypeOf<string>();

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "b",
            TargetId = contentId
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedItems = cut.FindAll("[role='menuitem']");
            await Assert.That(updatedItems[2].GetAttribute("tabindex")).IsEqualTo("0");
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

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedItems = cut.FindAll("[role='menuitem']");
            await Assert.That(updatedItems[0].GetAttribute("tabindex")).IsEqualTo("0");
        });
    }

    [Test]
    public async Task Content_root_home_and_end_keys_move_focus_to_first_and_last_items()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenu(disableFirstItem: true));
        IRenderedComponent<BradixMenuContent> content = cut.FindComponent<BradixMenuContent>();
        string contentId = await Assert.That(cut.Find("[role='menu']").Id).IsTypeOf<string>();

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "End",
            TargetId = contentId
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedItems = cut.FindAll("[role='menuitem']");
            await Assert.That(updatedItems[2].GetAttribute("tabindex")).IsEqualTo("0");
        });

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "Home",
            TargetId = contentId
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedItems = cut.FindAll("[role='menuitem']");
            await Assert.That(updatedItems[1].GetAttribute("tabindex")).IsEqualTo("0");
        });
    }

    [Test]
    public async Task Content_root_character_key_runs_typeahead_when_content_has_focus()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenu());
        IRenderedComponent<BradixMenuContent> content = cut.FindComponent<BradixMenuContent>();
        string contentId = await Assert.That(cut.Find("[role='menu']").Id).IsTypeOf<string>();

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "b",
            TargetId = contentId
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedItems = cut.FindAll("[role='menuitem']");
            await Assert.That(updatedItems[2].GetAttribute("tabindex")).IsEqualTo("0");
        });
    }

    [Test]
    public async Task Touch_pointer_move_does_not_change_menu_tab_stop()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenu());
        IReadOnlyList<IElement> items = cut.FindAll("[role='menuitem']");

        await Assert.That(items[0].GetAttribute("tabindex")).IsEqualTo("0");
        await items[2].TriggerEventAsync("onpointermove", new PointerEventArgs { PointerType = "touch" });

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedItems = cut.FindAll("[role='menuitem']");
            await Assert.That(updatedItems[0].GetAttribute("tabindex")).IsEqualTo("0");
            await Assert.That(updatedItems[2].GetAttribute("tabindex")).IsEqualTo("-1");
        });
    }

    [Test]
    public async Task Disabled_item_pointer_move_does_not_take_tab_stop()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenu(disableFirstItem: true));
        IReadOnlyList<IElement> items = cut.FindAll("[role='menuitem']");

        await items[0].TriggerEventAsync("onpointermove", new PointerEventArgs { PointerType = "mouse" });

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedItems = cut.FindAll("[role='menuitem']");
            await Assert.That(updatedItems[0].GetAttribute("tabindex")).IsEqualTo("-1");
            await Assert.That(updatedItems[1].GetAttribute("tabindex")).IsEqualTo("-1");
        });
    }

    [Test]
    public async Task Pointer_leave_clears_current_menu_item_tab_stop()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenu());
        IReadOnlyList<IElement> items = cut.FindAll("[role='menuitem']");

        await items[2].TriggerEventAsync("onpointermove", new PointerEventArgs { PointerType = "mouse" });

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedItems = cut.FindAll("[role='menuitem']");
            await Assert.That(updatedItems[2].GetAttribute("tabindex")).IsEqualTo("0");
        });

        await cut.FindAll("[role='menuitem']")[2].TriggerEventAsync("onpointerleave", new PointerEventArgs { PointerType = "mouse" });

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedItems = cut.FindAll("[role='menuitem']");
            await Assert.That(updatedItems[2].GetAttribute("tabindex")).IsEqualTo("-1");
        });
    }

    [Test]
    public async Task Selecting_item_closes_menu_and_invokes_callback()
    {
        var selected = false;
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenu(onSelect: () => selected = true));

        await cut.FindAll("[role='menuitem']")[2].ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(selected).IsTrue();
            await Assert.That(cut.Find("[role='menu']").GetAttribute("data-state")).IsEqualTo("closed");
        });
    }

    [Test]
    public async Task Pointer_up_on_item_after_pointer_down_on_different_item_selects_it()
    {
        var selected = false;
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenu(onSelect: () => selected = true));
        IReadOnlyList<IElement> items = cut.FindAll("[role='menuitem']");

        await items[0].TriggerEventAsync("onpointerdown", new PointerEventArgs { PointerType = "mouse", Button = 0 });
        await items[2].TriggerEventAsync("onpointerup", new PointerEventArgs { PointerType = "mouse", Button = 0 });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(selected).IsTrue();
            await Assert.That(cut.Find("[role='menu']").GetAttribute("data-state")).IsEqualTo("closed");
        });
    }

    [Test]
    public async Task Detailed_select_callback_can_prevent_menu_close()
    {
        var selected = false;
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenu(
            onSelect: () => selected = true,
            onSelectDetailed: args => args.PreventDefault()));

        await cut.FindAll("[role='menuitem']")[2].ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(selected).IsTrue();
            await Assert.That(cut.Find("[role='menu']").GetAttribute("data-state")).IsEqualTo("open");
        });
    }

    [Test]
    public async Task Detailed_focus_outside_can_prevent_submenu_close()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSubmenuMenu(
            onSubFocusOutsideDetailed: EventCallback.Factory.Create<BradixFocusOutsideEventArgs>(this, args => args.PreventDefault())));

        await cut.Find("[aria-haspopup='menu'][role='menuitem']").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });
        await Assert.That(cut.FindAll("[role='menu']").Count).IsEqualTo(2);

        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponents<BradixDismissableLayer>().Last();
        await cut.InvokeAsync(() => layer.Instance.HandleFocusOutside(new BradixDelegatedFocusEvent
        {
            TargetId = "outside-target"
        }));

        await Assert.That(cut.FindAll("[role='menu']").Count).IsEqualTo(2);
    }

    [Test]
    public async Task Submenu_content_keydown_callback_is_invoked()
    {
        string? key = null;

        IRenderedComponent<ContainerFragment> cut = Render(CreateSubmenuMenu(
            onSubContentKeyDown: EventCallback.Factory.Create<KeyboardEventArgs>(this, args => key = args.Key)));

        await cut.Find("[aria-haspopup='menu'][role='menuitem']").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });
        await Assert.That(cut.FindAll("[role='menu']").Count).IsEqualTo(2);

        IRenderedComponent<BradixMenuContent> submenuContent = cut.FindComponents<BradixMenuContent>().Last();
        string submenuContentId = await Assert.That(cut.FindAll("[role='menu']").Last().Id).IsTypeOf<string>();

        await cut.InvokeAsync(() => submenuContent.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "x",
            TargetId = submenuContentId
        }));

        await Assert.That(key).IsEqualTo("x");
    }

    [Test]
    public async Task Modal_menu_registers_scroll_lock_and_hide_others()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenu());

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerRemoveScroll")).IsTrue();
        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerHideOthers")).IsTrue();
        await Assert.That(cut.Find("[role='menu']").GetAttribute("aria-modal")).IsNull();
    }

    [Test]
    public async Task Controlled_open_uses_keyboard_focus_path_when_last_input_was_keyboard()
    {
        _module.Setup<bool>("isKeyboardInteractionMode", _ => true).SetResult(true);

        IRenderedComponent<ContainerFragment> cut = Render(CreateMenu(open: true));
        int focusCountBefore = _module.Invocations.Count(invocation => invocation.Identifier == "focusElementPreventScroll");
        IRenderedComponent<BradixFocusScope> focusScope = cut.FindComponent<BradixFocusScope>();
        await cut.InvokeAsync(() => focusScope.Instance.HandleMountAutoFocus());

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("[role='menu']").GetAttribute("data-state")).IsEqualTo("open");
            await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "focusElementPreventScroll")).IsEqualTo(focusCountBefore + 2);
        });
    }

    [Test]
    public async Task Checkbox_item_renders_checked_semantics_and_indicator()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelectionMenu());

        IElement checkboxItem = cut.Find("[role='menuitemcheckbox']");

        await Assert.That(checkboxItem.GetAttribute("aria-checked")).IsEqualTo("mixed");
        await Assert.That(checkboxItem.GetAttribute("data-state")).IsEqualTo("indeterminate");
        await Assert.That(cut.Find(".menu-indicator").GetAttribute("data-state")).IsEqualTo("indeterminate");
    }

    [Test]
    public async Task Radio_group_can_change_selected_item_without_closing_when_configured()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelectionMenu());
        IReadOnlyList<IElement> radioItems = cut.FindAll("[role='menuitemradio']");

        await radioItems[1].ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updated = cut.FindAll("[role='menuitemradio']");
            await Assert.That(updated[0].GetAttribute("aria-checked")).IsEqualTo("false");
            await Assert.That(updated[1].GetAttribute("aria-checked")).IsEqualTo("true");
            await Assert.That(updated[1].GetAttribute("data-state")).IsEqualTo("checked");
        });
    }

    [Test]
    public async Task Checkbox_item_select_can_prevent_close_while_still_updating_checked_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelectionMenu(
            onCheckboxSelectDetailed: EventCallback.Factory.Create<BradixMenuItemSelectEventArgs>(this, args => args.PreventDefault())));

        await cut.Find("[role='menuitemcheckbox']").ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("[role='menu']").GetAttribute("data-state")).IsEqualTo("open");
            await Assert.That(cut.Find("[role='menuitemcheckbox']").GetAttribute("data-state")).IsEqualTo("checked");
        });
    }

    [Test]
    public async Task Sub_trigger_opens_submenu_and_exposes_expanded_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSubmenuMenu());
        IElement trigger = cut.Find("[aria-haspopup='menu']");

        await trigger.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(cut.Markup).Contains("Copy link");
        });
    }

    [Test]
    public async Task Keyboard_opened_submenu_focuses_content_before_first_item()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSubmenuMenu());
        IElement trigger = cut.Find("[aria-haspopup='menu']");
        int focusCountBefore = _module.Invocations.Count(invocation => invocation.Identifier == "focusElementPreventScroll");

        await trigger.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });
        IRenderedComponent<BradixFocusScope> focusScope = cut.FindComponents<BradixFocusScope>().Last();
        await cut.InvokeAsync(() => focusScope.Instance.HandleMountAutoFocus());

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "focusElementPreventScroll")).IsEqualTo(focusCountBefore + 2);
        });
    }

    [Test]
    public async Task Sub_trigger_pointer_move_opens_after_delay()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSubmenuMenu());
        IElement trigger = cut.Find("[aria-haspopup='menu']");

        await trigger.TriggerEventAsync("onpointermove", new PointerEventArgs { PointerType = "mouse" });
        await Assert.That(cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded")).IsEqualTo("false");

        await Task.Delay(150, global::TUnit.Core.TestContext.Current.Execution.CancellationToken);

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded")).IsEqualTo("true");
        });
    }

    [Test]
    public async Task Space_does_not_open_submenu_while_typeahead_is_active()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSubmenuMenu());
        IRenderedComponent<BradixMenuContent> content = cut.FindComponent<BradixMenuContent>();
        string contentId = await Assert.That(cut.Find("[role='menu']").Id).IsTypeOf<string>();

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "s",
            TargetId = contentId
        }));

        await cut.Find("[aria-haspopup='menu'][role='menuitem']").KeyDownAsync(new KeyboardEventArgs { Key = " " });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("[aria-haspopup='menu'][role='menuitem']").GetAttribute("aria-expanded")).IsEqualTo("false");
            await Assert.That(cut.FindAll("[role='menu']")).HasSingleItem();
        });
    }

    [Test]
    public async Task Pointer_grace_keeps_sibling_hover_from_stealing_focus_while_moving_to_open_submenu()
    {
        _module.Setup<bool>("beginMenuSubmenuPointerGrace", _ => true).SetResult(true);

        IRenderedComponent<ContainerFragment> cut = Render(CreateSubmenuMenu());
        IElement alphaItem = cut.FindAll("[role='menuitem']")[0];
        IElement subTriggerElement = cut.Find("[aria-haspopup='menu']");

        await subTriggerElement.TriggerEventAsync("onpointermove", new PointerEventArgs { PointerType = "mouse" });
        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> items = cut.FindAll("[role='menuitem']");
            await Assert.That(items[1].GetAttribute("tabindex")).IsEqualTo("0");
        });

        await subTriggerElement.ClickAsync();
        await Assert.That(cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded")).IsEqualTo("true");

        await subTriggerElement.TriggerEventAsync("onpointerleave", new PointerEventArgs { PointerType = "mouse", ClientX = 10, ClientY = 10 });
        await alphaItem.TriggerEventAsync("onpointermove", new PointerEventArgs { PointerType = "mouse" });

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> items = cut.FindAll("[role='menuitem']");
            await Assert.That(items[0].GetAttribute("tabindex")).IsEqualTo("-1");
            await Assert.That(items[1].GetAttribute("tabindex")).IsEqualTo("0");
        });
    }

    [Test]
    public async Task Pointer_grace_exit_restores_normal_parent_menu_hover_behavior()
    {
        _module.Setup<bool>("beginMenuSubmenuPointerGrace", _ => true).SetResult(true);

        IRenderedComponent<ContainerFragment> cut = Render(CreateSubmenuMenu());
        IRenderedComponent<BradixMenuSubTrigger> subTrigger = cut.FindComponent<BradixMenuSubTrigger>();
        IElement alphaItem = cut.FindAll("[role='menuitem']")[0];
        IElement subTriggerElement = cut.Find("[aria-haspopup='menu']");

        await subTriggerElement.TriggerEventAsync("onpointermove", new PointerEventArgs { PointerType = "mouse" });
        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> items = cut.FindAll("[role='menuitem']");
            await Assert.That(items[1].GetAttribute("tabindex")).IsEqualTo("0");
        });

        await subTriggerElement.ClickAsync();
        await Assert.That(cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded")).IsEqualTo("true");

        await subTriggerElement.TriggerEventAsync("onpointerleave", new PointerEventArgs { PointerType = "mouse", ClientX = 10, ClientY = 10 });
        await cut.InvokeAsync(() => subTrigger.Instance.HandlePointerGraceChanged(false));

        await alphaItem.TriggerEventAsync("onpointermove", new PointerEventArgs { PointerType = "mouse" });

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> items = cut.FindAll("[role='menuitem']");
            await Assert.That(items[0].GetAttribute("tabindex")).IsEqualTo("0");
        });
    }

    [Test]
    public async Task Submenu_close_key_closes_submenu_and_returns_trigger_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSubmenuMenu());
        IElement trigger = cut.Find("[aria-haspopup='menu']");

        await trigger.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });
        await Assert.That(cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded")).IsEqualTo("true");

        IRenderedComponent<BradixMenuContent> submenuContent = cut.FindComponents<BradixMenuContent>().Last();
        string targetId = await Assert.That(cut.FindAll("[role='menu']").Last().Id).IsTypeOf<string>();

        await cut.InvokeAsync(() => submenuContent.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "ArrowLeft",
            TargetId = targetId
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("[aria-haspopup='menu']").GetAttribute("aria-expanded")).IsEqualTo("false");
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