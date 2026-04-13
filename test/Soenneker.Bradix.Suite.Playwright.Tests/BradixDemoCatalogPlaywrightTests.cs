using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

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

        await page.OpenDemoPage(_fixture, spec);
        await Assertions.Expect(page).ToHaveURLAsync(_fixture.GetRouteUrl(route));
    }

    public static System.Collections.Generic.IEnumerable<object[]> AllDemoRoutes()
    {
        return DemoPageSpecs.AllRoutes();
    }
}
