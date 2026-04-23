using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixAlertDialogPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixAlertDialogPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async ValueTask Alert_dialog_demo_opens_and_closes_from_cancel()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/alert-dialog"));

        ILocator trigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Delete account", Exact = true });
        await trigger.ClickAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Are you absolutely sure?", Exact = true })).ToBeVisibleAsync();
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Cancel", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Are you absolutely sure?", Exact = true })).Not.ToBeVisibleAsync();
        await Assertions.Expect(trigger).ToBeFocusedAsync();
    }

[Test]
    public async ValueTask Alert_dialog_demo_can_disable_escape_dismissal()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/alert-dialog"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Guard escape dismissal", Exact = true }).ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Alertdialog, new PageGetByRoleOptions { Name = "Stay open on escape", Exact = true });
        await Assertions.Expect(dialog).ToHaveAttributeAsync("data-state", "open");

        await page.Keyboard.PressAsync("Escape");

        await Assertions.Expect(dialog).ToHaveAttributeAsync("data-state", "open");

        await dialog.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Cancel", Exact = true }).ClickAsync();

        await Assertions.Expect(dialog).Not.ToBeVisibleAsync();
    }

[Test]
    public async ValueTask Alert_dialog_demo_action_closes_and_restores_trigger_focus()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/alert-dialog"));

        ILocator trigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Delete account", Exact = true });
        await trigger.ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Alertdialog, new PageGetByRoleOptions { Name = "Are you absolutely sure?", Exact = true });
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        await dialog.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Yes, delete account", Exact = true }).ClickAsync();

        await Assertions.Expect(dialog).Not.ToBeVisibleAsync();
        await Assertions.Expect(trigger).ToBeFocusedAsync();
    }

[Test]
    public async ValueTask Alert_dialog_demo_does_not_dismiss_from_outside_click_and_focuses_cancel_by_default()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/alert-dialog"));

        ILocator trigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Delete account", Exact = true });
        await trigger.ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Alertdialog, new PageGetByRoleOptions { Name = "Are you absolutely sure?", Exact = true });
        ILocator cancel = dialog.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Cancel", Exact = true });
        ILocator action = dialog.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Yes, delete account", Exact = true });

        await Assertions.Expect(dialog).ToBeVisibleAsync();
        string activeElementHtml = await page.EvaluateAsync<string>("() => document.activeElement?.outerHTML ?? 'null'");
        Xunit.Assert.True(await cancel.EvaluateAsync<bool>("element => document.activeElement === element"),
            $"Expected cancel button to be focused. Active element was: {activeElementHtml}");

        await page.Keyboard.PressAsync("Shift+Tab");
        await Assertions.Expect(action).ToBeFocusedAsync();

        await page.Keyboard.PressAsync("Tab");
        await Assertions.Expect(cancel).ToBeFocusedAsync();

        await ClickJustOutsideActiveDialogAsync(page, dialog);

        await Assertions.Expect(dialog).ToBeVisibleAsync();
        await Assertions.Expect(dialog).ToHaveAttributeAsync("data-state", "open");
        await Assertions.Expect(cancel).ToBeFocusedAsync();
    }
}

