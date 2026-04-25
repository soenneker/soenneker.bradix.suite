using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AwesomeAssertions;
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
        List<string> consoleErrors = [];
        var sawPageError = false;

        page.Console += (_, message) =>
        {
            if (message.Type == "error")
                consoleErrors.Add(message.Text);
        };

        page.PageError += (_, _) => sawPageError = true;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/form"));

        ILocator form = page.Locator("form");
        ILocator email = page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Email", Exact = true });
        ILocator question = page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Question", Exact = true });
        ILocator submit = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Post question", Exact = true });
        await page.WaitForTimeoutAsync(2000);
        await submit.ClickAsync(new LocatorClickOptions { Timeout = 2000 });
        await Assertions.Expect(form).ToContainTextAsync("Please enter your email", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
        await Assertions.Expect(form).ToContainTextAsync("Please enter a question", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
        await Assertions.Expect(email).ToBeFocusedAsync();

        await email.FillAsync("invalid");
        await question.FillAsync("How does this work?");
        await submit.ClickAsync(new LocatorClickOptions { Timeout = 2000 });

        await Assertions.Expect(form).ToContainTextAsync("Please provide a valid email", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
        await Assertions.Expect(email).ToHaveAttributeAsync("aria-describedby", new Regex(".+"));
        await Assertions.Expect(email).ToHaveAttributeAsync("title", string.Empty);

        sawPageError.Should().BeFalse();
        consoleErrors.Should().BeEmpty();
    }

    [Test]
    public async ValueTask Form_demo_runs_async_custom_matcher_through_browser_events()
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

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/form"));

        ILocator form = page.Locator("form");
        ILocator username = page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Username", Exact = true });
        ILocator question = page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Question", Exact = true });

        await username.FillAsync("taken");
        await question.FocusAsync();

        await Assertions.Expect(form).ToContainTextAsync("Username is already taken", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
        await Assertions.Expect(username).ToHaveJSPropertyAsync("validationMessage", "This value is not valid");
        await Assertions.Expect(username).ToHaveAttributeAsync("aria-describedby", new Regex(".+"));

        await username.FillAsync("available");
        await question.FocusAsync();

        await Assertions.Expect(form).Not.ToContainTextAsync("Username is already taken", new LocatorAssertionsToContainTextOptions { Timeout = 3000 });
        await Assertions.Expect(username).ToHaveJSPropertyAsync("validationMessage", string.Empty);

        sawPageError.Should().BeFalse();
        consoleErrors.Should().BeEmpty();
    }
}

