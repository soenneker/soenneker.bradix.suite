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
