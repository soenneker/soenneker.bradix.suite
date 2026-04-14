using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Extensions.TestPages;
using Soenneker.Playwrights.Session;
using Soenneker.Playwrights.Tests.Unit;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixDemoCatalogPlaywrightTests : PlaywrightUnitTest
{
    public BradixDemoCatalogPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

    [Theory]
    [MemberData(nameof(AllDemoRoutes))]
    public async ValueTask Every_demo_route_loads_intro_and_core_affordance(string route)
    {
        DemoPageSpec spec = DemoPageSpecs.Get(route);

        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, spec);
        await Assertions.Expect(page).ToHaveURLAsync(BaseUrl.GetRouteUrl(route));
    }

    public static global::System.Collections.Generic.IEnumerable<object[]> AllDemoRoutes()
    {
        return DemoPageSpecs.AllRoutes();
    }
}
