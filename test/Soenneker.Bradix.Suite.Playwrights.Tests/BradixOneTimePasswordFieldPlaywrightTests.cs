using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixOneTimePasswordFieldPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixOneTimePasswordFieldPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async ValueTask One_time_password_demo_distributes_typed_digits_across_slots()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/one-time-password-field"));

        ILocator slots = page.Locator(".otp-slot");
        await slots.First.ClickAsync();
        await page.Keyboard.TypeAsync("123456");

        await Assertions.Expect(slots.Nth(0)).ToHaveValueAsync("1");
        await Assertions.Expect(slots.Nth(1)).ToHaveValueAsync("2");
        await Assertions.Expect(slots.Nth(2)).ToHaveValueAsync("3");
        await Assertions.Expect(slots.Nth(3)).ToHaveValueAsync("4");
        await Assertions.Expect(slots.Nth(4)).ToHaveValueAsync("5");
        await Assertions.Expect(slots.Nth(5)).ToHaveValueAsync("6");
    }

    [Test]
    public async ValueTask One_time_password_demo_rejects_non_numeric_characters()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/one-time-password-field"));

        ILocator first = page.Locator(".otp-slot").First;
        await first.ClickAsync();
        await page.Keyboard.TypeAsync("A");

        await Assertions.Expect(first).ToHaveValueAsync(string.Empty);
        await Assertions.Expect(page.Locator("input[type='hidden']").First).ToHaveValueAsync(string.Empty);
    }

[Test]
    public async ValueTask One_time_password_demo_home_and_end_keys_move_focus_to_first_and_last_filled_slots()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/one-time-password-field"));

        ILocator slots = page.Locator(".otp-controlled-slot");
        ILocator first = slots.Nth(0);
        ILocator last = slots.Nth(3);

        await first.ClickAsync();
        await first.PressAsync("End");
        await Assertions.Expect(last).ToBeFocusedAsync();

        await last.PressAsync("Home");
        await Assertions.Expect(first).ToBeFocusedAsync();
    }

[Test]
    public async ValueTask One_time_password_demo_controlled_buttons_keep_visible_slots_and_hidden_input_in_sync()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/one-time-password-field"));

        ILocator controlledSlots = page.Locator(".otp-controlled-slot");

        await Assertions.Expect(controlledSlots.Nth(0)).ToHaveValueAsync("1");
        await Assertions.Expect(controlledSlots.Nth(1)).ToHaveValueAsync("3");
        await Assertions.Expect(controlledSlots.Nth(2)).ToHaveValueAsync("5");
        await Assertions.Expect(controlledSlots.Nth(3)).ToHaveValueAsync("7");
        await Assertions.Expect(page.Locator("#otp-controlled-value")).ToContainTextAsync("Current controlled value: 1357");
        await Assertions.Expect(page.Locator("input[type='hidden'][name='otp-controlled']")).ToHaveValueAsync("1357");

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Fill sample", Exact = true }).ClickAsync();

        await Assertions.Expect(controlledSlots.Nth(0)).ToHaveValueAsync("8");
        await Assertions.Expect(controlledSlots.Nth(1)).ToHaveValueAsync("6");
        await Assertions.Expect(controlledSlots.Nth(2)).ToHaveValueAsync("4");
        await Assertions.Expect(controlledSlots.Nth(3)).ToHaveValueAsync("2");
        await Assertions.Expect(page.Locator("#otp-controlled-value")).ToContainTextAsync("Current controlled value: 8642");
        await Assertions.Expect(page.Locator("input[type='hidden'][name='otp-controlled']")).ToHaveValueAsync("8642");

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Clear", Exact = true }).ClickAsync();

        for (var i = 0; i < 4; i++)
        {
            await Assertions.Expect(controlledSlots.Nth(i)).ToHaveValueAsync(string.Empty);
        }

        await Assertions.Expect(page.Locator("#otp-controlled-value")).ToContainTextAsync("Current controlled value:");
        await Assertions.Expect(page.Locator("input[type='hidden'][name='otp-controlled']")).ToHaveValueAsync(string.Empty);
    }

[Test]
    public async ValueTask One_time_password_demo_backspace_after_paste_clears_uncontrolled_slots_and_form_reset_restores_default_value()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/one-time-password-field"));

        ILocator primarySlots = page.Locator(".otp-slot").Filter(new LocatorFilterOptions { HasNot = page.Locator(".otp-reset-slot") });
        await primarySlots.First.ClickAsync();
        await page.Keyboard.InsertTextAsync("123456");

        for (var i = 0; i < 6; i++)
        {
            await page.Keyboard.PressAsync("Backspace");
        }

        for (var i = 0; i < 6; i++)
        {
            await Assertions.Expect(primarySlots.Nth(i)).ToHaveValueAsync(string.Empty);
        }

        ILocator resetSlots = page.Locator(".otp-reset-slot");
        await Assertions.Expect(page.Locator("input[type='hidden'][name='otp-reset']")).ToHaveValueAsync("2468");
        await Assertions.Expect(resetSlots.Nth(0)).ToHaveValueAsync("2");
        await Assertions.Expect(resetSlots.Nth(1)).ToHaveValueAsync("4");
        await Assertions.Expect(resetSlots.Nth(2)).ToHaveValueAsync("6");
        await Assertions.Expect(resetSlots.Nth(3)).ToHaveValueAsync("8");

        await resetSlots.Nth(3).ClickAsync();
        await page.Keyboard.PressAsync("Backspace");
        await Assertions.Expect(resetSlots.Nth(3)).ToHaveValueAsync(string.Empty);

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Reset OTP", Exact = true }).ClickAsync();

        await Assertions.Expect(resetSlots.Nth(0)).ToHaveValueAsync("2");
        await Assertions.Expect(resetSlots.Nth(1)).ToHaveValueAsync("4");
        await Assertions.Expect(resetSlots.Nth(2)).ToHaveValueAsync("6");
        await Assertions.Expect(resetSlots.Nth(3)).ToHaveValueAsync("8");
        await Assertions.Expect(page.Locator("input[type='hidden'][name='otp-reset']")).ToHaveValueAsync("2468");
    }
}

