using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixCollectionPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixCollectionPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
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
}

