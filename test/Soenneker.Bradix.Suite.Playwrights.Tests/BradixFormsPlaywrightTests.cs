using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Soenneker.Playwrights.Tests.Unit;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixFormsPlaywrightTests : PlaywrightUnitTest
{
    public BradixFormsPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

    [Fact]
    public async ValueTask Checkbox_demo_is_checked_by_default_and_can_toggle()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/checkbox"));

        ILocator checkbox = page.GetByRole(AriaRole.Checkbox);
        await Assertions.Expect(checkbox).ToHaveAttributeAsync("aria-checked", "true");

        await checkbox.ClickAsync();

        await Assertions.Expect(checkbox).ToHaveAttributeAsync("aria-checked", "false");
    }

    [Fact]
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

    [Fact]
    public async ValueTask One_time_password_demo_distributes_typed_digits_across_slots()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/one-time-password-field"));

        ILocator slots = page.Locator(".otp-slot");
        await slots.First.ClickAsync();
        await page.Keyboard.TypeAsync("123456");

        await Assertions.Expect(slots.Nth(0)).ToHaveValueAsync("1");
        await Assertions.Expect(slots.Nth(1)).ToHaveValueAsync("2");
        await Assertions.Expect(slots.Nth(2)).ToHaveValueAsync("3");
        await Assertions.Expect(slots.Nth(3)).ToHaveValueAsync("4");
        await Assertions.Expect(slots.Nth(4)).ToHaveValueAsync("5");
        await Assertions.Expect(slots.Nth(5)).ToHaveValueAsync("6");
    }

    [Fact]
    public async ValueTask Progress_demo_exposes_current_value()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/progress"));

        await Assertions.Expect(page.GetByRole(AriaRole.Progressbar)).ToHaveAttributeAsync("aria-valuenow", "66");
    }

    [Fact]
    public async ValueTask Radio_group_demo_changes_selected_density()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/radio-group"));

        ILocator defaultRadio = page.GetByRole(AriaRole.Radio, new PageGetByRoleOptions { Name = "Default", Exact = true });
        ILocator compactRadio = page.GetByRole(AriaRole.Radio, new PageGetByRoleOptions { Name = "Compact", Exact = true });

        await Assertions.Expect(defaultRadio).ToHaveAttributeAsync("aria-checked", "true");
        await compactRadio.ClickAsync();

        await Assertions.Expect(compactRadio).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(defaultRadio).ToHaveAttributeAsync("aria-checked", "false");
    }

    [Fact]
    public async ValueTask Radio_group_demo_disabled_item_does_not_change_selection()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/radio-group"));

        ILocator disabledSection = page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "Disabled" });
        ILocator starterRadio = disabledSection.GetByRole(AriaRole.Radio).Nth(0);
        ILocator proRadio = disabledSection.GetByRole(AriaRole.Radio).Nth(1);
        ILocator enterpriseRadio = disabledSection.GetByRole(AriaRole.Radio).Nth(2);

        await Assertions.Expect(starterRadio).ToHaveAttributeAsync("disabled", "");
        await Assertions.Expect(proRadio).ToHaveAttributeAsync("aria-checked", "true");

        await starterRadio.ClickAsync(new LocatorClickOptions { Force = true });

        await Assertions.Expect(starterRadio).ToHaveAttributeAsync("aria-checked", "false");
        await Assertions.Expect(proRadio).ToHaveAttributeAsync("aria-checked", "true");

        await enterpriseRadio.ClickAsync();

        await Assertions.Expect(enterpriseRadio).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(proRadio).ToHaveAttributeAsync("aria-checked", "false");
    }

    [Fact]
    public async ValueTask Radio_group_demo_controlled_buttons_and_clicks_stay_in_sync()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/radio-group"));

        ILocator controlledSection = page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "Controlled" });
        ILocator weeklyRadio = controlledSection.GetByRole(AriaRole.Radio).Nth(0);
        ILocator monthlyRadio = controlledSection.GetByRole(AriaRole.Radio).Nth(1);

        await Assertions.Expect(weeklyRadio).ToHaveAttributeAsync("aria-checked", "true");

        await controlledSection.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Select monthly", Exact = true }).ClickAsync();

        await Assertions.Expect(monthlyRadio).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(weeklyRadio).ToHaveAttributeAsync("aria-checked", "false");

        await weeklyRadio.ClickAsync();

        await Assertions.Expect(weeklyRadio).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(monthlyRadio).ToHaveAttributeAsync("aria-checked", "false");
    }

    [Fact]
    public async ValueTask Slider_demo_updates_value_from_keyboard_input()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/slider"));

        ILocator slider = page.GetByRole(AriaRole.Slider, new PageGetByRoleOptions { Name = "Volume", Exact = true });
        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "50");

        await slider.FocusAsync();
        await page.Keyboard.PressAsync("ArrowRight");

        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "51");
    }

    [Fact]
    public async ValueTask Switch_demo_toggles_checked_state()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/switch"));

        ILocator toggle = page.GetByRole(AriaRole.Switch, new PageGetByRoleOptions { Name = "Airplane mode", Exact = true });
        await Assertions.Expect(toggle).ToHaveAttributeAsync("aria-checked", "false");

        await toggle.ClickAsync();

        await Assertions.Expect(toggle).ToHaveAttributeAsync("aria-checked", "true");
    }

    [Fact]
    public async ValueTask Toggle_demo_updates_pressed_state()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toggle"));

        ILocator toggle = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Toggle italic", Exact = true });
        await Assertions.Expect(toggle).ToHaveAttributeAsync("aria-pressed", "false");

        await toggle.ClickAsync();

        await Assertions.Expect(toggle).ToHaveAttributeAsync("aria-pressed", "true");
    }

    [Fact]
    public async ValueTask Toggle_group_demo_enforces_single_selection()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toggle-group"));

        ILocator left = page.GetByRole(AriaRole.Radio, new PageGetByRoleOptions { Name = "L", Exact = true });
        ILocator center = page.GetByRole(AriaRole.Radio, new PageGetByRoleOptions { Name = "C", Exact = true });

        await Assertions.Expect(center).ToHaveAttributeAsync("aria-checked", "true");

        await left.ClickAsync();

        await Assertions.Expect(left).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(center).ToHaveAttributeAsync("aria-checked", "false");
    }

    [Fact]
    public async ValueTask Toggle_group_demo_preserves_multiple_selection_state()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toggle-group"));

        ILocator section = page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "Multiple selection" });
        ILocator bold = section.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Bold", Exact = true });
        ILocator italic = section.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Italic", Exact = true });

        await Assertions.Expect(bold).ToHaveAttributeAsync("aria-pressed", "true");
        await Assertions.Expect(italic).ToHaveAttributeAsync("aria-pressed", "false");

        await italic.ClickAsync();

        await Assertions.Expect(bold).ToHaveAttributeAsync("aria-pressed", "true");
        await Assertions.Expect(italic).ToHaveAttributeAsync("aria-pressed", "true");
        await Assertions.Expect(bold).ToHaveAttributeAsync("data-state", "on");
        await Assertions.Expect(italic).ToHaveAttributeAsync("data-state", "on");
    }

    [Fact]
    public async ValueTask Toggle_group_demo_vertical_navigation_skips_disabled_item()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toggle-group"));

        ILocator section = page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "Vertical with disabled item" });
        ILocator top = section.GetByRole(AriaRole.Radio, new LocatorGetByRoleOptions { Name = "Top", Exact = true });
        ILocator middle = section.GetByRole(AriaRole.Radio, new LocatorGetByRoleOptions { Name = "Middle", Exact = true });
        ILocator bottom = section.GetByRole(AriaRole.Radio, new LocatorGetByRoleOptions { Name = "Bottom", Exact = true });

        await Assertions.Expect(top).ToHaveAttributeAsync("tabindex", "0");
        await Assertions.Expect(middle).ToHaveAttributeAsync("disabled", "");

        await top.FocusAsync();
        await page.Keyboard.PressAsync("ArrowDown");

        await Assertions.Expect(bottom).ToBeFocusedAsync();
        await Assertions.Expect(bottom).ToHaveAttributeAsync("tabindex", "0");
        await Assertions.Expect(top).ToHaveAttributeAsync("tabindex", "-1");
    }

    [Fact]
    public async ValueTask Toggle_group_demo_disabled_group_does_not_change_pressed_state()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toggle-group"));

        ILocator section = page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "Disabled group" });
        ILocator left = section.GetByRole(AriaRole.Radio, new LocatorGetByRoleOptions { Name = "Left", Exact = true });
        ILocator right = section.GetByRole(AriaRole.Radio, new LocatorGetByRoleOptions { Name = "Right", Exact = true });

        await Assertions.Expect(left).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(right).ToHaveAttributeAsync("aria-checked", "false");
        await Assertions.Expect(right).ToHaveAttributeAsync("disabled", "");

        await right.ClickAsync(new LocatorClickOptions { Force = true });

        await Assertions.Expect(left).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(right).ToHaveAttributeAsync("aria-checked", "false");
    }
}
