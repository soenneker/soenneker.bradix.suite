using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixRemoveScrollPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixRemoveScrollPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

[Fact]
    public async Task Remove_scroll_demo_mount_toggle_shows_and_hides_locked_surface()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/remove-scroll"));

        ILocator textarea = page.GetByRole(AriaRole.Textbox);
        ILocator toggle = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Toggle scroll lock", Exact = true });

        await Assertions.Expect(textarea).ToBeVisibleAsync();

        await toggle.ClickAsync();
        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Mounted: False");
        await Assertions.Expect(textarea).ToHaveCountAsync(0);

        await toggle.ClickAsync();
        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Mounted: True");
        await Assertions.Expect(page.GetByRole(AriaRole.Textbox)).ToBeVisibleAsync();
    }
}

