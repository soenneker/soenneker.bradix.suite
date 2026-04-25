using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixFocusScopePlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixFocusScopePlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

    [Test]
    public async Task Focus_scope_demo_loops_focus_back_to_first_item()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;
        var consoleErrors = new List<string>();
        var pageErrors = new List<string>();
        page.Console += (_, message) =>
        {
            if (message.Type == "error")
                consoleErrors.Add(message.Text);
        };
        page.PageError += (_, error) => pageErrors.Add(error);

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/focus-scope"));

        ILocator loopDemo = page.Locator(".card").Filter(new LocatorFilterOptions { HasText = "Looping scope" }).First;
        ILocator buttons = loopDemo.Locator(".portal-surface > button");
        ILocator first = buttons.Nth(0);
        ILocator second = buttons.Nth(1);
        ILocator third = buttons.Nth(2);

        await Assertions.Expect(first).ToBeFocusedAsync();

        await first.PressAsync("Shift+Tab");

        await Assertions.Expect(third).ToBeFocusedAsync();

        await third.PressAsync("Tab");

        await Assertions.Expect(first).ToBeFocusedAsync();

        await first.PressAsync("Tab");

        await Assertions.Expect(second).ToBeFocusedAsync();

        consoleErrors.Should().BeEmpty();
        pageErrors.Should().BeEmpty();
    }

    [Test]
    public async Task Focus_scope_demo_traps_programmatic_and_tab_focus()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;
        var consoleErrors = new List<string>();
        var pageErrors = new List<string>();
        page.Console += (_, message) =>
        {
            if (message.Type == "error")
                consoleErrors.Add(message.Text);
        };
        page.PageError += (_, error) => pageErrors.Add(error);

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/focus-scope"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Trapped scope" }).ClickAsync();

        ILocator trapDemo = page.Locator(".card").Filter(new LocatorFilterOptions { HasText = "Trapped scope" }).First;
        ILocator beforeTrap = trapDemo.Locator("#focus-scope-before-trap");
        ILocator first = trapDemo.Locator("#focus-scope-trap-first");
        ILocator second = trapDemo.Locator("#focus-scope-trap-second");
        ILocator afterTrap = trapDemo.Locator("#focus-scope-after-trap");

        await first.FocusAsync();
        await Assertions.Expect(first).ToBeFocusedAsync();

        await page.EvaluateAsync("document.getElementById('focus-scope-after-trap').focus()");
        await first.FocusAsync();
        await Assertions.Expect(first).ToBeFocusedAsync();

        await second.FocusAsync();
        await Assertions.Expect(second).ToBeFocusedAsync();

        await page.EvaluateAsync("document.getElementById('focus-scope-before-trap').focus()");
        await Assertions.Expect(second).ToBeFocusedAsync();

        await second.PressAsync("Tab");
        await Assertions.Expect(first).ToBeFocusedAsync();

        await first.PressAsync("Shift+Tab");
        await Assertions.Expect(second).ToBeFocusedAsync();

        await Assertions.Expect(beforeTrap).Not.ToBeFocusedAsync();
        await Assertions.Expect(afterTrap).Not.ToBeFocusedAsync();

        consoleErrors.Should().BeEmpty();
        pageErrors.Should().BeEmpty();
    }
}

