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
        await Assertions.Expect(trigger)
                        .ToBeFocusedAsync();
    }

    [Fact]
    public async ValueTask Select_demo_portals_content_and_closes_on_outside_click()
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
        await Assertions.Expect(trigger)
                        .ToHaveAttributeAsync("data-state", "open");

        ILocator listBox = page.Locator("[role='listbox']:visible")
                               .First;
        await Assertions.Expect(listBox)
                        .ToBeVisibleAsync();
        await Assertions.Expect(listBox)
                        .ToHaveAttributeAsync("data-state", "open");

        bool renderedOutsideMain = await page.EvaluateAsync<bool>(
            "() => {" +
            "const listbox = document.querySelector('[role=\"listbox\"][data-state=\"open\"]');" +
            "const main = document.querySelector('main');" +
            "return !!listbox && document.body.contains(listbox) && !!main && !main.contains(listbox);" +
            "}");

        Assert.True(renderedOutsideMain);

        await ClickJustOutsideAsync(page, listBox);

        await Assertions.Expect(trigger)
                        .ToHaveAttributeAsync("aria-expanded", "false");
        await Assertions.Expect(listBox)
                        .Not.ToBeVisibleAsync();
    }

    [Fact]
    public async ValueTask Select_demo_marks_disabled_items_and_checked_selection_correctly()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady($"{BaseUrl}select", static p => p.Locator("[role='combobox']")
                                                                        .First, expectedTitle: "Select Demo");

        ILocator trigger = page.Locator("[role='combobox']")
                               .First;

        await trigger.ClickAsync();

        ILocator carrotOption = page.GetByRole(AriaRole.Option, new PageGetByRoleOptions { Name = "Carrot", Exact = true });
        await Assertions.Expect(carrotOption)
                        .ToHaveAttributeAsync("aria-disabled", "true");
        await Assertions.Expect(carrotOption)
                        .ToHaveAttributeAsync("data-disabled", "");
        await Assertions.Expect(carrotOption)
                        .ToHaveAttributeAsync("data-state", "unchecked");

        ILocator bananaOption = page.GetByRole(AriaRole.Option, new PageGetByRoleOptions { Name = "Banana", Exact = true });
        await bananaOption.ClickAsync();

        await trigger.ClickAsync();

        ILocator selectedBanana = page.GetByRole(AriaRole.Option, new PageGetByRoleOptions { Name = "Banana", Exact = true });
        await Assertions.Expect(selectedBanana)
                        .ToHaveAttributeAsync("data-state", "checked");
        await Assertions.Expect(selectedBanana)
                        .ToHaveAttributeAsync("aria-selected", "true");
    }

    [Fact]
    public async ValueTask Select_demo_supports_nested_select_inside_modal_dialog()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady($"{BaseUrl}select", static p => p.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open select dialog", Exact = true }),
            expectedTitle: "Select Demo");

        ILocator dialogTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open select dialog", Exact = true });
        await dialogTrigger.ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Select dialog", Exact = true });
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        ILocator trigger = dialog.GetByRole(AriaRole.Combobox, new LocatorGetByRoleOptions { Name = "Open nested select", Exact = true });
        await trigger.ClickAsync();

        ILocator listBox = page.GetByRole(AriaRole.Listbox);
        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(listBox).ToBeVisibleAsync();

        ILocator remix = listBox.GetByRole(AriaRole.Option, new LocatorGetByRoleOptions { Name = "Remix", Exact = true });
        await remix.ClickAsync();

        await Assertions.Expect(dialog).ToBeVisibleAsync();
        await Assertions.Expect(trigger).ToContainTextAsync("Remix");
    }

    private static async Task ClickJustOutsideAsync(IPage page, ILocator locator)
    {
        var box = await locator.BoundingBoxAsync();
        Assert.NotNull(box);
        float x = box.X > 40 ? box.X - 20 : box.X + box.Width + 20;
        float y = box.Y > 40 ? box.Y - 20 : box.Y + 20;
        await page.Mouse.ClickAsync(x, y);
    }
}
