using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Soenneker.Playwrights.Tests.Unit;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixFoundationAndInfrastructurePlaywrightTests : PlaywrightUnitTest
{
    public BradixFoundationAndInfrastructurePlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

    [Fact]
    public async Task Accessible_icon_demo_exposes_accessible_name_and_toggles_state()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/accessible-icon"));

        ILocator button = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Close panel", Exact = true });
        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Panel: Open");

        await button.ClickAsync();

        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Panel: Closed");
    }

    [Fact]
    public async Task Collection_demo_updates_active_match_and_respects_reordering()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/collection"));

        ILocator input = page.Locator("#typeahead-input");
        await input.ClickAsync();
        await page.Keyboard.PressAsync("b");

        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Active match: Beta");

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Reset search", Exact = true }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Move Blue to first", Exact = true }).ClickAsync();
        await input.ClickAsync();
        await page.Keyboard.PressAsync("b");

        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Active match: Blue");
    }

    [Fact]
    public async Task Label_demo_focuses_input_when_label_is_clicked()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/label"));

        ILocator input = page.Locator("#firstName");
        await page.GetByText("First name", new PageGetByTextOptions { Exact = true }).ClickAsync();

        await Assertions.Expect(input).ToBeFocusedAsync();
    }

    [Fact]
    public async Task Portal_demo_reparents_content_outside_docs_content()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/portal"));

        await Assertions.Expect(page.Locator("body .portal-surface")).ToContainTextAsync("Portaled into body.");
        await Assertions.Expect(page.Locator(".docs-shell__content .portal-surface")).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task Presence_demo_runs_exit_completion_when_toggled_closed()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/presence"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Toggle presence", Exact = true }).ClickAsync();

        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Present: False");
        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Exit complete count: 1");
    }

    [Fact]
    public async Task Slot_demo_merges_child_attributes_into_target_element()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/slot"));

        ILocator button = page.Locator("#slot-button");

        await Assertions.Expect(button).ToHaveAttributeAsync("title", "child target title");
        await Assertions.Expect(button).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("slot-demo__child"));
        await Assertions.Expect(button).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("primitive__trigger"));
    }

    [Fact]
    public async Task Visually_hidden_demo_preserves_accessible_name_for_icon_button()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/visually-hidden"));

        await Assertions.Expect(page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save the file", Exact = true })).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Dismissable_layer_demo_dismisses_on_outside_click()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dismissable-layer"));

        await page.Locator(".docs-shell__main").ClickAsync(new LocatorClickOptions
        {
            Position = new Position
            {
                X = 12,
                Y = 12
            }
        });

        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Dismissed: True");
    }

    [Fact]
    public async Task Focus_scope_demo_loops_focus_back_to_first_item()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/focus-scope"));

        ILocator first = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "First", Exact = true });
        ILocator third = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Third", Exact = true });

        await third.FocusAsync();
        await page.Keyboard.PressAsync("Tab");

        await Assertions.Expect(first).ToBeFocusedAsync();
    }

    [Fact]
    public async Task Popper_demo_reports_initial_placement()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/popper"));

        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync(new System.Text.RegularExpressions.Regex("Placed:\\s+[1-9]"));
    }

    [Fact]
    public async Task Remove_scroll_demo_mount_toggle_shows_and_hides_locked_surface()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/remove-scroll"));

        ILocator textarea = page.GetByRole(AriaRole.Textbox);
        ILocator toggle = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Toggle scroll lock", Exact = true });

        await Assertions.Expect(textarea).ToBeVisibleAsync();

        await toggle.ClickAsync();
        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Mounted: False");
        await Assertions.Expect(textarea).ToHaveCountAsync(0);

        await toggle.ClickAsync();
        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Mounted: True");
        await Assertions.Expect(page.GetByRole(AriaRole.Textbox)).ToBeVisibleAsync();
    }
}
