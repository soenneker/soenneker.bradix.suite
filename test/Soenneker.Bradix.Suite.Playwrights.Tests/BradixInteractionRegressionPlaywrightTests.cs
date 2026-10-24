using System.Threading;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixInteractionRegressionPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixInteractionRegressionPlaywrightTests(BradixPlaywrightHost host) : base(host) { }

    [Test]
    public async ValueTask Empty_collapsible_overrides_inherited_dimensions_and_hidden_content_preserves_measurements(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        await session.Page.GotoAsync(BaseUrl.TrimEnd('/') + "/");
        // Complete initial route focus before injecting an isolated focus test fixture.
        await session.Page.WaitForFunctionAsync("() => document.activeElement?.matches('h1') === true");
        bool correct = await session.Page.EvaluateAsync<bool>("""
            async () => {
                const collapsible = await import('/_content/Soenneker.Bradix.Suite/js/bradix/collapsible.js');
                const parent = document.createElement('div'), child = document.createElement('div');
                parent.style.cssText = '--radix-collapsible-content-height:100px;--radix-collapsible-content-width:100px';
                child.style.cssText = 'width:0;height:0;overflow:hidden';
                parent.append(child); document.body.append(parent);
                try {
                    collapsible.observeCollapsibleContent(child);
                    const empty = getComputedStyle(child).getPropertyValue('--radix-collapsible-content-height') === '0px'
                        && getComputedStyle(child).getPropertyValue('--radix-collapsible-content-width') === '0px';
                    child.style.height = '40px'; child.style.width = '60px';
                    collapsible.observeCollapsibleContent(child);
                    parent.hidden = true;
                    collapsible.observeCollapsibleContent(child);
                    const hidden = child.style.getPropertyValue('--radix-collapsible-content-height') === '40px'
                        && child.style.getPropertyValue('--radix-collapsible-content-width') === '60px';
                    parent.hidden = false; child.style.height = '0px'; child.style.width = '0px';
                    await new Promise(resolve => setTimeout(resolve, 150));
                    return empty && hidden && child.style.getPropertyValue('--radix-collapsible-content-height') === '0px'
                        && child.style.getPropertyValue('--radix-collapsible-content-width') === '0px';
                } finally {
                    collapsible.unobserveCollapsibleContent(child); parent.remove();
                }
            }
            """);
        await Assert.That(correct).IsTrue();
    }

    [Test]
    [Arguments("/selects", "button[role=combobox]")]
    [Arguments("/dropdownmenus", "button[aria-haspopup=menu]")]
    public async ValueTask Disabled_fieldset_blocks_pointer_activation_and_first_legend_remains_enabled(string path, string selector, CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;
        await page.GotoAsync(BaseUrl.TrimEnd('/') + path);
        ILocator trigger = page.Locator(selector).First;
        await Assertions.Expect(trigger).ToBeVisibleAsync();
        await trigger.FocusAsync();
        await trigger.EvaluateAsync("el => { const field = document.createElement('fieldset'); field.id='disabled-test'; field.disabled=true; el.before(field); field.append(el); }");
        await trigger.DispatchEventAsync("pointerdown", new { pointerType = "mouse", button = 0 });
        await page.WaitForTimeoutAsync(200);
        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "false");
        await trigger.EvaluateAsync("el => { const legend = document.createElement('legend'); el.parentElement.prepend(legend); legend.append(el); }");
        await trigger.ClickAsync();
        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "true");
    }

    [Test]
    public async ValueTask Dropdown_virtual_click_opens_and_focuses_first_item(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;
        await page.GotoAsync(BaseUrl.TrimEnd('/') + "/dropdownmenus");
        ILocator trigger = page.Locator("button[aria-haspopup=menu]").First;
        await Assertions.Expect(trigger).ToBeVisibleAsync();
        await trigger.FocusAsync();
        await trigger.EvaluateAsync("el => el.click()");
        await Assertions.Expect(page.GetByRole(AriaRole.Menu)).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByRole(AriaRole.Menuitem, new() { Name = "New Tab", Exact = true })).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("ArrowDown");
        await Assertions.Expect(page.GetByRole(AriaRole.Menuitem, new() { Name = "New Window", Exact = true })).ToBeFocusedAsync();
    }

    [Test]
    public async ValueTask Rapid_radio_navigation_keeps_focus_and_submitted_value_in_sync(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;
        await page.GotoAsync(BaseUrl.TrimEnd('/') + "/radiogroups");
        await page.Locator("#density-default").FocusAsync();
        await page.EvaluateAsync("() => { for(let i=0;i<2;i++) document.activeElement.dispatchEvent(new KeyboardEvent('keydown', {key:'ArrowRight', bubbles:true, cancelable:true})); }");
        await Assertions.Expect(page.Locator("#density-compact")).ToBeFocusedAsync();
        await Assertions.Expect(page.Locator("#density-compact")).ToHaveAttributeAsync("aria-checked", "true");
        await page.WaitForFunctionAsync("() => new FormData(document.querySelector('#density-default').closest('form')).get('view-density') === 'compact'");
    }

    [Test]
    public async ValueTask Dropdown_keyboard_activation_does_not_toggle_twice(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;
        await page.GotoAsync(BaseUrl.TrimEnd('/') + "/dropdownmenus");
        ILocator trigger = page.Locator("button[aria-haspopup=menu]").First;
        await trigger.FocusAsync();
        await trigger.PressAsync("Enter");
        await Assertions.Expect(page.GetByRole(AriaRole.Menuitem, new() { Name = "New Tab", Exact = true })).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("Escape");
        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "false");
        await trigger.PressAsync("Space");
        await Assertions.Expect(page.GetByRole(AriaRole.Menuitem, new() { Name = "New Tab", Exact = true })).ToBeFocusedAsync();
    }

    [Test]
    public async ValueTask Detached_controls_release_delegated_and_roving_handlers(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        await session.Page.GotoAsync(BaseUrl.TrimEnd('/') + "/");
        // Complete initial route focus before injecting an isolated focus test fixture.
        await session.Page.WaitForFunctionAsync("() => document.activeElement?.matches('h1') === true");
        bool released = await session.Page.EvaluateAsync<bool>("""
            async () => {
                const events = await import('/_content/Soenneker.Bradix.Suite/js/bradix/delegatedInteraction.js');
                const roving = await import('/_content/Soenneker.Bradix.Suite/js/bradix/rovingFocus.js');
                const first = document.createElement('button'), second = document.createElement('button');
                for (const b of [first, second]) { b.dataset.bradixRovingGroup='cleanup'; b.dataset.bradixRovingItem=''; }
                document.body.append(first, second);
                let calls = 0;
                events.registerDelegatedInteraction(first, {invokeMethodAsync: () => { calls++; return Promise.resolve(); }}, {click:{method:'Click'}}, 'events-cleanup');
                roving.registerRovingFocusNavigationKeys(first, null, 'roving-cleanup');
                first.remove();
                events.unregisterDelegatedInteraction(null, 'events-cleanup');
                roving.unregisterRovingFocusNavigationKeys(null, 'roving-cleanup');
                second.before(first);
                first.click(); first.focus();
                const canceled = !first.dispatchEvent(new KeyboardEvent('keydown', {key:'ArrowRight', bubbles:true, cancelable:true}));
                await new Promise(resolve => setTimeout(resolve, 30));
                const correct = calls === 0 && !canceled && document.activeElement === first;
                events.unregisterDelegatedInteraction(first); roving.unregisterRovingFocusNavigationKeys(first);
                first.remove(); second.remove();
                return correct;
            }
            """);
        await Assert.That(released).IsTrue();
    }

    [Test]
    public async ValueTask Rapid_roving_keys_advance_for_each_key_and_skip_disabled_fieldsets(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        await session.Page.GotoAsync(BaseUrl.TrimEnd('/') + "/");
        // Complete initial route focus before injecting an isolated focus test fixture.
        await session.Page.WaitForFunctionAsync("() => document.activeElement?.matches('h1') === true");
        string result = await session.Page.EvaluateAsync<string>("""
            async () => {
                const m = await import('/_content/Soenneker.Bradix.Suite/js/bradix/rovingFocus.js');
                const group = document.createElement('div');
                const buttons = Array.from({length:4}, (_,i) => {
                    const b = document.createElement('button'); b.textContent = String(i);
                    b.dataset.bradixRovingGroup = 'regression'; b.dataset.bradixRovingItem = '';
                    b.dataset.bradixRovingLoop = 'true'; group.append(b); return b;
                });
                const disabled = document.createElement('fieldset'); disabled.disabled = true;
                buttons[1].before(disabled); disabled.append(buttons[1]);
                document.body.append(group);
                buttons.forEach(b => m.registerRovingFocusNavigationKeys(b));
                buttons[0].focus();
                for (let i=0;i<2;i++) document.activeElement.dispatchEvent(new KeyboardEvent('keydown', {key:'ArrowRight', bubbles:true, cancelable:true}));
                await new Promise(resolve => setTimeout(resolve, 30));
                const result = document.activeElement.textContent;
                buttons.forEach(b => m.unregisterRovingFocusNavigationKeys(b)); group.remove();
                return result;
            }
            """);
        await Assert.That(result).IsEqualTo("3");
    }
}
