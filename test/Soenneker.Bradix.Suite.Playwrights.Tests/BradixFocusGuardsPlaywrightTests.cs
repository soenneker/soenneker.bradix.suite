using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixFocusGuardsPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixFocusGuardsPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

    [Test]
    public async Task Focus_guards_demo_mounts_body_edge_sentinels_and_removes_them_on_unmount()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/focus-guards"));

        ILocator guards = page.Locator("[data-radix-focus-guard]");
        await Assertions.Expect(guards).ToHaveCountAsync(2);

        bool guardsAreBodyEdges = await page.EvaluateAsync<bool>(
            """
            () => document.body.firstElementChild?.hasAttribute('data-radix-focus-guard') === true
                && document.body.lastElementChild?.hasAttribute('data-radix-focus-guard') === true
            """);

        await Assert.That(guardsAreBodyEdges).IsTrue();

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Toggle guards", Exact = true }).ClickAsync();

        await Assertions.Expect(guards).ToHaveCountAsync(0);

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Toggle guards", Exact = true }).ClickAsync();

        await Assertions.Expect(guards).ToHaveCountAsync(2);
    }
}
