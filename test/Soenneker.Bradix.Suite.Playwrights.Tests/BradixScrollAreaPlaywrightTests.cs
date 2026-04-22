using Soenneker.Playwrights.Extensions.TestPages;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixScrollAreaPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixScrollAreaPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async Task Scroll_area_demo_supports_horizontal_viewport_scrolling()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/scroll-area"));

        int scrollLeft = await page.EvaluateAsync<int>(
            "() => { const viewport = document.querySelector('.scroll-area-demo__horizontal [data-radix-scroll-area-viewport]'); if (!viewport) return -1; viewport.scrollLeft = 200; return viewport.scrollLeft; }");
        Assert.True(scrollLeft > 0);
    }

[Test]
    public async Task Scroll_area_demo_supports_vertical_horizontal_and_rtl_viewport_scrolling()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenPage(BaseUrl, "/scroll-area", async currentPage =>
        {
            await Assertions.Expect(currentPage.Locator(".scroll-area-demo__horizontal")).ToBeVisibleAsync();
            await Assertions.Expect(currentPage.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "RTL notes", Exact = true })).ToBeVisibleAsync();
        });

        int verticalScrollTop = await page.EvaluateAsync<int>(
            "() => { const viewport = document.querySelector('[data-radix-scroll-area-viewport]'); if (!viewport) return -1; viewport.scrollTop = 200; return viewport.scrollTop; }");
        Assert.True(verticalScrollTop > 0);

        int horizontalScrollLeft = await page.EvaluateAsync<int>(
            "() => { const viewport = document.querySelector('.scroll-area-demo__horizontal [data-radix-scroll-area-viewport]'); if (!viewport) return -1; viewport.scrollLeft = 200; return viewport.scrollLeft; }");
        Assert.True(horizontalScrollLeft > 0);

        ILocator rtlRoot = page.Locator("section").Filter(new LocatorFilterOptions { HasText = "RTL notes" })
                               .Locator(".scroll-area-root[dir='rtl']")
                               .First;
        await Assertions.Expect(rtlRoot).ToHaveAttributeAsync("dir", "rtl");

        int rtlScrollTop = await page.EvaluateAsync<int>(
            "() => { const viewport = document.querySelector('[dir=\"rtl\"] [data-radix-scroll-area-viewport]'); if (!viewport) return -1; viewport.scrollTop = 160; return viewport.scrollTop; }");
        Assert.True(rtlScrollTop > 0);
    }

[Test]
    public async Task Scroll_area_demo_allows_viewport_scrolling()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/scroll-area"));

        int scrollTop = await page.EvaluateAsync<int>(
            "() => { const viewport = document.querySelector('[data-radix-scroll-area-viewport]'); if (!viewport) return -1; viewport.scrollTop = 200; return viewport.scrollTop; }");
        Assert.True(scrollTop > 0);
    }
}

