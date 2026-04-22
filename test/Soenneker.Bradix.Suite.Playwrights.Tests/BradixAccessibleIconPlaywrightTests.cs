using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixAccessibleIconPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixAccessibleIconPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
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
}

