using System.Threading.Tasks;
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
    }
}

