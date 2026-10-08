using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using System.Threading;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixPortalPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixPortalPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async ValueTask Portal_demo_reparents_content_outside_docs_content(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/portals"));

        await Assertions.Expect(page.Locator("body .portal-surface")).ToContainTextAsync("Portaled into body.");
        await Assertions.Expect(page.Locator(".docs-shell__content .portal-surface")).ToHaveCountAsync(0);
    }
}

