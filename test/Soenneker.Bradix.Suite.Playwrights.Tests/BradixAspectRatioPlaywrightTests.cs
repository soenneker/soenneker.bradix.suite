using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixAspectRatioPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixAspectRatioPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

[Fact]
    public async Task Aspect_ratio_demo_preserves_landscape_square_and_portrait_geometry()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/aspect-ratio"));

        ILocator wrappers = page.Locator("[data-radix-aspect-ratio-wrapper]");
        Assert.Equal(3, await wrappers.CountAsync());

        double[] ratios = await page.EvaluateAsync<double[]>(
            @"() => Array.from(document.querySelectorAll('[data-radix-aspect-ratio-wrapper]')).map(element => {
                const rect = element.getBoundingClientRect();
                return rect.width / rect.height;
            })");

        Assert.InRange(ratios[0], 1.70, 1.85);
        Assert.InRange(ratios[1], 0.95, 1.05);
        Assert.InRange(ratios[2], 0.52, 0.60);
    }
}

