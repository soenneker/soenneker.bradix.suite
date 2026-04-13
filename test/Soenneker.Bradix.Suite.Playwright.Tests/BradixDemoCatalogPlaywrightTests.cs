using Microsoft.Playwright;
using Soenneker.Tests.FixturedUnit;
using System.Threading.Tasks;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwright.Tests;

[Collection("Collection")]
public sealed class BradixDemoCatalogPlaywrightTests : FixturedUnitTest
{
    private readonly Fixture _fixture;

    public BradixDemoCatalogPlaywrightTests(Fixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
        _fixture = fixture;
    }

    [Theory]
    [MemberData(nameof(AllDemoRoutes))]
    public async Task Every_demo_route_loads_intro_and_core_affordance(string route)
    {
        DemoPageSpec spec = DemoPageSpecs.Get(route);

        await using BrowserSession session = await _fixture.CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPageAsync(_fixture, spec);
        await Assertions.Expect(page).ToHaveURLAsync(_fixture.GetRouteUrl(route));
    }

    public static System.Collections.Generic.IEnumerable<object[]> AllDemoRoutes()
    {
        return DemoPageSpecs.AllRoutes();
    }
}
