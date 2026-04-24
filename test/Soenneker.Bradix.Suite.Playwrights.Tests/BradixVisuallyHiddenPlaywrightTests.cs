using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixVisuallyHiddenPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixVisuallyHiddenPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async Task Visually_hidden_demo_preserves_accessible_name_for_icon_button()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;
        var consoleErrors = new System.Collections.Generic.List<string>();
        var sawPageError = false;
        page.Console += (_, message) =>
        {
            if (message.Type == "error")
                consoleErrors.Add(message.Text);
        };
        page.PageError += (_, _) => sawPageError = true;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/visually-hidden"));

        ILocator button = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save the file", Exact = true });
        await Assertions.Expect(button).ToBeVisibleAsync();

        ILocator hiddenText = button.Locator("span").Filter(new LocatorFilterOptions { HasText = "Save the file" });
        await Assertions.Expect(hiddenText).ToHaveCSSAsync("position", "absolute");
        await Assertions.Expect(hiddenText).ToHaveCSSAsync("width", "1px");
        await Assertions.Expect(hiddenText).ToHaveCSSAsync("height", "1px");
        await Assertions.Expect(hiddenText).ToHaveCSSAsync("overflow", "hidden");
        await Assertions.Expect(hiddenText).ToHaveCSSAsync("white-space", "nowrap");

        consoleErrors.Should().BeEmpty();
        sawPageError.Should().BeFalse();
    }
}

