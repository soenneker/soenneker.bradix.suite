using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixRemoveScrollPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixRemoveScrollPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
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

    [Test]
    public async Task Remove_scroll_demo_locks_body_scroll_and_restores_styles_on_unmount()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/remove-scroll"));

        ILocator toggle = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Toggle scroll lock", Exact = true });

        await Assertions.Expect(page.GetByRole(AriaRole.Textbox)).ToBeVisibleAsync();
        await Assertions.Expect(page.Locator("body")).ToHaveCSSAsync("overflow", "hidden");

        string touchAction = await page.EvaluateAsync<string>("() => document.documentElement.style.touchAction");
        await Assert.That(touchAction).IsEqualTo(string.Empty);

        await toggle.ClickAsync();
        await Assertions.Expect(page.Locator("body")).Not.ToHaveCSSAsync("overflow", "hidden");

        string restoredTouchAction = await page.EvaluateAsync<string>("() => document.documentElement.style.touchAction");
        await Assert.That(restoredTouchAction).IsEqualTo(string.Empty);
    }
}

