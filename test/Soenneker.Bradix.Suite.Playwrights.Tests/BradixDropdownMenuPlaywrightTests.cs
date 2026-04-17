using Soenneker.Playwrights.Extensions.TestPages;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixDropdownMenuPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixDropdownMenuPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

[Fact]
    public async Task Dropdown_menu_submenu_home_and_end_keys_move_focus_to_first_and_last_items()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dropdown-menu"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Customise options", Exact = true }).ClickAsync();
        await page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "More Tools", Exact = true }).ClickAsync();

        ILocator savePageAs = page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "Save Page As…", Exact = true });
        ILocator createShortcut = page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "Create Shortcut…", Exact = true });
        ILocator developerTools = page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "Developer Tools", Exact = true });

        await createShortcut.FocusAsync();
        await createShortcut.PressAsync("Home");

        await Assertions.Expect(savePageAs).ToBeFocusedAsync();

        await savePageAs.PressAsync("End");

        await Assertions.Expect(developerTools).ToBeFocusedAsync();

        await developerTools.PressAsync("Home");

        await Assertions.Expect(savePageAs).ToBeFocusedAsync();
    }

[Fact]
    public async Task Dropdown_menu_demo_keeps_checkbox_and_radio_groups_open_when_close_on_select_is_disabled()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dropdown-menu"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Customise options", Exact = true }).ClickAsync();

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

[Fact]
    public async Task Dropdown_menu_demo_home_and_end_keys_move_focus_to_first_and_last_items()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dropdown-menu"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Customise options", Exact = true }).ClickAsync();

        ILocator menu = page.VisibleMenu();
        ILocator newTab = menu.GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "New Tab", Exact = true });
        ILocator newWindow = menu.GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "New Window", Exact = true });
        ILocator colm = menu.Locator("[role='menuitemradio']").Filter(new LocatorFilterOptions { HasText = "Colm Tuite" }).First;

        await newWindow.FocusAsync();
        await newWindow.PressAsync("Home");

        await Assertions.Expect(newTab).ToBeFocusedAsync();

        await newTab.PressAsync("End");

        await Assertions.Expect(colm).ToBeFocusedAsync();

        await colm.PressAsync("Home");

        await Assertions.Expect(newTab).ToBeFocusedAsync();
    }

[Fact]
    public async Task Dropdown_menu_demo_opens_and_reveals_submenu_items()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dropdown-menu"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Customise options", Exact = true }).ClickAsync();

        await Assertions.Expect(page.VisibleMenu()).ToContainTextAsync("New Tab");
        await page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "More Tools", Exact = true }).ClickAsync();
        await Assertions.Expect(page.VisibleMenu()).ToContainTextAsync("Developer Tools");
    }

[Fact]
    public async Task Dropdown_menu_demo_supports_nested_menu_inside_modal_dialog()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dropdown-menu"));

        ILocator dialogTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open dropdown dialog", Exact = true });
        await dialogTrigger.ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Dropdown dialog", Exact = true });
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        ILocator menuTrigger = dialog.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Open nested dropdown menu", Exact = true });
        await menuTrigger.ClickAsync();

        ILocator menu = page.GetByRole(AriaRole.Menu).Filter(new LocatorFilterOptions { HasText = "More options" });
        await Assertions.Expect(menuTrigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(menu).ToBeVisibleAsync();

        await menu.GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "More options", Exact = true }).ClickAsync();

        ILocator submenuTrigger = menu.GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "More options", Exact = true });
        ILocator submenu = page.GetByRole(AriaRole.Menu).Filter(new LocatorFilterOptions { HasText = "Create shortcut" });
        await Assertions.Expect(submenu).ToBeVisibleAsync();
        await Assertions.Expect(submenu).ToContainTextAsync("Save page");

        await submenuTrigger.PressAsync("Escape");

        await Assertions.Expect(submenu).Not.ToBeVisibleAsync();
        await Assertions.Expect(menu).Not.ToBeVisibleAsync();
        await Assertions.Expect(dialog).ToBeVisibleAsync();
    }
}

