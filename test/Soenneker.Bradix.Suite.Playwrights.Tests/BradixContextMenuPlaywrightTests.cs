using Soenneker.Playwrights.Extensions.TestPages;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixContextMenuPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixContextMenuPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async Task Context_menu_demo_supports_nested_menu_inside_modal_dialog()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/context-menu"));

        ILocator dialogTrigger = page.Locator("button.dialog-demo__button").Filter(new LocatorFilterOptions { HasText = "Open context menu dialog" });
        await dialogTrigger.ClickAsync();

        ILocator dialog = page.Locator(".dialog-demo__content[role='dialog'][data-state='open']").Filter(new LocatorFilterOptions { HasText = "Context menu dialog" });
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        ILocator contextSurface = dialog.GetByText("Right-click dialog surface.", new LocatorGetByTextOptions { Exact = true });
        await contextSurface.ClickAsync(new LocatorClickOptions { Button = MouseButton.Right });

        ILocator menu = page.GetByRole(AriaRole.Menu).Filter(new LocatorFilterOptions { HasText = "Pin panel" });
        await Assertions.Expect(menu).ToBeVisibleAsync();
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        await menu.GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "More actions", Exact = true }).ClickAsync();

        ILocator submenu = page.GetByRole(AriaRole.Menu).Filter(new LocatorFilterOptions { HasText = "Duplicate view" });
        await Assertions.Expect(submenu).ToBeVisibleAsync();
        await Assertions.Expect(submenu).ToContainTextAsync("Developer Tools");
        await Assertions.Expect(dialog).ToBeVisibleAsync();
    }

[Test]
    public async Task Context_menu_demo_keeps_checkbox_and_radio_groups_open_when_close_on_select_is_disabled()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/context-menu"));

        await page.GetByText("Right-click here.", new PageGetByTextOptions { Exact = true }).ClickAsync(new LocatorClickOptions
        {
            Button = MouseButton.Right
        });

        ILocator menu = page.VisibleMenu();
        ILocator bookmarks = menu.Locator("[role='menuitemcheckbox']").Filter(new LocatorFilterOptions { HasText = "Show Bookmarks" });
        ILocator urls = menu.Locator("[role='menuitemcheckbox']").Filter(new LocatorFilterOptions { HasText = "Show Full URLs" });
        ILocator pedro = menu.Locator("[role='menuitemradio']").Filter(new LocatorFilterOptions { HasText = "Pedro Duarte" });
        ILocator colm = menu.Locator("[role='menuitemradio']").Filter(new LocatorFilterOptions { HasText = "Colm Tuite" });

        await Assertions.Expect(bookmarks).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(urls).ToHaveAttributeAsync("aria-checked", "false");
        await Assertions.Expect(pedro).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(colm).ToHaveAttributeAsync("aria-checked", "false");

        await urls.ClickAsync();

        await Assertions.Expect(menu).ToBeVisibleAsync();
        await Assertions.Expect(urls).ToHaveAttributeAsync("aria-checked", "true");

        await colm.ClickAsync();

        await Assertions.Expect(menu).ToBeVisibleAsync();
        await Assertions.Expect(colm).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(pedro).ToHaveAttributeAsync("aria-checked", "false");
    }

[Test]
    public async Task Context_menu_demo_opens_from_right_click_and_reveals_submenu()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/context-menu"));

        await page.GetByText("Right-click here.", new PageGetByTextOptions { Exact = true }).ClickAsync(new LocatorClickOptions
        {
            Button = MouseButton.Right
        });

        await Assertions.Expect(page.VisibleMenu()).ToContainTextAsync("Back");
        await page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "More Tools", Exact = true }).ClickAsync();
        await Assertions.Expect(page.VisibleMenu()).ToContainTextAsync("Save Page As");
    }
}

