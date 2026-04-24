using Soenneker.Playwrights.Extensions.TestPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[NotInParallel]
[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixDialogPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixDialogPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async ValueTask Dialog_demo_saves_updated_project_details()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady(
            $"{BaseUrl}dialog",
            static p => p.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true }),
            expectedTitle: "Bradix Dialog");

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Edit profile", Exact = true })).ToBeVisibleAsync();

        ILocator nameInput = page.Locator("#dialog-name");
        ILocator usernameInput = page.Locator("#dialog-username");
        await nameInput.ClearAsync();
        await nameInput.PressSequentiallyAsync("Jake");
        await usernameInput.ClearAsync();
        await usernameInput.PressSequentiallyAsync("@jake");
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save changes", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Edit profile", Exact = true })).Not.ToBeVisibleAsync();

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true }).ClickAsync();
        await Assertions.Expect(nameInput).ToHaveValueAsync("Jake");
        await Assertions.Expect(usernameInput).ToHaveValueAsync("@jake");
    }

[Test]
    public async ValueTask Dialog_demo_traps_focus_and_restores_trigger_focus_after_escape()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;
        var consoleMessages = new List<string>();
        var pageErrors = new List<string>();

        page.Console += (_, message) =>
        {
            if (message.Type is "error" or "warning")
                consoleMessages.Add($"{message.Type}: {message.Text}");
        };
        page.PageError += (_, exception) => pageErrors.Add(exception);

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dialog"));

        ILocator trigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true });
        await trigger.ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Edit profile", Exact = true });

        await Assertions.Expect(dialog).ToBeVisibleAsync();
        await Assertions.Expect(dialog).ToHaveAttributeAsync("aria-modal", "true");
        await Assert.That(await WaitForDialogTabBoundaryAsync(dialog, first: true)).IsTrue();

        await page.Keyboard.PressAsync("Shift+Tab");
        await Assert.That(await WaitForDialogTabBoundaryAsync(dialog, first: false)).IsTrue();

        await page.Keyboard.PressAsync("Tab");
        await Assert.That(await WaitForDialogTabBoundaryAsync(dialog, first: true)).IsTrue();

        await page.Keyboard.PressAsync("Escape");

        await Assertions.Expect(dialog).Not.ToBeVisibleAsync();
        await Assertions.Expect(trigger).ToBeFocusedAsync();
        await Assert.That(pageErrors).IsEmpty();
        await Assert.That(consoleMessages).IsEmpty();
    }

[Test]
    public async ValueTask Dialog_demo_supports_popover_nested_inside_modal_dialog()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dialog"));

        ILocator dialogTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open nested overlay dialog", Exact = true });
        await dialogTrigger.ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Nested overlay dialog", Exact = true });
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        ILocator popoverTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open nested popover", Exact = true });
        await popoverTrigger.ClickAsync();

        ILocator popover = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Nested popover", Exact = true });
        await Assertions.Expect(popoverTrigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(popover).ToBeVisibleAsync();

        await page.Keyboard.PressAsync("Escape");

        await Assertions.Expect(popover).Not.ToBeVisibleAsync();
        await Assertions.Expect(dialog).ToBeVisibleAsync();
        await Assertions.Expect(popoverTrigger).ToBeFocusedAsync();

        await page.Keyboard.PressAsync("Escape");

        await Assertions.Expect(dialog).Not.ToBeVisibleAsync();
        await Assertions.Expect(dialogTrigger).ToBeFocusedAsync();
    }

[Test]
    public async ValueTask Dialog_demo_close_discards_unsaved_changes()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dialog"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true }).ClickAsync();
        await page.Locator("#dialog-name").FillAsync("Unsaved");
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Close", Exact = true }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true }).ClickAsync();
        await Assertions.Expect(page.Locator("#dialog-name")).ToHaveValueAsync("Pedro Duarte");
    }

[Test]
    public async ValueTask Dialog_demo_dismisses_from_overlay_click()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dialog"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true }).ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Edit profile", Exact = true });
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        await ClickJustOutsideActiveDialogAsync(page, dialog);

        await Assertions.Expect(dialog).Not.ToBeVisibleAsync();
    }
}

