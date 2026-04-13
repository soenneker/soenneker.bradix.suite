using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

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
        await using BrowserSession session = await _fixture.CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady(
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
        await using BrowserSession session = await _fixture.CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady(
            $"{_fixture.BaseUrl}dialog",
            static p => p.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true }),
            expectedTitle: "Bradix Dialog");

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Edit profile", Exact = true })).ToBeVisibleAsync();

        await page.Locator("#dialog-name").FillAsync("Jake");
        await page.Locator("#dialog-username").FillAsync("@jake");
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save changes", Exact = true }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true }).ClickAsync();
        await Assertions.Expect(page.Locator("#dialog-name")).ToHaveValueAsync("Jake");
        await Assertions.Expect(page.Locator("#dialog-username")).ToHaveValueAsync("@jake");
    }
}
