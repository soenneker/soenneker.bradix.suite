using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixSeparatorPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixSeparatorPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

    [Test]
    public async Task Separator_demo_exposes_semantic_horizontal_and_decorative_vertical_metadata()
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

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/separator"));

        ILocator semantic = page.Locator("[role='separator'][data-orientation='horizontal']").First;
        await Assertions.Expect(semantic).Not.ToHaveAttributeAsync("aria-orientation", new System.Text.RegularExpressions.Regex(".+"));

        ILocator decorativeVerticals = page.Locator("[role='none'][data-orientation='vertical']");
        await Assertions.Expect(decorativeVerticals).ToHaveCountAsync(2);
        await Assertions.Expect(decorativeVerticals.First).Not.ToHaveAttributeAsync("aria-orientation", new System.Text.RegularExpressions.Regex(".+"));

        consoleErrors.Should().BeEmpty();
        sawPageError.Should().BeFalse();
    }
}

