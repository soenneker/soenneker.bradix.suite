using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixTabsPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixTabsPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async Task Tabs_demo_rtl_reverses_horizontal_arrow_navigation()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/tabs"));

        ILocator section = page.Locator("[data-testid='bradix-tabs-rtl-demo']");
        ILocator list = section.GetByLabel("RTL tabs example", new LocatorGetByLabelOptions { Exact = true });
        ILocator account = list.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "الحساب", Exact = true });
        ILocator password = list.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "كلمة المرور", Exact = true });

        await Assertions.Expect(list).ToHaveAttributeAsync("aria-orientation", "horizontal");
        await Assertions.Expect(account).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(section.Locator("[dir='rtl']").First).ToHaveAttributeAsync("dir", "rtl");
        await Assertions.Expect(section.GetByText("RTL panel A.", new LocatorGetByTextOptions { Exact = true })).ToBeVisibleAsync();

        await account.FocusAsync();
        await account.PressAsync("ArrowLeft");

        await Assertions.Expect(password).ToBeFocusedAsync();
        await Assertions.Expect(password).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(account).ToHaveAttributeAsync("aria-selected", "false");
        await Assertions.Expect(section.GetByText("RTL panel B.", new LocatorGetByTextOptions { Exact = true })).ToBeVisibleAsync();
    }

[Test]
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

[Test]
    public async Task Tabs_demo_manual_activation_keeps_selection_until_space_commits_focused_tab()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/tabs"));

        ILocator manualList = page.GetByLabel("Manual tabs example", new PageGetByLabelOptions { Exact = true });
        ILocator manualAccount = manualList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "Manual Account", Exact = true });
        ILocator manualPassword = manualList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "Manual Password", Exact = true });

        await Assertions.Expect(manualAccount).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(page.GetByText("Manual panel A.", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();

        await manualAccount.FocusAsync();
        await manualAccount.PressAsync("ArrowRight");

        await Assertions.Expect(manualPassword).ToBeFocusedAsync();
        await Assertions.Expect(manualAccount).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(manualPassword).ToHaveAttributeAsync("aria-selected", "false");
        await Assertions.Expect(page.GetByText("Manual panel A.", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();

        await manualPassword.PressAsync("Space");

        await Assertions.Expect(manualPassword).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(manualAccount).ToHaveAttributeAsync("aria-selected", "false");
        await Assertions.Expect(page.GetByText("Manual panel B.", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();
    }

[Test]
    public async Task Tabs_demo_vertical_orientation_uses_down_arrow_to_move_selection()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/tabs"));

        ILocator section = page.Locator("[data-testid='bradix-tabs-vertical-demo']");
        ILocator list = section.GetByLabel("Vertical tabs example", new LocatorGetByLabelOptions { Exact = true });
        ILocator account = list.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "Vertical Account", Exact = true });
        ILocator password = list.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "Vertical Password", Exact = true });

        await Assertions.Expect(list).ToHaveAttributeAsync("aria-orientation", "vertical");
        await Assertions.Expect(account).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(section.GetByText("Vertical panel A.", new LocatorGetByTextOptions { Exact = true })).ToBeVisibleAsync();

        await account.FocusAsync();
        await account.PressAsync("ArrowDown");

        await Assertions.Expect(password).ToBeFocusedAsync();
        await Assertions.Expect(password).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(account).ToHaveAttributeAsync("aria-selected", "false");
        await Assertions.Expect(section.GetByText("Vertical panel B.", new LocatorGetByTextOptions { Exact = true })).ToBeVisibleAsync();
    }

[Test]
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

[Test]
    public async Task Tabs_demo_home_and_end_keys_move_focus_and_selection_to_edges()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/tabs"));

        ILocator verticalSection = page.Locator("[data-testid='bradix-tabs-vertical-demo']");
        ILocator verticalList = verticalSection.GetByLabel("Vertical tabs example", new LocatorGetByLabelOptions { Exact = true });
        ILocator verticalAccount = verticalList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "Vertical Account", Exact = true });
        ILocator verticalNotifications = verticalList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "Vertical Notifications", Exact = true });

        await verticalAccount.FocusAsync();
        await verticalAccount.PressAsync("End");

        await Assertions.Expect(verticalNotifications).ToBeFocusedAsync();
        await Assertions.Expect(verticalNotifications).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(verticalSection.GetByText("Vertical panel C.", new LocatorGetByTextOptions { Exact = true })).ToBeVisibleAsync();

        await verticalNotifications.PressAsync("Home");

        await Assertions.Expect(verticalAccount).ToBeFocusedAsync();
        await Assertions.Expect(verticalAccount).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(verticalSection.GetByText("Vertical panel A.", new LocatorGetByTextOptions { Exact = true })).ToBeVisibleAsync();

        ILocator manualList = page.GetByLabel("Manual tabs example", new PageGetByLabelOptions { Exact = true });
        ILocator manualAccount = manualList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "Manual Account", Exact = true });
        ILocator manualPassword = manualList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = "Manual Password", Exact = true });

        await manualAccount.FocusAsync();
        await manualAccount.PressAsync("End");

        await Assertions.Expect(manualPassword).ToBeFocusedAsync();
        await Assertions.Expect(manualAccount).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(manualPassword).ToHaveAttributeAsync("aria-selected", "false");
        await Assertions.Expect(page.GetByText("Manual panel A.", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();

        await manualPassword.PressAsync("Home");

        await Assertions.Expect(manualAccount).ToBeFocusedAsync();
        await Assertions.Expect(manualAccount).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(manualPassword).ToHaveAttributeAsync("aria-selected", "false");
    }

[Test]
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
}

