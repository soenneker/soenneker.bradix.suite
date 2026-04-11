using Microsoft.Playwright;
using Soenneker.Tests.FixturedUnit;
using System.Threading.Tasks;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwright.Tests;

[Collection("Collection")]
public sealed class BradixDemoPlaywrightTests : FixturedUnitTest
{
    private readonly Fixture _fixture;

    public BradixDemoPlaywrightTests(Fixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Overview_page_loads_and_lists_core_demo_links()
    {
        await using BrowserSession session = await _fixture.CreateSessionAsync();
        IPage page = session.Page;

        await page.GotoAndWaitForReadyAsync(
            _fixture.BaseUrl,
            static p => p.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Bradix primitives" }),
            expectedTitle: "Bradix Component Library Demo");

        await Assertions.Expect(page.GetByRole(AriaRole.Navigation, new PageGetByRoleOptions { Name = "Bradix primitives" })).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Dialog", Exact = true })).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Checkbox", Exact = true })).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Dialog_demo_saves_updated_project_details()
    {
        await using BrowserSession session = await _fixture.CreateSessionAsync();
        IPage page = session.Page;

        await page.GotoAndWaitForReadyAsync(
            $"{_fixture.BaseUrl}dialog",
            static p => p.Locator("section.card").First.GetByRole(AriaRole.Heading, new LocatorGetByRoleOptions { Name = "Modal dialog", Exact = true }),
            expectedTitle: "Bradix Dialog");

        ILocator modalCard = page.Locator("section.card").First;
        ILocator stateLines = modalCard.Locator(".state-line");

        await Assertions.Expect(stateLines.Nth(0)).ToContainTextAsync("Quark Gateway");
        await Assertions.Expect(stateLines.Nth(1)).ToContainTextAsync("Platform team");
        await Assertions.Expect(stateLines.Nth(2)).ToContainTextAsync("Production");

        await modalCard.GetByText("Edit project details").ClickAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Edit deployment project" })).ToBeVisibleAsync();

        await page.GetByPlaceholder("Project name").FillAsync("Quark Gateway UI");
        await page.GetByPlaceholder("Owner").FillAsync("QA");
        await page.GetByPlaceholder("Environment").FillAsync("Staging");
        await page.GetByText("Save changes").ClickAsync();

        await Assertions.Expect(stateLines.Nth(0)).ToContainTextAsync("Quark Gateway UI");
        await Assertions.Expect(stateLines.Nth(1)).ToContainTextAsync("QA");
        await Assertions.Expect(stateLines.Nth(2)).ToContainTextAsync("Staging");
    }
}
