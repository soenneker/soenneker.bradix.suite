using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Soenneker.Playwrights.Extensions.TestPages;
using Soenneker.Playwrights.Session;
using Soenneker.Playwrights.Tests.Unit;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixDemoPlaywrightTests : PlaywrightUnitTest
{
    public BradixDemoPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

    [Fact]
    public async ValueTask Overview_page_loads_and_lists_core_demo_links()
    {
        Logger.LogInformation("Initial loading complete");

        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady(
            BaseUrl,
            static p => p.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Bradix primitives" }),
            expectedTitle: "Bradix Component Library Demo");

        await Assertions.Expect(page.GetByRole(AriaRole.Navigation, new PageGetByRoleOptions { Name = "Bradix primitives" })).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Dialog", Exact = true })).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Checkbox", Exact = true })).ToBeVisibleAsync();
    }
}

