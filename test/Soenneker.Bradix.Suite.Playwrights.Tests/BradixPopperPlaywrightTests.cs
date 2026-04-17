using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixPopperPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixPopperPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

[Fact]
    public async Task Popper_demo_reports_initial_placement()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/popper"));

        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync(new System.Text.RegularExpressions.Regex("Placed:\\s+[1-9]"));
    }
}

