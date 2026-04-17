using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixNavigationMenuPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixNavigationMenuPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
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
    public async Task Navigation_menu_demo_home_and_end_keys_move_focus_between_edge_triggers()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });
        ILocator githubLink = page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Github", Exact = true });

        await overviewTrigger.FocusAsync();
        await overviewTrigger.PressAsync("Home");

        await Assertions.Expect(learnTrigger).ToBeFocusedAsync();

        await learnTrigger.PressAsync("End");

        await Assertions.Expect(githubLink).ToBeFocusedAsync();

        await githubLink.PressAsync("Home");

        await Assertions.Expect(learnTrigger).ToBeFocusedAsync();
    }

[Fact]
    public async Task Navigation_menu_demo_switches_visible_content_between_triggers_on_hover()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });
        ILocator viewport = page.Locator(".nav-menu-viewport");

        await learnTrigger.EvaluateAsync("element => element.click()");
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });

        await overviewTrigger.HoverAsync();

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
    public async Task Navigation_menu_demo_marks_active_link_and_direct_link_does_not_open_viewport()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator section = page.Locator("section").Filter(new LocatorFilterOptions { HasText = "Learn" }).First;
        ILocator learnTrigger = section.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = section.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Overview", Exact = true });
        ILocator githubLink = section.GetByRole(AriaRole.Link, new LocatorGetByRoleOptions { Name = "Github", Exact = true });

        await githubLink.HoverAsync();

        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "false");
        await Assertions.Expect(overviewTrigger).ToHaveAttributeAsync("aria-expanded", "false");
        await Assertions.Expect(page.Locator(".nav-menu-viewport")).ToHaveCountAsync(0);

        await learnTrigger.ClickAsync(new LocatorClickOptions { Force = true, Timeout = 1000 });

        ILocator activeLink = page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Radix Primitives", Exact = true });
        await Assertions.Expect(activeLink).ToHaveAttributeAsync("aria-current", "page");
        await Assertions.Expect(activeLink).ToHaveAttributeAsync("data-active", string.Empty);
        Assert.Null(await githubLink.GetAttributeAsync("aria-expanded"));
    }

[Fact]
    public async Task Navigation_menu_demo_home_and_end_keys_move_focus_between_edge_links_in_open_content()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        await learnTrigger.ClickAsync(new LocatorClickOptions { Force = true, Timeout = 1000 });

        ILocator primitives = page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Radix Primitives", Exact = true });
        ILocator stitches = page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Stitches", Exact = true });
        ILocator icons = page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Icons", Exact = true });

        await stitches.FocusAsync();
        await stitches.PressAsync("Home");

        await Assertions.Expect(primitives).ToBeFocusedAsync();

        await primitives.PressAsync("End");

        await Assertions.Expect(icons).ToBeFocusedAsync();

        await icons.PressAsync("Home");

        await Assertions.Expect(primitives).ToBeFocusedAsync();
    }

[Fact]
    public async Task Navigation_menu_demo_shared_viewport_expands_to_fit_open_content()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;
        await page.SetViewportSizeAsync(1400, 1000);

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator viewport = page.Locator(".nav-menu-viewport");

        await learnTrigger.EvaluateAsync("element => element.click()");
        await Assertions.Expect(viewport).ToContainTextAsync("Radix Primitives", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });

        LocatorBoundingBoxResult? viewportBox = await viewport.BoundingBoxAsync();
        Assert.NotNull(viewportBox);
        Assert.True(viewportBox.Width >= 320, $"Expected the shared Bradix navigation viewport to expand beyond a sliver, but measured width {viewportBox.Width}.");
        Assert.True(viewportBox.Height >= 120, $"Expected the shared Bradix navigation viewport to show full content, but measured height {viewportBox.Height}.");
    }
}

