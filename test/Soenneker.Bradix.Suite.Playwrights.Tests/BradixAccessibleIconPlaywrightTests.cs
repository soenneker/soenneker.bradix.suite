using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixAccessibleIconPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixAccessibleIconPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

[Fact]
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

