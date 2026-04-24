using Soenneker.Playwrights.Extensions.TestPages;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixMenuPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixMenuPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

    [Test]
    public async Task Menu_demo_updates_selection_from_modal_submenu_item()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady(
            $"{BaseUrl}menu",
            static p => p.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open modal menu", Exact = true }),
            expectedTitle: "Menu Demo");

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open modal menu", Exact = true })
            .ClickAsync(new LocatorClickOptions { Timeout = 2000 });

        ILocator shareTrigger = page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "Share", Exact = true });
        ILocator copyLink = page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "Copy link", Exact = true });

        await shareTrigger.ClickAsync();
        await Assertions.Expect(shareTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await copyLink.ClickAsync();

        await Assertions.Expect(page.Locator(".docs-shell__content"))
            .ToContainTextAsync("Last selection: Copy link", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
    }

    [Test]
    public async Task Menu_demo_supports_keyboard_entry_typeahead_and_escape_close()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady(
            $"{BaseUrl}menu",
            static p => p.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open modal menu", Exact = true }),
            expectedTitle: "Menu Demo");

        ILocator trigger = page.Locator("button").Filter(new LocatorFilterOptions { HasText = "Open modal menu" });
        await trigger.ClickAsync();

        ILocator menu = page.VisibleMenu();
        await Assertions.Expect(menu).ToContainTextAsync("Actions");
        await menu.FocusAsync();

        await page.Keyboard.PressAsync("ArrowDown");
        await Assertions.Expect(page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "New file", Exact = true })).ToBeFocusedAsync();

        await page.Keyboard.PressAsync("d");
        await Assertions.Expect(page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "Duplicate", Exact = true })).ToBeFocusedAsync();

        await page.Keyboard.PressAsync("Escape");

        await Assertions.Expect(menu).Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task Menu_demo_submenu_close_key_returns_focus_to_submenu_trigger()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady(
            $"{BaseUrl}menu",
            static p => p.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open modal menu", Exact = true }),
            expectedTitle: "Menu Demo");

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open modal menu", Exact = true }).ClickAsync();

        ILocator shareTrigger = page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "Share", Exact = true });
        await shareTrigger.ClickAsync();

        ILocator submenu = page.GetByRole(AriaRole.Menu).Filter(new LocatorFilterOptions { HasText = "Copy link" });
        await Assertions.Expect(submenu).ToBeVisibleAsync();

        ILocator copyLink = page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "Copy link", Exact = true });
        await copyLink.FocusAsync();
        await copyLink.PressAsync("ArrowLeft");

        await Assertions.Expect(submenu).Not.ToBeVisibleAsync();
        await Assertions.Expect(page.VisibleMenu()).ToContainTextAsync("Actions");
        await Assertions.Expect(shareTrigger).ToBeFocusedAsync();
    }
}

