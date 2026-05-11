using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Soenneker.Playwrights.Extensions.TestPages;
using Soenneker.Playwrights.Session;
using Soenneker.Playwrights.Tests.Unit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixDemoPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixDemoPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

    [Test]
    public async ValueTask Overview_page_loads_and_lists_core_demo_links()
    {
        Logger.LogInformation("Initial loading complete");

        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady(
            BaseUrl,
            static p => p.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Bradix primitives" }),
            expectedTitle: "Bradix Component Library");

        await Assertions.Expect(page.GetByRole(AriaRole.Navigation, new PageGetByRoleOptions { Name = "Bradix primitives" })).ToBeVisibleAsync();
        var main = page.GetByRole(AriaRole.Main);
        await Assertions.Expect(main.GetByRole(AriaRole.Link, new LocatorGetByRoleOptions { Name = "Dialog", Exact = true })).ToBeVisibleAsync();
        await Assertions.Expect(main.GetByRole(AriaRole.Link, new LocatorGetByRoleOptions { Name = "Checkbox", Exact = true })).ToBeVisibleAsync();
    }
}

