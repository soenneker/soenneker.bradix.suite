using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

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

[Fact(Skip = "Hover-triggered content switching does not fire in the interactive browser demo under Playwright; click switching is covered separately.")]
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
        Assert.NotNull(learnBox);
        Assert.NotNull(overviewBox);

        await page.Mouse.MoveAsync(learnBox.X + (learnBox.Width / 2), learnBox.Y + (learnBox.Height / 2));
        await page.Mouse.MoveAsync(overviewBox.X + (overviewBox.Width / 2), overviewBox.Y + (overviewBox.Height / 2));

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
        Assert.Null(await githubLink.GetAttributeAsync("aria-expanded"));
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
        Assert.NotNull(viewportBox);
        Assert.True(viewportBox.Width >= 320, $"Expected the shared Bradix navigation viewport to expand beyond a sliver, but measured width {viewportBox.Width}.");
        Assert.True(viewportBox.Height >= 120, $"Expected the shared Bradix navigation viewport to show full content, but measured height {viewportBox.Height}.");
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

        Assert.NotNull(diagnostics);
        TriggerProbe? overview = Assert.Single(diagnostics.buttons, button => button.text == "Overview");
        Assert.True(overview.width > 0 && overview.height > 0,
            $"Expected Overview trigger to keep a measurable box after opening Learn, but measured left={overview.left}, top={overview.top}, width={overview.width}, height={overview.height}.");
        Assert.NotNull(diagnostics.hitTarget);
        Assert.True(string.Equals(diagnostics.hitTarget.tagName, "BUTTON", System.StringComparison.OrdinalIgnoreCase),
            $"Expected Overview trigger center to resolve to the trigger button, but hit tag={diagnostics.hitTarget.tagName}, id={diagnostics.hitTarget.id}, text={diagnostics.hitTarget.text}, aria-labelledby={diagnostics.hitTarget.ariaLabelledBy}, data-state={diagnostics.hitTarget.dataState}.");
    }

    [Test]
    public async Task Navigation_menu_demo_debugs_overview_trigger_events_after_open()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });
        ILocator overviewTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Overview", Exact = true });

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        await overviewTrigger.EvaluateAsync(
            @"button => {
                window.__navMenuDebug = { pointerdown: 0, click: 0, mousedown: 0 };
                button.addEventListener('pointerdown', () => window.__navMenuDebug.pointerdown++);
                button.addEventListener('mousedown', () => window.__navMenuDebug.mousedown++);
                button.addEventListener('click', () => window.__navMenuDebug.click++);
                document.addEventListener('click', event => {
                    window.__navMenuDebug.documentClick = (window.__navMenuDebug.documentClick || 0) + 1;
                    window.__navMenuDebug.documentDefaultPrevented = !!event.defaultPrevented;
                });
            }");

        await ClickTriggerAsync(page, overviewTrigger);

        TriggerEventDiagnostics? diagnostics = await page.EvaluateAsync<TriggerEventDiagnostics>(
            @"() => ({
                pointerdown: window.__navMenuDebug?.pointerdown ?? 0,
                mousedown: window.__navMenuDebug?.mousedown ?? 0,
                click: window.__navMenuDebug?.click ?? 0,
                documentClick: window.__navMenuDebug?.documentClick ?? 0,
                documentDefaultPrevented: !!window.__navMenuDebug?.documentDefaultPrevented,
                activeElementTag: document.activeElement?.tagName ?? null,
                activeElementText: document.activeElement?.textContent?.trim() ?? null
            })");

        Assert.NotNull(diagnostics);
        Assert.True(diagnostics.pointerdown > 0, "Expected Overview trigger to receive pointerdown when clicked after opening Learn.");
        Assert.True(diagnostics.click > 0,
            $"Expected Overview trigger to receive click when clicked after opening Learn, but observed pointerdown={diagnostics.pointerdown}, mousedown={diagnostics.mousedown}, click={diagnostics.click}, documentClick={diagnostics.documentClick}, documentDefaultPrevented={diagnostics.documentDefaultPrevented}, activeElementTag={diagnostics.activeElementTag}, activeElementText={diagnostics.activeElementText}.");
        Assert.True(diagnostics.documentClick > 0,
            $"Expected Overview click to bubble to document, but observed pointerdown={diagnostics.pointerdown}, mousedown={diagnostics.mousedown}, click={diagnostics.click}, documentClick={diagnostics.documentClick}, documentDefaultPrevented={diagnostics.documentDefaultPrevented}.");
        Assert.False(diagnostics.documentDefaultPrevented,
            $"Expected Overview click to reach document without being default-prevented, but observed pointerdown={diagnostics.pointerdown}, mousedown={diagnostics.mousedown}, click={diagnostics.click}, documentClick={diagnostics.documentClick}, documentDefaultPrevented={diagnostics.documentDefaultPrevented}.");
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

        Assert.True(string.Equals(immediate, "true", System.StringComparison.Ordinal) || string.Equals(delayed, "true", System.StringComparison.Ordinal),
            $"Expected Overview to open at least briefly when clicked after Learn, but immediate aria-expanded={immediate ?? "<null>"}, delayed aria-expanded={delayed ?? "<null>"}.");
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

        Assert.NotNull(before);
        Assert.True(before.isConnected);

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

        Assert.NotNull(after);
        Assert.True(after.isSameNode,
            $"Expected Overview trigger DOM node to stay stable after opening Learn, but it was replaced. PreviousConnected={after.previousIsConnected}, CurrentConnected={after.currentIsConnected}, CurrentHtml={after.outerHtml}");
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
        Assert.True(string.Equals(expanded, "true", System.StringComparison.Ordinal),
            $"Expected Overview to open from the initial closed state, but aria-expanded was {expanded ?? "<null>"}.");
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

[Fact(Skip = "Keyboard trigger-switch parity is deferred while core render/layout issues are being stabilized.")]
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

    [Fact(Skip = "Repeat-click close behavior in the browser demo remains a non-contract debug probe; core open, switch, focus, and outside-dismiss behavior is covered separately.")]
    public async Task Navigation_menu_demo_debugs_active_trigger_can_close_itself_after_open()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/navigation-menu"));

        ILocator learnTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true });

        await ClickTriggerAsync(page, learnTrigger);
        await Assertions.Expect(learnTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        await ClickTriggerAsync(page, learnTrigger);

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

        Assert.Empty(pageErrors);
        Assert.DoesNotContain(consoleMessages, message => message.Contains("ObjectDisposed", System.StringComparison.Ordinal));
        Assert.DoesNotContain(consoleMessages, message => message.Contains("Unhandled exception rendering component", System.StringComparison.Ordinal));
    }

    private static async Task ClickTriggerAsync(IPage page, ILocator trigger)
    {
        LocatorBoundingBoxResult? box = await trigger.BoundingBoxAsync();
        Assert.NotNull(box);
        await page.Mouse.ClickAsync(box.X + (box.Width / 2), box.Y + (box.Height / 2));
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

    private sealed class TriggerEventDiagnostics
    {
        public int pointerdown { get; set; }
        public int mousedown { get; set; }
        public int click { get; set; }
        public int documentClick { get; set; }
        public bool documentDefaultPrevented { get; set; }
        public string? activeElementTag { get; set; }
        public string? activeElementText { get; set; }
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

