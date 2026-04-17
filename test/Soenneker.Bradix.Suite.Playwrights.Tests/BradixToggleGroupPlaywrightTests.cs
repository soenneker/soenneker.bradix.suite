using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixToggleGroupPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixToggleGroupPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
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
    public async ValueTask Toggle_group_demo_rtl_arrow_keys_follow_visual_direction()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toggle-group"));

        ILocator section = page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "RTL single selection" });
        ILocator group = section.GetByRole(AriaRole.Radiogroup);
        ILocator right = section.GetByRole(AriaRole.Radio, new LocatorGetByRoleOptions { Name = "Right", Exact = true });
        ILocator center = section.GetByRole(AriaRole.Radio, new LocatorGetByRoleOptions { Name = "Center", Exact = true });
        ILocator left = section.GetByRole(AriaRole.Radio, new LocatorGetByRoleOptions { Name = "Left", Exact = true });

        await Assertions.Expect(group).ToHaveAttributeAsync("dir", "rtl");
        await Assertions.Expect(center).ToHaveAttributeAsync("aria-checked", "true");

        await center.FocusAsync();
        await page.Keyboard.PressAsync("ArrowLeft");

        await Assertions.Expect(left).ToBeFocusedAsync();
        await Assertions.Expect(center).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(left).ToHaveAttributeAsync("aria-checked", "false");

        await page.Keyboard.PressAsync("ArrowRight");

        await Assertions.Expect(center).ToBeFocusedAsync();
        await Assertions.Expect(center).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(right).ToHaveAttributeAsync("aria-checked", "false");
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
}

