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
        await Assert.That(scrollLeft > 0).IsTrue();
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
        await Assert.That(verticalScrollTop > 0).IsTrue();

        int horizontalScrollLeft = await page.EvaluateAsync<int>(
            "() => { const viewport = document.querySelector('.scroll-area-demo__horizontal [data-radix-scroll-area-viewport]'); if (!viewport) return -1; viewport.scrollLeft = 200; return viewport.scrollLeft; }");
        await Assert.That(horizontalScrollLeft > 0).IsTrue();

        ILocator rtlRoot = page.Locator("section").Filter(new LocatorFilterOptions { HasText = "RTL notes" })
                               .Locator(".scroll-area-root[dir='rtl']")
                               .First;
        await Assertions.Expect(rtlRoot).ToHaveAttributeAsync("dir", "rtl");

        int rtlScrollTop = await page.EvaluateAsync<int>(
            "() => { const viewport = document.querySelector('[dir=\"rtl\"] [data-radix-scroll-area-viewport]'); if (!viewport) return -1; viewport.scrollTop = 160; return viewport.scrollTop; }");
        await Assert.That(rtlScrollTop > 0).IsTrue();
    }

[Test]
    public async Task Scroll_area_demo_allows_viewport_scrolling()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/scroll-area"));

        int scrollTop = await page.EvaluateAsync<int>(
            "() => { const viewport = document.querySelector('[data-radix-scroll-area-viewport]'); if (!viewport) return -1; viewport.scrollTop = 200; return viewport.scrollTop; }");
        await Assert.That(scrollTop > 0).IsTrue();
    }

    [Test]
    public async Task Scroll_area_demo_custom_vertical_scrollbar_wheel_scrolls_viewport()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/scroll-area"));

        ILocator firstDemo = page.Locator(".website-demo-card").First;
        ILocator scrollbar = firstDemo.Locator("[data-orientation='vertical']");
        ILocator thumb = scrollbar.Locator(".scroll-area-thumb");

        await Assertions.Expect(scrollbar).ToHaveAttributeAsync("data-state", "visible");
        await Assertions.Expect(thumb).ToBeVisibleAsync();

        await scrollbar.HoverAsync();
        await page.Mouse.WheelAsync(0, 220);

        int scrollTop = await firstDemo.Locator("[data-radix-scroll-area-viewport]").EvaluateAsync<int>("element => element.scrollTop");
        await Assert.That(scrollTop > 0).IsTrue();
    }
}

