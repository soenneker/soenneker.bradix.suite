using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixPopperPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixPopperPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async Task Popper_demo_reports_initial_placement()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/popper"));

        await Assertions.Expect(page.Locator(".docs-shell__content")).ToContainTextAsync(new System.Text.RegularExpressions.Regex("Placed:\\s+[1-9]"));
    }

    [Test]
    public async Task Popper_demo_positions_content_below_anchor_with_arrow_offset_and_no_console_errors()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;
        var consoleMessages = new System.Collections.Generic.List<string>();
        var pageErrors = new System.Collections.Generic.List<string>();

        page.Console += (_, message) =>
        {
            if (message.Type is "error" or "warning")
                consoleMessages.Add($"{message.Type}: {message.Text}");
        };
        page.PageError += (_, exception) => pageErrors.Add(exception);

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/popper"));

        ILocator anchor = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Anchor", Exact = true });
        ILocator content = page.Locator(".popper-content");
        ILocator arrow = page.Locator(".popper-arrow");

        await Assertions.Expect(content).ToHaveAttributeAsync("data-side", "bottom");
        await Assertions.Expect(content).ToHaveCSSAsync("position", "fixed");
        await Assertions.Expect(content).ToHaveCSSAsync("z-index", "50");

        LocatorBoundingBoxResult? anchorBox = await anchor.BoundingBoxAsync();
        LocatorBoundingBoxResult? contentBox = await content.BoundingBoxAsync();
        LocatorBoundingBoxResult? arrowBox = await arrow.BoundingBoxAsync();

        await Assert.That(anchorBox).IsNotNull();
        await Assert.That(contentBox).IsNotNull();
        await Assert.That(arrowBox).IsNotNull();

        var expectedGap = (float)(8 + arrowBox!.Height);
        await Assert.That(contentBox!.Y).IsGreaterThanOrEqualTo(anchorBox!.Y + anchorBox.Height + expectedGap - 1);
        await Assert.That(contentBox.Y).IsLessThan(anchorBox.Y + anchorBox.Height + expectedGap + 4);
        await Assert.That(contentBox.Y).IsGreaterThan(anchorBox.Y + anchorBox.Height);

        await Assert.That(pageErrors).IsEmpty();
        await Assert.That(consoleMessages).IsEmpty();
    }
}

