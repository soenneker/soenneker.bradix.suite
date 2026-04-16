using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Soenneker.Playwrights.Tests.Unit;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixDisclosurePlaywrightTests : PlaywrightUnitTest
{
    public BradixDisclosurePlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

    [Fact]
    public async ValueTask Accordion_demo_switches_visible_content_between_items()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/accordion"));

        ILocator accessibleTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Is it accessible?", Exact = true });
        ILocator unstyledTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Is it unstyled?", Exact = true });

        await Assertions.Expect(accessibleTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        await unstyledTrigger.ClickAsync();

        await Assertions.Expect(unstyledTrigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(accessibleTrigger).ToHaveAttributeAsync("aria-expanded", "false");
    }

    [Fact]
    public async ValueTask Accordion_demo_supports_multiple_items_without_closing_previous_content()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/accordion"));

        ILocator multipleDemo = page.GetByRole(AriaRole.Region, new PageGetByRoleOptions { Name = "Multiple accordion demo", Exact = true });
        ILocator firstTrigger = multipleDemo.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Can I open more than one item?", Exact = true });
        ILocator secondTrigger = multipleDemo.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Will the first item stay open?", Exact = true });

        await Assertions.Expect(firstTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        await secondTrigger.ClickAsync();

        await Assertions.Expect(firstTrigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(secondTrigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(multipleDemo.GetByText("Multiple mode keeps previously expanded content available while you open another item.", new LocatorGetByTextOptions { Exact = true })).ToBeVisibleAsync();
        await Assertions.Expect(multipleDemo.GetByText("Yes. Radix keeps earlier items expanded until you explicitly close them.", new LocatorGetByTextOptions { Exact = true })).ToBeVisibleAsync();
    }

    [Fact]
    public async ValueTask Accordion_demo_skips_disabled_items_and_honors_orientation_specific_keyboard_navigation()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/accordion"));

        ILocator singleDemo = page.GetByRole(AriaRole.Region, new PageGetByRoleOptions { Name = "Single accordion demo", Exact = true });
        ILocator accessibleTrigger = singleDemo.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Is it accessible?", Exact = true });
        ILocator unstyledTrigger = singleDemo.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Is it unstyled?", Exact = true });
        ILocator animatedTrigger = singleDemo.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Can it be animated?", Exact = true });

        await accessibleTrigger.PressAsync("ArrowDown");
        await ExpectActiveElementAsync(page, unstyledTrigger);

        await unstyledTrigger.PressAsync("End");
        await ExpectActiveElementAsync(page, animatedTrigger);

        await animatedTrigger.PressAsync("Home");
        await ExpectActiveElementAsync(page, accessibleTrigger);

        ILocator disabledDemo = page.GetByRole(AriaRole.Region, new PageGetByRoleOptions { Name = "Disabled accordion demo", Exact = true });
        ILocator enabledTrigger = disabledDemo.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Can I access account history?", Exact = true });
        ILocator disabledTrigger = disabledDemo.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Premium features", Exact = true });
        ILocator trailingTrigger = disabledDemo.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "How do I update my email?", Exact = true });

        await Assertions.Expect(disabledTrigger).ToBeDisabledAsync();

        await enabledTrigger.PressAsync("ArrowDown");
        await ExpectActiveElementAsync(page, trailingTrigger);

        await disabledTrigger.ClickAsync(new LocatorClickOptions { Force = true });
        await Assertions.Expect(disabledTrigger).ToHaveAttributeAsync("aria-expanded", "false");
        await Assertions.Expect(disabledDemo.GetByText("Disabled items should not open or receive roving focus.", new LocatorGetByTextOptions { Exact = true })).Not.ToBeVisibleAsync();

        ILocator horizontalDemo = page.GetByRole(AriaRole.Region, new PageGetByRoleOptions { Name = "Horizontal RTL accordion demo", Exact = true });
        ILocator firstHorizontalTrigger = horizontalDemo.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "First horizontal item", Exact = true });
        ILocator secondHorizontalTrigger = horizontalDemo.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Second horizontal item", Exact = true });
        ILocator thirdHorizontalTrigger = horizontalDemo.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Third horizontal item", Exact = true });

        await firstHorizontalTrigger.PressAsync("ArrowLeft");
        await ExpectActiveElementAsync(page, secondHorizontalTrigger);

        await secondHorizontalTrigger.PressAsync("ArrowRight");
        await ExpectActiveElementAsync(page, firstHorizontalTrigger);

        await firstHorizontalTrigger.PressAsync("ArrowRight");
        await ExpectActiveElementAsync(page, thirdHorizontalTrigger);
    }

    [Fact]
    public async ValueTask Alert_dialog_demo_opens_and_closes_from_cancel()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/alert-dialog"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Delete account", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Are you absolutely sure?", Exact = true })).ToBeVisibleAsync();
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Cancel", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Are you absolutely sure?", Exact = true })).Not.ToBeVisibleAsync();
    }

    [Fact]
    public async ValueTask Alert_dialog_demo_can_disable_escape_dismissal()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/alert-dialog"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Guard escape dismissal", Exact = true }).ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Alertdialog, new PageGetByRoleOptions { Name = "Stay open on escape", Exact = true });
        await Assertions.Expect(dialog).ToHaveAttributeAsync("data-state", "open");

        await page.Keyboard.PressAsync("Escape");

        await Assertions.Expect(dialog).ToHaveAttributeAsync("data-state", "open");

        await dialog.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Cancel", Exact = true }).ClickAsync();

        await Assertions.Expect(dialog).Not.ToBeVisibleAsync();
    }

    [Fact]
    public async ValueTask Collapsible_demo_reveals_additional_repositories_when_opened()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/collapsible"));

        ILocator trigger = page.GetByRole(AriaRole.Button);
        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "false");

        await trigger.ClickAsync();

        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(page.Locator(".collapsible-demo__root")).ToContainTextAsync("@radix-ui/colors");
        await Assertions.Expect(page.Locator(".collapsible-demo__root")).ToContainTextAsync("@radix-ui/themes");
    }

    [Fact]
    public async ValueTask Dialog_demo_close_discards_unsaved_changes()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dialog"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true }).ClickAsync();
        await page.Locator("#dialog-name").FillAsync("Unsaved");
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Close", Exact = true }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true }).ClickAsync();
        await Assertions.Expect(page.Locator("#dialog-name")).ToHaveValueAsync("Pedro Duarte");
    }

    [Fact]
    public async ValueTask Dialog_demo_traps_focus_and_restores_trigger_focus_after_escape()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dialog"));

        ILocator trigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true });
        await trigger.ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Edit profile", Exact = true });

        await Assertions.Expect(dialog).ToBeVisibleAsync();
        await Assertions.Expect(dialog).ToHaveAttributeAsync("aria-modal", "true");

        for (var i = 0; i < 4; i++)
        {
            await page.Keyboard.PressAsync("Tab");
            Assert.True(await FocusIsWithinAsync(dialog));
        }

        await page.Keyboard.PressAsync("Escape");

        await Assertions.Expect(dialog).Not.ToBeVisibleAsync();
        await Assertions.Expect(trigger).ToBeFocusedAsync();
    }

    [Fact]
    public async ValueTask Dialog_demo_dismisses_from_overlay_click()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dialog"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true }).ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Edit profile", Exact = true });
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        await ClickJustOutsideActiveDialogAsync(page, dialog);

        await Assertions.Expect(dialog).Not.ToBeVisibleAsync();
    }

    [Fact]
    public async ValueTask Dialog_demo_supports_popover_nested_inside_modal_dialog()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/dialog"));

        ILocator dialogTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open nested overlay dialog", Exact = true });
        await dialogTrigger.ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Nested overlay dialog", Exact = true });
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        ILocator popoverTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open nested popover", Exact = true });
        await popoverTrigger.ClickAsync();

        ILocator popover = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Nested popover", Exact = true });
        await Assertions.Expect(popoverTrigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(popover).ToBeVisibleAsync();

        await page.Keyboard.PressAsync("Escape");

        await Assertions.Expect(popover).Not.ToBeVisibleAsync();
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        ILocator main = page.Locator(".docs-shell__main");
        var mainBox = await main.BoundingBoxAsync();
        Assert.NotNull(mainBox);
        await page.Mouse.ClickAsync(mainBox.X + mainBox.Width - 10, mainBox.Y + mainBox.Height - 10);

        await Assertions.Expect(dialog).Not.ToBeVisibleAsync();
    }

    [Fact]
    public async ValueTask Hover_card_demo_shows_profile_details_on_hover()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/hover-card"));

        await page.GetByAltText("Radix UI").HoverAsync();

        await Assertions.Expect(page.GetByText("@radix_ui")).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByText("2,900")).ToBeVisibleAsync();
    }

    [Fact]
    public async ValueTask Hover_card_demo_supports_nested_hover_card_inside_modal_dialog()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/hover-card"));

        ILocator dialogTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open hover card dialog", Exact = true });
        await dialogTrigger.ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Hover card dialog", Exact = true });
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        ILocator hoverCardTrigger = dialog.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Show nested hover card", Exact = true });
        await hoverCardTrigger.HoverAsync();

        ILocator hoverCard = page.GetByText("Hover card content inside dialog", new PageGetByTextOptions { Exact = true });
        await Assertions.Expect(hoverCard).ToBeVisibleAsync();
        await Assertions.Expect(dialog).ToBeVisibleAsync();
    }

    [Fact]
    public async ValueTask Popover_demo_opens_and_closes_from_close_button()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/popover"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Update dimensions", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByText("Dimensions", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Close", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByText("Dimensions", new PageGetByTextOptions { Exact = true })).Not.ToBeVisibleAsync();
    }

    [Fact]
    public async ValueTask Popover_demo_preserves_explicit_content_role_over_dialog_default()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/popover"));

        ILocator trigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open custom listbox", Exact = true });
        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "true");

        var popupState = await page.EvaluateAsync<PopoverRoleProbe>(
            @"() => {
                const popup = document.querySelector('[role=""listbox""][aria-label=""Framework choices""]');
                const astro = popup?.querySelector('[role=""option""][aria-selected=""true""]');

                return {
                    role: popup?.getAttribute('role'),
                    ariaLabel: popup?.getAttribute('aria-label'),
                    dataState: popup?.getAttribute('data-state'),
                    selectedText: astro?.textContent?.trim()
                };
            }");

        Assert.NotNull(popupState);
        Assert.Equal("listbox", popupState.role);
        Assert.Equal("Framework choices", popupState.ariaLabel);
        Assert.Equal("open", popupState.dataState);
        Assert.Equal("Astro", popupState.selectedText);
        await Assertions.Expect(page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);
    }

    [Fact]
    public async ValueTask Popover_demo_closes_from_outside_click_after_opening_from_trigger()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/popover"));

        ILocator trigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Update dimensions", Exact = true });
        await trigger.ClickAsync();

        ILocator content = page.Locator(".popover-demo__content[data-state='open']").Filter(new LocatorFilterOptions { HasText = "Dimensions" });

        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(content).ToBeVisibleAsync();

        ILocator main = page.Locator(".docs-shell__main");
        var mainBox = await main.BoundingBoxAsync();
        Assert.NotNull(mainBox);
        await page.Mouse.ClickAsync(mainBox.X + mainBox.Width - 10, mainBox.Y + mainBox.Height - 10);

        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "false");
        await Assertions.Expect(content).Not.ToBeVisibleAsync();
    }

    private sealed class PopoverRoleProbe
    {
        public string? role { get; set; }
        public string? ariaLabel { get; set; }
        public string? dataState { get; set; }
        public string? selectedText { get; set; }
    }

    [Fact]
    public async ValueTask Toast_demo_shows_scheduled_notification()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toast"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add to calendar", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByText("Scheduled: Catch up", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByText("Undo", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();
    }

    [Fact]
    public async ValueTask Tooltip_demo_reveals_content_on_hover()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/tooltip"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "+", Exact = true }).HoverAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Tooltip, new PageGetByRoleOptions { Name = "Add to library", Exact = true })).ToBeVisibleAsync();
    }

    [Fact]
    public async ValueTask Tooltip_demo_supports_nested_tooltip_inside_modal_dialog()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/tooltip"));

        ILocator dialogTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open tooltip dialog", Exact = true });
        await dialogTrigger.ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Tooltip dialog", Exact = true });
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        ILocator tooltipTrigger = dialog.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Show nested tooltip", Exact = true });
        await tooltipTrigger.HoverAsync();

        ILocator tooltip = page.GetByRole(AriaRole.Tooltip, new PageGetByRoleOptions { Name = "Nested tooltip", Exact = true });
        await Assertions.Expect(tooltip).ToBeVisibleAsync();
        await Assertions.Expect(dialog).ToBeVisibleAsync();
    }

    private static Task<bool> FocusIsWithinAsync(ILocator dialog)
    {
        return dialog.EvaluateAsync<bool>("element => element.contains(document.activeElement)");
    }

    private static async Task ClickJustOutsideActiveDialogAsync(IPage page, ILocator dialog)
    {
        var box = await dialog.BoundingBoxAsync();
        Assert.NotNull(box);
        float x = box.X > 40 ? box.X - 20 : box.X + box.Width + 20;
        float y = box.Y > 40 ? box.Y - 20 : box.Y + 20;
        await page.Mouse.ClickAsync(x, y);
    }

    private static async Task ExpectActiveElementAsync(IPage page, ILocator locator)
    {
        string? id = await locator.GetAttributeAsync("id");
        Assert.False(string.IsNullOrWhiteSpace(id));

        await page.WaitForFunctionAsync(
            "(expectedId) => document.activeElement?.id === expectedId",
            id);
    }
}
