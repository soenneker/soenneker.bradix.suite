using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixPortalPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixPortalPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

[Fact]
    public async Task Portal_demo_reparents_content_outside_docs_content()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/portal"));

        await Assertions.Expect(page.Locator("body .portal-surface")).ToContainTextAsync("Portaled into body.");
        await Assertions.Expect(page.Locator(".docs-shell__content .portal-surface")).ToHaveCountAsync(0);
    }
}

