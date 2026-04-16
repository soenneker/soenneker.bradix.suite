using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Extensions.TestPages;
using Soenneker.Playwrights.Session;
using Soenneker.Playwrights.Tests.Unit;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixNavigationPlaywrightTests : PlaywrightUnitTest
{
    public BradixNavigationPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

    [Fact]
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

    [Fact]
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

    [Fact]
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

        ILocator submenu = page.GetByRole(AriaRole.Menu).Filter(new LocatorFilterOptions { HasText = "Create shortcut" });
        await Assertions.Expect(submenu).ToBeVisibleAsync();
        await Assertions.Expect(submenu).ToContainTextAsync("Save page");
    }

    [Fact]
    public async Task Menubar_demo_allows_radio_selection_changes()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/menubar"));

        await page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "View", Exact = true }).ClickAsync();
        ILocator dateModified = page.GetByText("Date modified", new PageGetByTextOptions { Exact = true }).Locator("..");

        await dateModified.ClickAsync();

        await Assertions.Expect(dateModified).ToHaveAttributeAsync("data-state", "checked");
    }

    [Fact]
    public async Task Menubar_demo_keeps_checkbox_menu_open_when_close_on_select_is_disabled()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/menubar"));

        ILocator editTrigger = page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "Edit", Exact = true });
        await editTrigger.ClickAsync();

        ILocator menu = page.VisibleMenu();
        ILocator lineNumbers = menu.Locator("[role='menuitemcheckbox']").Filter(new LocatorFilterOptions { HasText = "Show line numbers" });
        ILocator wordWrap = menu.Locator("[role='menuitemcheckbox']").Filter(new LocatorFilterOptions { HasText = "Word wrap" });

        await Assertions.Expect(lineNumbers).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(wordWrap).ToHaveAttributeAsync("aria-checked", "false");

        await wordWrap.ClickAsync();

        await Assertions.Expect(menu).ToBeVisibleAsync();
        await Assertions.Expect(wordWrap).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(lineNumbers).ToHaveAttributeAsync("aria-checked", "true");
    }

    [Fact]
    public async Task Menubar_demo_closes_from_single_outside_click()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/menubar"));

        ILocator viewTrigger = page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "View", Exact = true });
        await viewTrigger.ClickAsync();
        await Assertions.Expect(page.VisibleMenu()).ToContainTextAsync("Sort by");

        await page.Locator(".demo-page-intro h1").ClickAsync();

        await Assertions.Expect(viewTrigger).ToHaveAttributeAsync("aria-expanded", "false");
        await Assertions.Expect(page.Locator("[role='menu']:visible")).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task Menu_demo_updates_selection_from_modal_submenu_item()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/menu"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open modal menu", Exact = true })
            .ClickAsync(new LocatorClickOptions { Timeout = 2000 });

        ILocator shareTrigger = page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "Share", Exact = true });
        ILocator copyLink = page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "Copy link", Exact = true });

        await shareTrigger.EvaluateAsync("element => element.click()");
        await Assertions.Expect(shareTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await page.WaitForTimeoutAsync(150);
        await copyLink.EvaluateAsync("element => element.click()");

        await Assertions.Expect(page.Locator(".docs-shell__content"))
            .ToContainTextAsync("Last selection: Copy link", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
    }

    [Fact]
    public async Task Navigation_menu_demo_switches_visible_content_between_triggers()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });
        ILocator viewport = page.Locator(".nav-menu-viewport");

        await learnTrigger.ClickAsync(new LocatorClickOptions { Force = true, Timeout = 1000 });
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(viewport).ToContainTextAsync("Radix Primitives", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });

        await overviewTrigger.ClickAsync(new LocatorClickOptions { Force = true, Timeout = 1000 });
        await Assertions.Expect(overviewTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "false", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(viewport).ToContainTextAsync("Introduction", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
    }

    [Fact]
    public async Task Navigation_menu_demo_closes_from_single_outside_click()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator viewport = page.Locator(".nav-menu-viewport");

        await learnTrigger.ClickAsync(new LocatorClickOptions { Force = true, Timeout = 1000 });
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(viewport).ToContainTextAsync("Radix Primitives", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });

        await page.Locator(".demo-page-intro h1").ClickAsync();

        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "false", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(viewport).ToHaveCountAsync(0, new LocatorAssertionsToHaveCountOptions { Timeout = 3000 });
    }

    [Fact]
    public async Task Scroll_area_demo_allows_viewport_scrolling()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/scroll-area"));

        int scrollTop = await page.EvaluateAsync<int>(
            "() => { const viewport = document.querySelector('[data-radix-scroll-area-viewport]'); if (!viewport) return -1; viewport.scrollTop = 200; return viewport.scrollTop; }");
        Assert.True(scrollTop > 0);
    }

    [Fact]
    public async Task Tabs_demo_switches_visible_panel_content()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/tabs"));

        await Assertions.Expect(page.GetByText("Make changes to your account here. Click save when you're done.")).ToBeVisibleAsync();
        await page.GetByRole(AriaRole.Tab, new PageGetByRoleOptions { Name = "Password", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByText("Change your password here. After saving, you'll be logged out.")).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByLabel("Current password")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Tabs_demo_disabled_trigger_stays_inactive_while_enabled_sibling_can_activate()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/tabs"));

        ILocator disabledList = page.GetByLabel("Disabled tabs example", new PageGetByLabelOptions { Exact = true });
        ILocator overview = disabledList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "Overview", Exact = true });
        ILocator disabled = disabledList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "Disabled", Exact = true });
        ILocator details = disabledList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "Details", Exact = true });

        await Assertions.Expect(overview).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(disabled).ToBeDisabledAsync();

        await disabled.ClickAsync(new LocatorClickOptions { Force = true });

        await Assertions.Expect(overview).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(page.GetByText("Disabled tab content should remain unreachable.", new PageGetByTextOptions { Exact = true })).Not.ToBeVisibleAsync();

        await details.ClickAsync();

        await Assertions.Expect(details).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(page.GetByText("Details tab content.", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Tabs_demo_controlled_buttons_sync_selected_trigger_and_panel()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/tabs"));

        ILocator controlledList = page.GetByLabel("Controlled tabs example", new PageGetByLabelOptions { Exact = true });
        ILocator overview = controlledList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "Overview", Exact = true });
        ILocator activity = controlledList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "Activity", Exact = true });
        ILocator settings = controlledList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "Settings", Exact = true });

        await Assertions.Expect(overview).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(page.GetByText("Controlled overview content.", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Settings", Exact = true }).First.ClickAsync();

        await Assertions.Expect(settings).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(activity).ToHaveAttributeAsync("aria-selected", "false");
        await Assertions.Expect(page.GetByText("Controlled settings content.", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Toolbar_demo_updates_pressed_states_for_toggle_groups()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toolbar"));

        ILocator bold = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "B", Exact = true }).First;
        ILocator left = page.GetByRole(AriaRole.Radio, new PageGetByRoleOptions { Name = "L", Exact = true }).First;
        ILocator center = page.GetByRole(AriaRole.Radio, new PageGetByRoleOptions { Name = "C", Exact = true }).First;

        await bold.ClickAsync();
        await left.ClickAsync();

        await Assertions.Expect(bold).ToHaveAttributeAsync("aria-pressed", "true");
        await Assertions.Expect(left).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(center).ToHaveAttributeAsync("aria-checked", "false");
    }
}
