using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[NotInParallel]
[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixNavigationMenuPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixNavigationMenuPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async Task Navigation_menu_demo_switches_visible_content_between_triggers()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });
        ILocator viewport = page.Locator(".nav-menu-viewport");

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(viewport).ToContainTextAsync("Radix Primitives", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });

        await ClickTriggerAsync(page, overviewTrigger);
        await Assertions.Expect(overviewTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "false", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(viewport).ToContainTextAsync("Introduction", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
    }

[Test]
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

[Test]
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

        LocatorBoundingBoxResult? learnBox = await learnTrigger.BoundingBoxAsync();
        LocatorBoundingBoxResult? overviewBox = await overviewTrigger.BoundingBoxAsync();
        await Assert.That(learnBox).IsNotNull();
        await Assert.That(overviewBox).IsNotNull();

        await page.Mouse.MoveAsync(learnBox!.X + (learnBox.Width / 2), learnBox.Y + (learnBox.Height / 2));
        await page.Mouse.MoveAsync(overviewBox!.X + (overviewBox.Width / 2), overviewBox.Y + (overviewBox.Height / 2));

        await Assertions.Expect(overviewTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "false", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(viewport).ToContainTextAsync("Introduction", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
    }

[Test]
    public async Task Navigation_menu_uncontrolled_demo_switches_visible_content_between_triggers()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu-uncontrolled"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });
        ILocator viewport = page.Locator(".nav-menu-viewport");

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(viewport).ToContainTextAsync("Radix Primitives", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });

        await ClickTriggerAsync(page, overviewTrigger);
        await Assertions.Expect(overviewTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "false", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(viewport).ToContainTextAsync("Introduction", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
    }

[Test]
    public async Task Navigation_menu_minimal_demo_switches_visible_content_between_triggers()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu-minimal"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });
        ILocator viewport = page.Locator(".nav-menu-viewport");

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(viewport).ToContainTextAsync("Learn body", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });

        await ClickTriggerAsync(page, overviewTrigger);
        await Assertions.Expect(overviewTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "false", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(viewport).ToContainTextAsync("Overview body", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
    }

[Test]
    public async Task Navigation_menu_inline_demo_switches_visible_content_between_triggers()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu-inline"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Radix Primitives", Exact = true }))
                        .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 3000 });

        await ClickTriggerAsync(page, overviewTrigger);
        await Assertions.Expect(overviewTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "false", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Introduction", Exact = true }))
                        .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 3000 });
    }

[Test]
    public async Task Navigation_menu_demo_closes_from_single_outside_click()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator viewport = page.Locator(".nav-menu-viewport");

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(viewport).ToContainTextAsync("Radix Primitives", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });

        await page.Locator(".demo-page-intro h1").ClickAsync();

        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "false", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(viewport).ToHaveCountAsync(0, new LocatorAssertionsToHaveCountOptions { Timeout = 3000 });
    }

[Test]
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

        await ClickTriggerAsync(page, learnTrigger);

        ILocator activeLink = page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Radix Primitives", Exact = true });
        await Assertions.Expect(activeLink).ToHaveAttributeAsync("aria-current", "page");
        await Assertions.Expect(activeLink).ToHaveAttributeAsync("data-active", string.Empty);
        await Assert.That(await githubLink.GetAttributeAsync("aria-expanded")).IsNull();
    }

[Test]
    public async Task Navigation_menu_demo_home_and_end_keys_move_focus_between_edge_links_in_open_content()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        await ClickTriggerAsync(page, learnTrigger);

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

[Test]
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
        await Assert.That(viewportBox).IsNotNull();
        await Assert.That(viewportBox!.Width >= 320).IsTrue();
        await Assert.That(viewportBox.Height >= 120).IsTrue();
    }

[Test]
    public async Task Navigation_menu_demo_debugs_trigger_hit_target_after_open()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        TriggerDiagnostics? diagnostics = await page.EvaluateAsync<TriggerDiagnostics>(
            @"() => {
                const buttons = [...document.querySelectorAll('button')].map(button => {
                    const rect = button.getBoundingClientRect();
                    return {
                        text: button.textContent?.trim() ?? null,
                        id: button.id || null,
                        ariaExpanded: button.getAttribute('aria-expanded'),
                        width: rect.width,
                        height: rect.height,
                        left: rect.left,
                        top: rect.top
                    };
                });

                const overview = buttons.find(button => button.text === 'Overview') ?? null;
                const hitTarget = overview && Number.isFinite(overview.left) && Number.isFinite(overview.top)
                    ? (() => {
                        const element = document.elementFromPoint(overview.left + (overview.width / 2), overview.top + (overview.height / 2));
                        return {
                            tagName: element?.tagName ?? null,
                            id: element?.id ?? null,
                            text: element?.textContent?.trim() ?? null,
                            ariaLabelledBy: element?.getAttribute?.('aria-labelledby') ?? null,
                            dataState: element?.getAttribute?.('data-state') ?? null
                        };
                    })()
                    : null;

                return { buttons, hitTarget };
            }");

        await Assert.That(diagnostics).IsNotNull();
        await Assert.That(diagnostics!.buttons.Count(button => button.text == "Overview")).IsEqualTo(1);
        TriggerProbe overview = diagnostics.buttons.Single(button => button.text == "Overview");
        await Assert.That(overview.width > 0 && overview.height > 0).IsTrue();
        await Assert.That(diagnostics.hitTarget).IsNotNull();
        await Assert.That(string.Equals(diagnostics.hitTarget!.tagName, "BUTTON", System.StringComparison.OrdinalIgnoreCase)).IsTrue();
    }

    [Test]
    public async Task Navigation_menu_demo_opens_overview_trigger_after_learn_is_open()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        await ClickTriggerAsync(page, overviewTrigger);
        await Assertions.Expect(overviewTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "false", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
    }

    [Test]
    public async Task Navigation_menu_demo_debugs_overview_state_immediately_and_after_delay()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        await ClickTriggerAsync(page, overviewTrigger);

        string? immediate = await overviewTrigger.GetAttributeAsync("aria-expanded");
        await page.WaitForTimeoutAsync(300);
        string? delayed = await overviewTrigger.GetAttributeAsync("aria-expanded");

        await Assert.That(string.Equals(immediate, "true", System.StringComparison.Ordinal) || string.Equals(delayed, "true", System.StringComparison.Ordinal)).IsTrue();
    }

    [Test]
    public async Task Navigation_menu_demo_debugs_programmatic_click_switch_after_open()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        await ClickTriggerAsync(page, overviewTrigger);
        await Assertions.Expect(overviewTrigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "false");
    }

    [Test]
    public async Task Navigation_menu_demo_debugs_overview_trigger_identity_after_open()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });

        TriggerIdentityDiagnostics? before = await page.EvaluateAsync<TriggerIdentityDiagnostics>(
            @"() => {
                const overview = [...document.querySelectorAll('button')].find(button => button.textContent?.trim() === 'Overview') ?? null;
                if (!overview) {
                    return null;
                }

                window.__overviewBeforeOpen = overview;

                return {
                    isConnected: overview.isConnected,
                    outerHtml: overview.outerHTML
                };
            }");

        await Assert.That(before).IsNotNull();
        await Assert.That(before!.isConnected).IsTrue();

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        TriggerIdentityAfterOpenDiagnostics? after = await page.EvaluateAsync<TriggerIdentityAfterOpenDiagnostics>(
            @"() => {
                const overview = [...document.querySelectorAll('button')].find(button => button.textContent?.trim() === 'Overview') ?? null;
                if (!overview) {
                    return null;
                }

                return {
                    isSameNode: window.__overviewBeforeOpen === overview,
                    previousIsConnected: !!window.__overviewBeforeOpen?.isConnected,
                    currentIsConnected: overview.isConnected,
                    outerHtml: overview.outerHTML
                };
            }");

        await Assert.That(after).IsNotNull();
        await Assert.That(after!.isSameNode).IsTrue();
    }

    [Test]
    public async Task Navigation_menu_demo_debugs_overview_can_open_first()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });

        await overviewTrigger.EvaluateAsync("element => element.click()");

        string? expanded = await overviewTrigger.GetAttributeAsync("aria-expanded");
        await Assert.That(string.Equals(expanded, "true", System.StringComparison.Ordinal)).IsTrue();
    }

    [Test]
    public async Task Navigation_menu_demo_debugs_state_after_failed_switch_attempt()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });
        ILocator viewport = page.Locator(".nav-menu-viewport");

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        await overviewTrigger.EvaluateAsync("element => element.click()");
        await Assertions.Expect(overviewTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "false", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(viewport).ToContainTextAsync("Introduction", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
    }

[Test]
    public async Task Navigation_menu_demo_debugs_keyboard_switch_after_open()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        await overviewTrigger.FocusAsync();
        await overviewTrigger.PressAsync("Enter");

        await Assertions.Expect(overviewTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "false", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
    }

    [Test]
    public async Task Navigation_menu_demo_debugs_active_trigger_can_close_itself_after_open()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        await learnTrigger.EvaluateAsync("element => element.click()");

        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "false", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
    }

    [Test]
    public async Task Navigation_menu_demo_debugs_console_and_page_errors_after_open()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;
        List<string> consoleMessages = [];
        List<string> pageErrors = [];

        page.Console += (_, message) => consoleMessages.Add($"{message.Type}: {message.Text}");
        page.PageError += (_, exception) => pageErrors.Add(exception);

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        await ClickTriggerAsync(page, overviewTrigger);
        await page.WaitForTimeoutAsync(250);

        await Assert.That(pageErrors).IsEmpty();
        await Assert.That(consoleMessages.Any(message => message.Contains("ObjectDisposed", System.StringComparison.Ordinal))).IsFalse();
        await Assert.That(consoleMessages.Any(message => message.Contains("Unhandled exception rendering component", System.StringComparison.Ordinal))).IsFalse();
    }

    private static async Task ClickTriggerAsync(IPage page, ILocator trigger)
    {
        LocatorBoundingBoxResult? box = await trigger.BoundingBoxAsync();
        await Assert.That(box).IsNotNull();
        await page.Mouse.ClickAsync(box!.X + (box.Width / 2), box.Y + (box.Height / 2));
    }

    private sealed class HitTargetProbe
    {
        public string? tagName { get; set; }
        public string? id { get; set; }
        public string? text { get; set; }
        public string? ariaLabelledBy { get; set; }
        public string? dataState { get; set; }
    }

    private sealed class TriggerProbe
    {
        public string? text { get; set; }
        public string? id { get; set; }
        public string? ariaExpanded { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public double left { get; set; }
        public double top { get; set; }
    }

    private sealed class TriggerDiagnostics
    {
        public TriggerProbe[] buttons { get; set; } = [];
        public HitTargetProbe? hitTarget { get; set; }
    }

    private sealed class TriggerIdentityDiagnostics
    {
        public bool isConnected { get; set; }
        public string? outerHtml { get; set; }
    }

    private sealed class TriggerIdentityAfterOpenDiagnostics
    {
        public bool isSameNode { get; set; }
        public bool previousIsConnected { get; set; }
        public bool currentIsConnected { get; set; }
        public string? outerHtml { get; set; }
    }

}

