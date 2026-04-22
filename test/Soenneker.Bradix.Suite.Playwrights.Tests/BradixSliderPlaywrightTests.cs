using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixSliderPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixSliderPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async ValueTask Slider_demo_rtl_keyboard_direction_matches_radix_behavior()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/slider"));

        ILocator rtlSection = page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "RTL" });
        ILocator slider = rtlSection.GetByRole(AriaRole.Slider, new LocatorGetByRoleOptions { Name = "RTL volume", Exact = true });

        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "75");

        await slider.FocusAsync();
        await page.Keyboard.PressAsync("ArrowRight");

        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "70");
        await Assertions.Expect(rtlSection).ToContainTextAsync("Value: 70");

        await page.Keyboard.PressAsync("ArrowLeft");

        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "75");
        await Assertions.Expect(rtlSection).ToContainTextAsync("Value: 75");
    }

[Test]
    public async ValueTask Slider_demo_minimum_spacing_prevents_thumbs_from_crossing()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/slider"));

        ILocator spacingSection = page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "Minimum spacing" });
        ILocator minimumThumb = spacingSection.GetByRole(AriaRole.Slider, new LocatorGetByRoleOptions { Name = "Minimum", Exact = true });
        ILocator maximumThumb = spacingSection.GetByRole(AriaRole.Slider, new LocatorGetByRoleOptions { Name = "Maximum", Exact = true });

        await Assertions.Expect(minimumThumb).ToHaveAttributeAsync("aria-valuenow", "20");
        await Assertions.Expect(maximumThumb).ToHaveAttributeAsync("aria-valuenow", "60");

        await maximumThumb.FocusAsync();
        await page.Keyboard.PressAsync("ArrowLeft");
        await page.Keyboard.PressAsync("ArrowLeft");

        await Assertions.Expect(maximumThumb).ToHaveAttributeAsync("aria-valuenow", "40");
        await Assertions.Expect(spacingSection).ToContainTextAsync("Values: 20, 40");

        await page.Keyboard.PressAsync("ArrowLeft");

        await Assertions.Expect(maximumThumb).ToHaveAttributeAsync("aria-valuenow", "40");
        await Assertions.Expect(minimumThumb).ToHaveAttributeAsync("aria-valuenow", "20");
        await Assertions.Expect(spacingSection).ToContainTextAsync("Values: 20, 40");
    }

[Test]
    public async ValueTask Slider_demo_home_and_end_keys_jump_to_minimum_and_maximum()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/slider"));

        ILocator slider = page.GetByRole(AriaRole.Slider, new PageGetByRoleOptions { Name = "Volume", Exact = true });
        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "50");

        await slider.FocusAsync();
        await page.Keyboard.PressAsync("Home");

        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "0");

        await page.Keyboard.PressAsync("End");

        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "100");
    }

[Test]
    public async ValueTask Slider_demo_controlled_buttons_and_keyboard_updates_stay_in_sync()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/slider"));

        ILocator controlledSection = page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "Controlled" });
        ILocator slider = controlledSection.GetByRole(AriaRole.Slider, new LocatorGetByRoleOptions { Name = "Controlled volume", Exact = true });

        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "40");

        await controlledSection.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Set 80", Exact = true }).ClickAsync();

        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "80");
        await Assertions.Expect(controlledSection).ToContainTextAsync("Value: 80");

        await slider.FocusAsync();
        await page.Keyboard.PressAsync("ArrowLeft");

        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "70");
        await Assertions.Expect(controlledSection).ToContainTextAsync("Value: 70");
    }

[Test]
    public async ValueTask Slider_demo_horizontal_track_click_updates_value()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/slider"));

        ILocator defaultSection = page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "Default" });
        ILocator slider = defaultSection.GetByRole(AriaRole.Slider, new LocatorGetByRoleOptions { Name = "Volume", Exact = true });
        ILocator track = defaultSection.Locator(".slider-track").First;

        await Assertions.Expect(defaultSection.Locator("[data-js-ready='true']")).ToBeVisibleAsync();
        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "50");

        var trackBox = await track.BoundingBoxAsync();
        Assert.NotNull(trackBox);

        await track.ClickAsync(new LocatorClickOptions
        {
            Position = new Position
            {
                X = (float)(trackBox!.Width * 0.8),
                Y = (float)(trackBox.Height / 2)
            }
        });

        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "80");
    }

[Test]
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

[Test]
    public async ValueTask Slider_demo_vertical_track_click_and_keyboard_update_value()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/slider"));

        ILocator verticalSection = page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "Vertical" });
        ILocator slider = verticalSection.GetByRole(AriaRole.Slider, new LocatorGetByRoleOptions { Name = "Vertical volume", Exact = true });

        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "35");
        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-orientation", "vertical");

        ILocator verticalRoot = verticalSection.Locator(".slider-root--vertical");
        var rootBox = await verticalRoot.BoundingBoxAsync();
        Assert.NotNull(rootBox);

        await verticalRoot.ClickAsync(new LocatorClickOptions
        {
            Position = new Position
            {
                X = (float)(rootBox!.Width / 2),
                Y = (float)(rootBox.Height * 0.2)
            }
        });

        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "80");
        await Assertions.Expect(verticalSection).ToContainTextAsync("Value: 80");

        await slider.FocusAsync();
        await page.Keyboard.PressAsync("ArrowDown");

        await Assertions.Expect(slider).ToHaveAttributeAsync("aria-valuenow", "75");
        await Assertions.Expect(verticalSection).ToContainTextAsync("Value: 75");
    }
}

