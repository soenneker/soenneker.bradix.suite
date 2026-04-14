using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Extensions.TestPages;
using Soenneker.Playwrights.Session;
using Soenneker.Playwrights.Tests.Unit;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixSelectPlaywrightTests : PlaywrightUnitTest
{
    public BradixSelectPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

    [Fact]
    public async ValueTask Select_demo_opens_options_and_updates_current_selection()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady($"{BaseUrl}select", static p => p.Locator("[role='combobox']")
                                                                        .First, expectedTitle: "Select Demo");

        ILocator trigger = page.Locator("[role='combobox']")
                               .First;

        await Assertions.Expect(trigger)
                        .ToContainTextAsync("Select a fruit");

        await trigger.ClickAsync();

        ILocator listBox = page.Locator("[role='listbox']:visible")
                               .First;
        await Assertions.Expect(listBox)
                        .ToBeVisibleAsync();
        ILocator options = listBox.Locator("[role='option']");
        await Assertions.Expect(options.Nth(0))
                        .ToContainTextAsync("Apple");
        await Assertions.Expect(options.Nth(1))
                        .ToContainTextAsync("Banana");
        await Assertions.Expect(options.Nth(2))
                        .ToContainTextAsync("Blueberry");
        await Assertions.Expect(options.Nth(3))
                        .ToContainTextAsync("Grapes");

        ILocator bananaOption = options.Nth(1);
        await bananaOption.ClickAsync();

        await Assertions.Expect(trigger)
                        .ToContainTextAsync("Banana");
        await Assertions.Expect(listBox)
                        .Not.ToBeVisibleAsync();
    }

    [Fact]
    public async ValueTask Select_demo_single_click_keeps_listbox_open()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady($"{BaseUrl}select", static p => p.Locator("[role='combobox']")
                                                                        .First, expectedTitle: "Select Demo");

        ILocator trigger = page.Locator("[role='combobox']")
                               .First;

        await trigger.ClickAsync();

        await Assertions.Expect(trigger)
                        .ToHaveAttributeAsync("aria-expanded", "true");

        ILocator listBox = page.Locator("[role='listbox']:visible")
                               .First;
        await Assertions.Expect(listBox)
                        .ToBeVisibleAsync();

        await page.WaitForTimeoutAsync(250);

        await Assertions.Expect(trigger)
                        .ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(listBox)
                        .ToBeVisibleAsync();
    }
}