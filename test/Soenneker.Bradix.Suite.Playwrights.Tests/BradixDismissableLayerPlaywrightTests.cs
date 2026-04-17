using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixDismissableLayerPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixDismissableLayerPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

[Fact]
    public async Task Dismissable_layer_demo_dismisses_on_outside_click()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dismissable-layer"));

        await page.Locator(".docs-shell__main").ClickAsync(new LocatorClickOptions
        {
            Position = new Position
            {
                X = 12,
                Y = 12
            }
        });

        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Dismissed: True");
    }
}

