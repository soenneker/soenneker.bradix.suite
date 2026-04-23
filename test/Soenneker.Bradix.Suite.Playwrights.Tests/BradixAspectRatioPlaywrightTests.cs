using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

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
        await Assert.That(await wrappers.CountAsync()).IsEqualTo(3);

        double[] ratios = await page.EvaluateAsync<double[]>(
            @"() => Array.from(document.querySelectorAll('[data-radix-aspect-ratio-wrapper]')).map(element => {
                const rect = element.getBoundingClientRect();
                return rect.width / rect.height;
            })");

        await Assert.That(ratios[0] >= 1.70 && ratios[0] <= 1.85).IsTrue();
        await Assert.That(ratios[1] >= 0.95 && ratios[1] <= 1.05).IsTrue();
        await Assert.That(ratios[2] >= 0.52 && ratios[2] <= 0.60).IsTrue();
    }
}

