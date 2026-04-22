using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixPopperPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixPopperPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async Task Popper_demo_reports_initial_placement()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/popper"));

        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync(new System.Text.RegularExpressions.Regex("Placed:\\s+[1-9]"));
    }
}

