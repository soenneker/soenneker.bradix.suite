using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixFormPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixFormPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async ValueTask Form_demo_surfaces_required_and_type_mismatch_messages()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/form"));

        ILocator form = page.Locator("form");
        ILocator submit = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Post question", Exact = true });
        await page.WaitForTimeoutAsync(2000);
        await submit.ClickAsync(new LocatorClickOptions { Timeout = 2000 });
        await Assertions.Expect(form).ToContainTextAsync("Please enter your email", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
        await Assertions.Expect(form).ToContainTextAsync("Please enter a question", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });

        await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Email", Exact = true }).FillAsync("invalid");
        await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Question", Exact = true }).FillAsync("How does this work?");
        await submit.ClickAsync(new LocatorClickOptions { Timeout = 2000 });

        await Assertions.Expect(form).ToContainTextAsync("Please provide a valid email", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
    }
}

