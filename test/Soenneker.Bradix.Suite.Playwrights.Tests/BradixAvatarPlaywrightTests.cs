using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixAvatarPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixAvatarPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

[Fact]
    public async Task Avatar_demo_renders_loaded_image_and_keeps_fallback_hidden()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/avatar"));

        ILocator loadedAvatar = page.Locator("#avatar-loaded");

        await Assertions.Expect(loadedAvatar.GetByAltText("Colm Tuite", new LocatorGetByAltTextOptions { Exact = true })).ToBeVisibleAsync();
        await Assertions.Expect(loadedAvatar.GetByText("CT", new LocatorGetByTextOptions { Exact = true })).ToHaveCountAsync(0);
    }

[Fact]
    public async Task Avatar_demo_delayed_and_broken_cases_render_fallbacks_when_expected()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/avatar"));

        ILocator delayedAvatar = page.Locator("#avatar-delayed");
        ILocator brokenAvatar = page.Locator("#avatar-broken");
        ILocator fallbackOnlyAvatar = page.Locator("#avatar-fallback-only");

        await Assertions.Expect(delayedAvatar.GetByText("DL", new LocatorGetByTextOptions { Exact = true })).ToHaveCountAsync(0);
        await Assertions.Expect(brokenAvatar.GetByText("BR", new LocatorGetByTextOptions { Exact = true })).ToHaveCountAsync(0);
        await Assertions.Expect(fallbackOnlyAvatar.GetByText("PD", new LocatorGetByTextOptions { Exact = true })).ToBeVisibleAsync();

        await Assertions.Expect(delayedAvatar.GetByText("DL", new LocatorGetByTextOptions { Exact = true })).ToBeVisibleAsync();
        await Assertions.Expect(brokenAvatar.GetByText("BR", new LocatorGetByTextOptions { Exact = true })).ToBeVisibleAsync();
        await Assertions.Expect(brokenAvatar.GetByRole(AriaRole.Img)).ToHaveCountAsync(0);
    }
}

