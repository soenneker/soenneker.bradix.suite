using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixPresencePlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixPresencePlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

[Fact]
    public async Task Presence_demo_runs_exit_completion_when_toggled_closed()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/presence"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Toggle presence", Exact = true }).ClickAsync();

        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Present: False");
        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Exit complete count: 1");
    }
}

