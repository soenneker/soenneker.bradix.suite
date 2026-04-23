using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Extensions.TestPages;
using Soenneker.Playwrights.Session;
using Soenneker.Playwrights.Tests.Unit;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixSelectPlaywrightTests : PlaywrightUnitTest
{
    public BradixSelectPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

    [Test]
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

    [Test]
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

        Xunit.Assert.True(renderedOutsideMain);

        await ClickJustOutsideAsync(page, listBox);

        await Assertions.Expect(trigger)
                        .ToHaveAttributeAsync("aria-expanded", "false");
        await Assertions.Expect(listBox)
                        .Not.ToBeVisibleAsync();
    }

    [Test]
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

    [Test]
    public async ValueTask Select_demo_home_and_end_keys_move_focus_to_first_and_last_enabled_options()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady(
            $"{BaseUrl}select",
            static p => p.Locator("[role='combobox']").First,
            expectedTitle: "Select Demo");

        ILocator trigger = page.Locator("[role='combobox']").First;
        await trigger.ClickAsync();

        ILocator listbox = page.Locator("[role='listbox']:visible").First;
        ILocator apple = page.GetByRole(AriaRole.Option, new PageGetByRoleOptions { Name = "Apple", Exact = true });
        ILocator pork = page.GetByRole(AriaRole.Option, new PageGetByRoleOptions { Name = "Pork", Exact = true });

        await Assertions.Expect(apple).ToHaveAttributeAsync("data-highlighted", string.Empty);

        await listbox.FocusAsync();
        await page.Keyboard.PressAsync("End");

        await Assertions.Expect(pork).ToHaveAttributeAsync("data-highlighted", string.Empty);

        await listbox.FocusAsync();
        await page.Keyboard.PressAsync("Home");

        await Assertions.Expect(apple).ToHaveAttributeAsync("data-highlighted", string.Empty);
    }

    [Test]
    public async ValueTask Select_demo_typeahead_moves_focus_to_matching_enabled_option()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady(
            $"{BaseUrl}select",
            static p => p.Locator("[role='combobox']").First,
            expectedTitle: "Select Demo");

        ILocator trigger = page.Locator("[role='combobox']").First;
        await trigger.ClickAsync();

        string? contentId = await trigger.GetAttributeAsync("aria-controls");
        Xunit.Assert.False(string.IsNullOrWhiteSpace(contentId));

        ILocator listbox = page.Locator($"#{contentId}");
        ILocator highlightedGrapes = listbox.Locator("[role='option'][data-highlighted]").Filter(new LocatorFilterOptions { HasText = "Grapes" });

        await listbox.FocusAsync();
        await page.Keyboard.PressAsync("g");

        Xunit.Assert.True(await highlightedGrapes.CountAsync() > 0, "Expected a highlighted Grapes option after typeahead.");
    }

    [Test]
    public async ValueTask Select_demo_typeahead_skips_disabled_matching_option()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady(
            $"{BaseUrl}select",
            static p => p.Locator("[role='combobox']").First,
            expectedTitle: "Select Demo");

        ILocator trigger = page.Locator("[role='combobox']").First;
        await trigger.ClickAsync();

        string? contentId = await trigger.GetAttributeAsync("aria-controls");
        Xunit.Assert.False(string.IsNullOrWhiteSpace(contentId));

        ILocator listbox = page.Locator($"#{contentId}");
        ILocator carrot = listbox.Locator("[role='option']:visible").Filter(new LocatorFilterOptions { HasText = "Carrot" }).First;
        ILocator highlightedCourgette = listbox.Locator("[role='option'][data-highlighted]").Filter(new LocatorFilterOptions { HasText = "Courgette" });

        await Assertions.Expect(carrot).ToHaveAttributeAsync("aria-disabled", "true");

        await listbox.FocusAsync();
        await page.Keyboard.PressAsync("c");

        Xunit.Assert.True(await highlightedCourgette.CountAsync() > 0, "Expected a highlighted Courgette option after typeahead.");
        await Assertions.Expect(carrot).Not.ToHaveAttributeAsync("data-highlighted", string.Empty);
    }

    [Test]
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

    [Test]
    public async ValueTask Select_demo_native_form_requires_selection_and_submits_selected_value()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.GotoAndWaitForReady(
            $"{BaseUrl}select",
            static p => p.GetByTestId("select-native-form-result"),
            expectedTitle: "Select Demo");

        ILocator form = page.GetByTestId("select-native-form");
        ILocator result = page.GetByTestId("select-native-form-result");
        ILocator hiddenSelect = page.Locator("select[name='framework']");

        await Assertions.Expect(result).ToContainTextAsync("No submission yet.");
        await Assertions.Expect(hiddenSelect).ToHaveAttributeAsync("required", string.Empty);
        bool isInitiallyValid = await hiddenSelect.EvaluateAsync<bool>("element => element.checkValidity()");
        Xunit.Assert.False(isInitiallyValid);
        ILocator trigger = form.GetByRole(AriaRole.Combobox, new LocatorGetByRoleOptions { Name = "Select framework", Exact = true });
        await trigger.ClickAsync();
        ILocator listBox = page.Locator("[role='listbox']:visible").First;
        await Assertions.Expect(listBox).ToBeVisibleAsync();
        ILocator astroOption = listBox.GetByRole(AriaRole.Option, new LocatorGetByRoleOptions { Name = "Astro", Exact = true });
        await astroOption.FocusAsync();
        await Assertions.Expect(astroOption).ToBeFocusedAsync();
        await page.Keyboard.PressAsync("Enter");

        await Assertions.Expect(trigger).ToContainTextAsync("Astro");
        await Assertions.Expect(hiddenSelect).ToHaveValueAsync("astro");
        bool isValidAfterSelection = await hiddenSelect.EvaluateAsync<bool>("element => element.checkValidity()");
        Xunit.Assert.True(isValidAfterSelection);

        await form.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Submit native form", Exact = true }).ClickAsync();
        await Assertions.Expect(result).ToContainTextAsync("framework=astro");
    }

    private static async Task ClickJustOutsideAsync(IPage page, ILocator locator)
    {
        var box = await locator.BoundingBoxAsync();
        Xunit.Assert.NotNull(box);
        float x = box.X > 40 ? box.X - 20 : box.X + box.Width + 20;
        float y = box.Y > 40 ? box.Y - 20 : box.Y + 20;
        await page.Mouse.ClickAsync(x, y);
    }
}

