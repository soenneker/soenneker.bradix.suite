using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixDismissableLayerPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixDismissableLayerPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

[Fact]
    public async Task Dismissable_layer_demo_dismisses_on_outside_click()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dismissable-layer"));
        await Assertions.Expect(page.Locator(".portal-surface[data-bradix-dismissable-layer-ready='true']")).ToHaveCountAsync(1);

        ILocator layer = page.Locator(".portal-surface").First;
        LocatorBoundingBoxResult? layerBox = await layer.BoundingBoxAsync();
        Assert.NotNull(layerBox);

        double outsideX = Math.Max(8, layerBox.X - 24);
        double outsideY = Math.Max(8, layerBox.Y - 24);

        await page.Mouse.ClickAsync((float)outsideX, (float)outsideY);

        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Dismissed: True");
    }

[Fact]
    public async Task Dismissable_layer_demo_dismisses_on_escape()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dismissable-layer"));

        await page.Keyboard.PressAsync("Escape");

        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync("Dismissed: True");
    }
}

