using System.Collections.Generic;
using AwesomeAssertions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixProgressPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixProgressPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async ValueTask Progress_demo_exposes_current_value()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;
        List<string> consoleErrors = [];
        var sawPageError = false;

        page.Console += (_, message) =>
        {
            if (message.Type == "error")
                consoleErrors.Add(message.Text);
        };

        page.PageError += (_, _) => sawPageError = true;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/progress"));

        ILocator loading = page.Locator("#progress-loading");
        ILocator complete = page.Locator("#progress-complete");
        ILocator indeterminate = page.Locator("#progress-indeterminate");

        await Assertions.Expect(loading).ToHaveAttributeAsync("aria-valuenow", "66");
        await Assertions.Expect(loading).ToHaveAttributeAsync("data-state", "loading");

        await Assertions.Expect(complete).ToHaveAttributeAsync("aria-valuenow", "100");
        await Assertions.Expect(complete).ToHaveAttributeAsync("data-state", "complete");

        await Assertions.Expect(indeterminate).ToHaveAttributeAsync("data-state", "indeterminate");
        await Assertions.Expect(indeterminate).Not.ToHaveAttributeAsync("aria-valuenow", new System.Text.RegularExpressions.Regex(".+"));
        sawPageError.Should().BeFalse();
        consoleErrors.Should().BeEmpty();
    }
}

