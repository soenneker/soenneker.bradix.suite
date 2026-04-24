using Soenneker.Playwrights.Extensions.TestPages;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixMenubarPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixMenubarPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async Task Menubar_demo_escape_closes_submenu_before_parent_menu()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/menubar"));

        ILocator demo = page.Locator("[data-testid='bradix-menubar-demo']");
        ILocator editTrigger = demo.GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "Edit", Exact = true });

        await editTrigger.ClickAsync();

        ILocator menu = page.VisibleMenu();
        await Assertions.Expect(menu).ToContainTextAsync("Show line numbers");
        ILocator submenuTrigger = menu.GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "Share", Exact = true });
        await submenuTrigger.ClickAsync();

        ILocator submenu = page.GetByRole(AriaRole.Menu).Filter(new LocatorFilterOptions { HasText = "Copy link" });
        await Assertions.Expect(submenu).ToBeVisibleAsync();
        await Assertions.Expect(submenu).ToContainTextAsync("Email");

        await page.Keyboard.PressAsync("Escape");

        await Assertions.Expect(submenu).Not.ToBeVisibleAsync();
        await Assertions.Expect(menu).ToBeVisibleAsync();
        await Assertions.Expect(submenuTrigger).ToBeFocusedAsync();

        await submenuTrigger.PressAsync("Escape");

        await Assertions.Expect(menu).Not.ToBeVisibleAsync();
        await Assertions.Expect(editTrigger).ToBeFocusedAsync();
    }

[Test]
    public async Task Menubar_demo_closes_from_single_outside_click()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/menubar"));

        ILocator viewTrigger = page.Locator("[data-testid='bradix-menubar-demo']").GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "View", Exact = true });
        await viewTrigger.ClickAsync();
        await Assertions.Expect(page.VisibleMenu()).ToContainTextAsync("Sort by");

        await page.Locator(".demo-page-intro h1").ClickAsync();

        await Assertions.Expect(viewTrigger).ToHaveAttributeAsync("aria-expanded", "false");
        await Assertions.Expect(page.Locator("[role='menu']:visible")).ToHaveCountAsync(0);
    }

[Test]
    public async Task Menubar_demo_end_key_moves_focus_to_last_top_level_trigger()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/menubar"));

        ILocator demo = page.Locator("[data-testid='bradix-menubar-demo']");
        ILocator file = demo.GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "File", Exact = true });
        ILocator view = demo.GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "View", Exact = true });

        await file.FocusAsync();
        await file.PressAsync("End");

        await Assertions.Expect(view).ToBeFocusedAsync();

        await view.PressAsync("Home");

        await Assertions.Expect(file).ToBeFocusedAsync();
    }

[Test]
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

[Test]
    public async Task Menubar_demo_trigger_keeps_checkbox_menu_open_after_non_closing_selection()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/menubar"));

        ILocator editTrigger = page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "Edit", Exact = true });
        await editTrigger.ClickAsync();

        ILocator wordWrap = page.VisibleMenu()
            .Locator("[role='menuitemcheckbox']")
            .Filter(new LocatorFilterOptions { HasText = "Word wrap" });

        await wordWrap.ClickAsync();
        await Assertions.Expect(editTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        await editTrigger.ClickAsync();

        await Assertions.Expect(editTrigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(page.VisibleMenu()).ToContainTextAsync("Show line numbers");
    }

[Test]
    public async Task Menubar_rtl_demo_inverts_horizontal_roving_focus_between_top_level_triggers()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/menubar"));

        ILocator demo = page.Locator("[data-testid='bradix-menubar-rtl-demo']");
        ILocator file = demo.GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "ملف", Exact = true });
        ILocator view = demo.GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "عرض", Exact = true });

        await file.FocusAsync();
        await file.PressAsync("ArrowRight");
        await Assertions.Expect(view).ToBeFocusedAsync();

        await view.PressAsync("ArrowLeft");
        await Assertions.Expect(file).ToBeFocusedAsync();
    }

[Test]
    public async Task Menubar_demo_allows_radio_selection_changes()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/menubar"));

        await page.Locator("[data-testid='bradix-menubar-demo']").GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "View", Exact = true }).ClickAsync();
        ILocator dateModified = page.GetByText("Date modified", new PageGetByTextOptions { Exact = true }).Locator("..");

        await dateModified.ClickAsync();

        await Assertions.Expect(dateModified).ToHaveAttributeAsync("data-state", "checked");
    }

[Test]
    public async Task Menubar_demo_trigger_keeps_radio_menu_open_after_non_closing_selection()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/menubar"));

        ILocator viewTrigger = page.Locator("[data-testid='bradix-menubar-demo']").GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "View", Exact = true });
        await viewTrigger.ClickAsync();

        ILocator dateModified = page.GetByText("Date modified", new PageGetByTextOptions { Exact = true }).Locator("..");
        await dateModified.ClickAsync();
        await Assertions.Expect(viewTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        await viewTrigger.ClickAsync();

        await Assertions.Expect(viewTrigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(page.VisibleMenu()).ToContainTextAsync("Sort by");
    }

[Test]
    public async Task Menubar_demo_roves_focus_across_triggers_and_opens_adjacent_menu_with_arrow_keys()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/menubar"));

        ILocator demo = page.Locator("[data-testid='bradix-menubar-demo']");
        ILocator file = demo.GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "File", Exact = true });
        ILocator edit = demo.GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "Edit", Exact = true });
        ILocator view = demo.GetByRole(AriaRole.Menuitem, new LocatorGetByRoleOptions { Name = "View", Exact = true });

        await file.FocusAsync();
        await file.PressAsync("ArrowRight");
        await Assertions.Expect(edit).ToBeFocusedAsync();
        await Assertions.Expect(edit).ToHaveAttributeAsync("aria-expanded", "false");

        await edit.PressAsync("Enter");
        await Assertions.Expect(edit).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(page.VisibleMenu()).ToContainTextAsync("Show line numbers");

        await edit.PressAsync("ArrowRight");
        await Assertions.Expect(view).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(edit).ToHaveAttributeAsync("aria-expanded", "false");
        await Assertions.Expect(page.VisibleMenu()).ToContainTextAsync("Sort by");

        await view.FocusAsync();
        await view.PressAsync("Home");
        await Assertions.Expect(file).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(view).ToHaveAttributeAsync("aria-expanded", "false");
        await Assertions.Expect(page.VisibleMenu()).ToContainTextAsync("New file");
    }
}

