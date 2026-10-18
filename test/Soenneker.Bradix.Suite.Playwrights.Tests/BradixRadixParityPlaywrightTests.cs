using System.Threading;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixRadixParityPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixRadixParityPlaywrightTests(BradixPlaywrightHost host) : base(host) { }

    [Test]
    public async ValueTask Dialog_waits_for_click_and_honors_canceled_outside_interactions(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;
        await page.GotoAsync(BaseUrl.TrimEnd('/') + "/dialogs");
        await page.GetByRole(AriaRole.Button, new() { Name = "Edit profile", Exact = true }).ClickAsync();
        ILocator dialog = page.GetByRole(AriaRole.Dialog).First;
        await Assertions.Expect(dialog).ToBeVisibleAsync();
        await Assertions.Expect(page.Locator("[data-bradix-dismissable-layer-ready=true]")).ToHaveCountAsync(1);
        await page.EvaluateAsync("() => document.addEventListener('click', e => e.preventDefault(), {once:true})");
        await page.Mouse.MoveAsync(5, 5);
        await page.Mouse.DownAsync();
        await Assertions.Expect(dialog).ToBeVisibleAsync();
        await page.Mouse.UpAsync();
        await Assertions.Expect(dialog).ToBeVisibleAsync();
        await page.Mouse.ClickAsync(5, 5);
        await Assertions.Expect(dialog).ToBeHiddenAsync();
    }

    [Test]
    public async ValueTask Context_menu_native_bridge_opens_at_pointer_and_supports_keyboard(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;
        await page.GotoAsync(BaseUrl.TrimEnd('/') + "/contextmenus");
        ILocator trigger = page.Locator("[data-bradix-context-menu-trigger]").First;
        await Assertions.Expect(trigger).ToBeVisibleAsync();
        await trigger.ClickAsync(new() { Button = MouseButton.Right });
        await Assertions.Expect(page.GetByRole(AriaRole.Menu).First).ToBeVisibleAsync();
        await page.Keyboard.PressAsync("Escape");
        await Assertions.Expect(page.GetByRole(AriaRole.Menu)).ToHaveCountAsync(0);
        await trigger.FocusAsync();
        await trigger.PressAsync("Shift+F10");
        await Assertions.Expect(page.GetByRole(AriaRole.Menu).First).ToBeVisibleAsync();
    }

    [Test]
    public async ValueTask Password_pointer_keyboard_and_form_submission_behave_natively(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;
        await page.GotoAsync(BaseUrl.TrimEnd('/') + "/passwordtogglefields");
        ILocator input = page.Locator("#password-demo");
        ILocator toggle = page.Locator("#password-toggle");
        await Assertions.Expect(input).ToHaveAttributeAsync("data-bradix-password-ready", "true");
        await input.FillAsync("example password");
        await input.EvaluateAsync("el => el.setSelectionRange(2, 7)");
        await toggle.ClickAsync();
        await Assertions.Expect(input).ToHaveAttributeAsync("type", "text");
        await Assertions.Expect(input).ToHaveValueAsync("example password");
        await Assertions.Expect(input).ToBeFocusedAsync();
        await page.WaitForFunctionAsync("() => document.querySelector('#password-demo').selectionStart === 2");
        await Assert.That(await input.EvaluateAsync<int>("el => el.selectionStart")).IsEqualTo(2);
        await Assert.That(await input.EvaluateAsync<int>("el => el.selectionEnd")).IsEqualTo(7);
        await toggle.FocusAsync();
        await toggle.PressAsync("Space");
        await Assertions.Expect(input).ToHaveAttributeAsync("type", "password");
        await Assertions.Expect(toggle).ToBeFocusedAsync();
        await toggle.PressAsync("Space");
        await Assertions.Expect(input).ToHaveAttributeAsync("type", "text");
        await page.GetByRole(AriaRole.Button, new() { Name = "Submit", Exact = true }).ClickAsync();
        await Assertions.Expect(input).ToHaveAttributeAsync("type", "password");
        await Assertions.Expect(input).ToHaveValueAsync("example password");
    }

    [Test]
    public async ValueTask Ime_keys_are_not_canceled_by_native_bridges(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;
        await page.GotoAsync(BaseUrl.TrimEnd('/') + "/switches");
        await Assertions.Expect(page.Locator("#sync-mode")).ToBeVisibleAsync();
        bool canceled = await page.Locator("#sync-mode").EvaluateAsync<bool>("el => !el.dispatchEvent(new KeyboardEvent('keydown', { key: ' ', code: 'Space', isComposing: true, bubbles: true, cancelable: true }))");
        await Assert.That(canceled).IsFalse();
        await Assertions.Expect(page.Locator("#sync-mode")).ToHaveAttributeAsync("aria-checked", "true");
    }
}
