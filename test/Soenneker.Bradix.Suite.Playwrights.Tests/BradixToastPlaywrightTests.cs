using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixToastPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixToastPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

[Fact]
    public async ValueTask Toast_demo_shows_scheduled_notification()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toast"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add to calendar", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByText("Scheduled: Catch up", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByText("Undo", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();
    }

[Fact]
    public async ValueTask Toast_demo_action_dismisses_the_current_notification()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toast"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add to calendar", Exact = true }).ClickAsync();

        ILocator toast = page.Locator(".toast-demo__root[data-state='open']").Filter(new LocatorFilterOptions { HasText = "Scheduled: Catch up" });
        await Assertions.Expect(toast).ToBeVisibleAsync();

        await Assertions.Expect(toast.GetByText("Undo", new LocatorGetByTextOptions { Exact = true })).ToBeVisibleAsync();
        await toast.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Goto schedule to undo", Exact = true }).ClickAsync();

        await Assertions.Expect(toast).Not.ToBeVisibleAsync();
    }
}

