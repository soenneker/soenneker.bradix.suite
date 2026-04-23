using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixAspectRatioPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixAspectRatioPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async Task Aspect_ratio_demo_preserves_landscape_square_and_portrait_geometry()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/aspect-ratio"));

        ILocator wrappers = page.Locator("[data-radix-aspect-ratio-wrapper]");
        Xunit.Assert.Equal(3, await wrappers.CountAsync());

        double[] ratios = await page.EvaluateAsync<double[]>(
            @"() => Array.from(document.querySelectorAll('[data-radix-aspect-ratio-wrapper]')).map(element => {
                const rect = element.getBoundingClientRect();
                return rect.width / rect.height;
            })");

        Xunit.Assert.InRange(ratios[0], 1.70, 1.85);
        Xunit.Assert.InRange(ratios[1], 0.95, 1.05);
        Xunit.Assert.InRange(ratios[2], 0.52, 0.60);
    }
}

