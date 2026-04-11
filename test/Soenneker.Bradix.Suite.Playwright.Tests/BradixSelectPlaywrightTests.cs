using Microsoft.Playwright;
using Soenneker.Tests.FixturedUnit;
using System.Threading.Tasks;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwright.Tests;

[Collection("Collection")]
public sealed class BradixSelectPlaywrightTests : FixturedUnitTest
{
    private readonly Fixture _fixture;

    public BradixSelectPlaywrightTests(Fixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Select_demo_opens_options_and_updates_current_selection()
    {
        await using BrowserSession session = await _fixture.CreateSessionAsync();
        IPage page = session.Page;

        await page.GotoAndWaitForReadyAsync(
            $"{_fixture.BaseUrl}select",
            static p => p.Locator("[role='combobox']").First,
            expectedTitle: "Select Demo");

        ILocator trigger = page.Locator("[role='combobox']").First;

        await Assertions.Expect(trigger).ToContainTextAsync("Select a fruit");

        await trigger.ClickAsync();

        ILocator listBox = page.Locator("[role='listbox']:visible").First;
        await Assertions.Expect(listBox).ToBeVisibleAsync();
        ILocator options = listBox.Locator("[role='option']");
        await Assertions.Expect(options.Nth(0)).ToContainTextAsync("Apple");
        await Assertions.Expect(options.Nth(1)).ToContainTextAsync("Banana");
        await Assertions.Expect(options.Nth(2)).ToContainTextAsync("Blueberry");
        await Assertions.Expect(options.Nth(3)).ToContainTextAsync("Grapes");

        ILocator bananaOption = options.Nth(1);
        await bananaOption.ClickAsync();

        await Assertions.Expect(trigger).ToContainTextAsync("Banana");
        await Assertions.Expect(listBox).Not.ToBeVisibleAsync();
    }
}
