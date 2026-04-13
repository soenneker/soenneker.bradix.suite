using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixDisclosurePlaywrightTests : FixturedUnitTest
{
    private readonly Fixture _fixture;

    public BradixDisclosurePlaywrightTests(Fixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Accordion_demo_switches_visible_content_between_items()
    {
        await using BrowserSession session = await _fixture.CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(_fixture, DemoPageSpecs.Get("/accordion"));

        ILocator accessibleTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Is it accessible?", Exact = true });
        ILocator unstyledTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Is it unstyled?", Exact = true });

        await Assertions.Expect(accessibleTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        await unstyledTrigger.ClickAsync();

        await Assertions.Expect(unstyledTrigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(accessibleTrigger).ToHaveAttributeAsync("aria-expanded", "false");
    }

    [Fact]
    public async Task Alert_dialog_demo_opens_and_closes_from_cancel()
    {
        await using BrowserSession session = await _fixture.CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(_fixture, DemoPageSpecs.Get("/alert-dialog"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Delete account", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Are you absolutely sure?", Exact = true })).ToBeVisibleAsync();
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Cancel", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Are you absolutely sure?", Exact = true })).Not.ToBeVisibleAsync();
    }

    [Fact]
    public async Task Collapsible_demo_reveals_additional_repositories_when_opened()
    {
        await using BrowserSession session = await _fixture.CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(_fixture, DemoPageSpecs.Get("/collapsible"));

        ILocator trigger = page.GetByRole(AriaRole.Button);
        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "false");

        await trigger.ClickAsync();

        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(page.Locator(".collapsible-demo__root")).ToContainTextAsync("@radix-ui/colors");
        await Assertions.Expect(page.Locator(".collapsible-demo__root")).ToContainTextAsync("@radix-ui/themes");
    }

    [Fact]
    public async Task Dialog_demo_close_discards_unsaved_changes()
    {
        await using BrowserSession session = await _fixture.CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(_fixture, DemoPageSpecs.Get("/dialog"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true }).ClickAsync();
        await page.Locator("#dialog-name").FillAsync("Unsaved");
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Close", Exact = true }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true }).ClickAsync();
        await Assertions.Expect(page.Locator("#dialog-name")).ToHaveValueAsync("Pedro Duarte");
    }

    [Fact]
    public async Task Hover_card_demo_shows_profile_details_on_hover()
    {
        await using BrowserSession session = await _fixture.CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(_fixture, DemoPageSpecs.Get("/hover-card"));

        await page.GetByAltText("Radix UI").HoverAsync();

        await Assertions.Expect(page.GetByText("@radix_ui")).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByText("2,900")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Popover_demo_opens_and_closes_from_close_button()
    {
        await using BrowserSession session = await _fixture.CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(_fixture, DemoPageSpecs.Get("/popover"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Update dimensions", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByText("Dimensions", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Close", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByText("Dimensions", new PageGetByTextOptions { Exact = true })).Not.ToBeVisibleAsync();
    }

    [Fact]
    public async Task Toast_demo_shows_scheduled_notification()
    {
        await using BrowserSession session = await _fixture.CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(_fixture, DemoPageSpecs.Get("/toast"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add to calendar", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByText("Scheduled: Catch up", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByText("Undo", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Tooltip_demo_reveals_content_on_hover()
    {
        await using BrowserSession session = await _fixture.CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(_fixture, DemoPageSpecs.Get("/tooltip"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "+", Exact = true }).HoverAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Tooltip, new PageGetByRoleOptions { Name = "Add to library", Exact = true })).ToBeVisibleAsync();
    }
}
