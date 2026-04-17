using Soenneker.Playwrights.Extensions.TestPages;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixMenuPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixMenuPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

[Fact]
    public async Task Menu_demo_updates_selection_from_modal_submenu_item()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady(
            $"{BaseUrl}menu",
            static p => p.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open modal menu", Exact = true }),
            expectedTitle: "Menu Demo");

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open modal menu", Exact = true })
            .ClickAsync(new LocatorClickOptions { Timeout = 2000 });

        ILocator shareTrigger = page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "Share", Exact = true });
        ILocator copyLink = page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "Copy link", Exact = true });

        await shareTrigger.EvaluateAsync("element => element.click()");
        await Assertions.Expect(shareTrigger).ToHaveAttributeAsync("aria-expanded", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = 3000 });
        await page.WaitForTimeoutAsync(150);
        await copyLink.EvaluateAsync("element => element.click()");

        await Assertions.Expect(page.Locator(".docs-shell__content"))
            .ToContainTextAsync("Last selection: Copy link", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
    }
}

