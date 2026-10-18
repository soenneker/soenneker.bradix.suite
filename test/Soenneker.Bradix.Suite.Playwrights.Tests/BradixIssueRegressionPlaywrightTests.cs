using System.Threading;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixIssueRegressionPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixIssueRegressionPlaywrightTests(BradixPlaywrightHost host) : base(host) { }

    [Test]
    public async ValueTask Dropdown_touch_scroll_does_not_open_but_tap_does(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;
        await page.GotoAsync(BaseUrl.TrimEnd('/') + "/dropdownmenus");
        ILocator trigger = page.GetByRole(AriaRole.Button, new() { Name = "Customise options", IncludeHidden = true });
        await Assertions.Expect(trigger).ToBeVisibleAsync();
        await trigger.FocusAsync();
        bool canceled = await trigger.EvaluateAsync<bool>("el => !el.dispatchEvent(new PointerEvent('pointerdown', {pointerId: 7, pointerType: 'touch', button: 0, bubbles: true, cancelable: true}))");
        await trigger.DispatchEventAsync("pointercancel", new { pointerId = 7, pointerType = "touch" });
        await page.WaitForTimeoutAsync(150);
        await Assert.That(canceled).IsFalse();
        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "false");
        await trigger.DispatchEventAsync("pointerdown", new { pointerId = 8, pointerType = "touch", button = 0 });
        await trigger.DispatchEventAsync("pointerup", new { pointerId = 8, pointerType = "touch", button = 0 });
        await trigger.DispatchEventAsync("click", new { button = 0, detail = 1 });
        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "true");
        await page.Keyboard.PressAsync("Escape");
        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "false");
        await trigger.ClickAsync();
        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "true");
    }

    [Test]
    public async ValueTask Nested_controls_can_consume_escape_without_dismissing_dialog(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;
        await page.GotoAsync(BaseUrl.TrimEnd('/') + "/dialogs");
        await page.GetByRole(AriaRole.Button, new() { Name = "Edit profile", Exact = true }).ClickAsync();
        ILocator dialog = page.GetByRole(AriaRole.Dialog).First;
        await Assertions.Expect(page.Locator("[data-bradix-dismissable-layer-ready=true]")).ToHaveCountAsync(1);
        ILocator input = dialog.Locator("input").First;
        await input.EvaluateAsync("el => el.addEventListener('keydown', e => e.preventDefault(), {once:true})");
        await input.PressAsync("Escape");
        await page.WaitForTimeoutAsync(150);
        await Assertions.Expect(dialog).ToBeVisibleAsync();
        await input.PressAsync("Escape");
        await Assertions.Expect(dialog).ToBeHiddenAsync();
    }

    [Test]
    public async ValueTask Escape_dismisses_only_top_nested_overlay(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;
        await page.GotoAsync(BaseUrl.TrimEnd('/') + "/dialogs");
        await page.GetByRole(AriaRole.Button, new() { Name = "Open nested overlay dialog", Exact = true }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Open nested popover", Exact = true }).ClickAsync();
        await Assertions.Expect(page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(2);
        await Assertions.Expect(page.Locator("[data-bradix-dismissable-layer-ready=true]")).ToHaveCountAsync(2);
        await page.Keyboard.PressAsync("Escape");
        await Assertions.Expect(page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(1);
        await Assertions.Expect(page.GetByRole(AriaRole.Dialog, new() { Name = "Nested overlay dialog", Exact = true })).ToBeVisibleAsync();
        await page.Keyboard.PressAsync("Escape");
        await Assertions.Expect(page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);
    }

    [Test]
    public async ValueTask Select_keeps_open_on_keyboard_height_resize_and_honors_canceled_outside_click(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;
        await page.SetViewportSizeAsync(1000, 800);
        await page.GotoAsync(BaseUrl.TrimEnd('/') + "/selects");
        ILocator trigger = page.GetByRole(AriaRole.Combobox).First;
        await trigger.ClickAsync();
        ILocator list = page.GetByRole(AriaRole.Listbox).First;
        await Assertions.Expect(list).ToBeVisibleAsync();
        await Assertions.Expect(page.Locator("[data-bradix-dismissable-layer-ready=true]")).ToHaveCountAsync(1);
        await page.SetViewportSizeAsync(1000, 600);
        await page.WaitForTimeoutAsync(150);
        await Assertions.Expect(list).ToBeVisibleAsync();
        await page.EvaluateAsync("() => document.addEventListener('click', e => e.preventDefault(), {once:true})");
        await page.Mouse.ClickAsync(5, 5);
        await page.WaitForTimeoutAsync(150);
        await Assertions.Expect(list).ToBeVisibleAsync();
        await page.Mouse.ClickAsync(5, 5);
        await Assertions.Expect(list).ToBeHiddenAsync();
        await trigger.ClickAsync();
        await Assertions.Expect(list).ToBeVisibleAsync();
        await page.SetViewportSizeAsync(900, 600);
        await Assertions.Expect(list).ToBeHiddenAsync();
    }

    [Test]
    public async ValueTask Non_modal_popover_does_not_loop_tab_at_last_control(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;
        await page.GotoAsync(BaseUrl.TrimEnd('/') + "/popovers");
        await page.GetByRole(AriaRole.Button, new() { Name = "Update dimensions", Exact = true }).ClickAsync();
        ILocator dialog = page.GetByRole(AriaRole.Dialog).First;
        await Assertions.Expect(dialog).ToBeVisibleAsync();
        ILocator close = dialog.GetByRole(AriaRole.Button, new() { Name = "Close", Exact = true });
        await close.FocusAsync();
        bool canceled = await close.EvaluateAsync<bool>("el => !el.dispatchEvent(new KeyboardEvent('keydown', {key:'Tab', bubbles:true, cancelable:true}))");
        await Assert.That(canceled).IsFalse();
        await Assertions.Expect(close).ToBeFocusedAsync();
    }

    [Test]
    public async ValueTask Scroll_lock_preserves_stable_gutter_and_existing_padding(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        await session.Page.GotoAsync(BaseUrl.TrimEnd('/') + "/");
        bool correct = await session.Page.EvaluateAsync<bool>("""
            async () => {
                const m = await import('/_content/Soenneker.Bradix.Suite/js/bradix/presenceOverlay.js');
                document.documentElement.style.scrollbarGutter = 'stable';
                document.body.style.paddingRight = '13px';
                const before = document.body.getBoundingClientRect().width;
                m.registerRemoveScroll('outer', true);
                m.registerRemoveScroll('inner', true);
                const during = getComputedStyle(document.body).paddingRight;
                m.unregisterRemoveScroll('outer');
                const stillLocked = document.body.style.overflow === 'hidden';
                m.unregisterRemoveScroll('inner');
                return during === '13px' && stillLocked && document.body.style.paddingRight === '13px'
                    && Math.abs(document.body.getBoundingClientRect().width - before) < 1;
            }
            """);
        await Assert.That(correct).IsTrue();
    }

    [Test]
    public async ValueTask Toast_lost_capture_and_window_blur_cancel_swipe_once(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        await session.Page.GotoAsync(BaseUrl.TrimEnd('/') + "/");
        bool correct = await session.Page.EvaluateAsync<bool>("""
            async () => {
                const m = await import('/_content/Soenneker.Bradix.Suite/js/bradix/toast.js');
                const toast = document.createElement('div');
                document.body.append(toast);
                const completions = [];
                m.registerToastSwipe(toast, 'right', 50, false, false, {
                    invokeMethodAsync: (method, ...args) => { completions.push(args); return Promise.resolve(); }
                }, 'swipe-test');
                const start = id => {
                    toast.dispatchEvent(new PointerEvent('pointerdown', {pointerId:id, button:0, clientX:0}));
                    toast.dispatchEvent(new PointerEvent('pointermove', {pointerId:id, buttons:1, clientX:25}));
                };
                start(1);
                toast.dispatchEvent(new PointerEvent('lostpointercapture', {pointerId:1}));
                const canceled = toast.dataset.swipe === 'cancel' && !toast.style.getPropertyValue('--radix-toast-swipe-move-x');
                start(2);
                window.dispatchEvent(new Event('blur'));
                toast.dispatchEvent(new PointerEvent('pointerup', {pointerId:2}));
                start(3);
                toast.dispatchEvent(new PointerEvent('pointermove', {pointerId:3, pointerType:'mouse', buttons:0, clientX:25}));
                start(4);
                toast.remove();
                m.unregisterToastSwipe(null, 'swipe-test');
                window.dispatchEvent(new Event('blur'));
                return canceled && completions.length === 3 && completions.every(args => args[0] === false);
            }
            """);
        await Assert.That(correct).IsTrue();
    }

    [Test]
    public async ValueTask Detached_overlay_cleanup_restores_accessibility_and_focus(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        await session.Page.GotoAsync(BaseUrl.TrimEnd('/') + "/");
        bool correct = await session.Page.EvaluateAsync<bool>("""
            async () => {
                const presence = await import('/_content/Soenneker.Bradix.Suite/js/bradix/presenceOverlay.js');
                const focus = await import('/_content/Soenneker.Bradix.Suite/js/bradix/focusScope.js');
                const outside = document.createElement('button'); outside.setAttribute('aria-hidden', 'false');
                const content = document.createElement('div'); content.tabIndex = -1;
                document.body.append(outside, content);
                outside.focus();
                await focus.registerFocusScope(content, null, true, true, false, false, false, 'detached-focus');
                presence.registerHideOthers(content, 'detached-modal');
                const hidden = outside.inert && outside.getAttribute('aria-hidden') === 'true';
                content.remove();
                presence.unregisterHideOthers(null, 'detached-modal');
                await focus.unregisterFocusScope(null, false, 'detached-focus');
                await new Promise(resolve => setTimeout(resolve, 30));
                const restored = !outside.inert && outside.getAttribute('aria-hidden') === 'false'
                    && document.activeElement === outside;
                outside.remove();
                return hidden && restored;
            }
            """);
        await Assert.That(correct).IsTrue();
    }

    [Test]
    public async ValueTask Closing_old_focus_scope_does_not_steal_focus_from_new_scope(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        await session.Page.GotoAsync(BaseUrl.TrimEnd('/') + "/");
        bool correct = await session.Page.EvaluateAsync<bool>("""
            async () => {
                const m = await import('/_content/Soenneker.Bradix.Suite/js/bradix/focusScope.js');
                const trigger = document.createElement('button');
                const old = document.createElement('div'); old.tabIndex = -1;
                const next = document.createElement('div'); next.tabIndex = -1;
                const input = document.createElement('input'); next.append(input);
                document.body.append(trigger, old, next);
                trigger.focus();
                await m.registerFocusScope(old, null, false, false, false, false, false);
                await m.unregisterFocusScope(old);
                await m.registerFocusScope(next, null, false, false, false, false, false);
                input.focus();
                await new Promise(resolve => setTimeout(resolve, 30));
                const retained = document.activeElement === input;
                await m.unregisterFocusScope(next, true);
                trigger.remove(); old.remove(); next.remove();
                return retained;
            }
            """);
        await Assert.That(correct).IsTrue();
    }
}
