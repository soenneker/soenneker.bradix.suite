using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixTogglePlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixTogglePlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

    [Test]
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

    [Test]
    public async ValueTask Toggle_demo_controlled_buttons_and_disabled_state_stay_in_sync()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toggle"));

        ILocator controlledSection = page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "Controlled toggle" });
        ILocator controlledToggle = controlledSection.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Toggle notifications", Exact = true });
        ILocator state = controlledSection.Locator("#toggle-controlled-state");
        ILocator setOn = controlledSection.Locator("#toggle-controlled-on");
        ILocator setOff = controlledSection.Locator("#toggle-controlled-off");

        await Assertions.Expect(controlledToggle).ToHaveAttributeAsync("aria-pressed", "true");
        await Assertions.Expect(state).ToContainTextAsync("Pressed: true");

        await setOff.ClickAsync();

        await Assertions.Expect(controlledToggle).ToHaveAttributeAsync("aria-pressed", "false");
        await Assertions.Expect(state).ToContainTextAsync("Pressed: false");

        await controlledToggle.ClickAsync();

        await Assertions.Expect(controlledToggle).ToHaveAttributeAsync("aria-pressed", "true");
        await Assertions.Expect(state).ToContainTextAsync("Pressed: true");

        await setOff.ClickAsync();
        await setOn.ClickAsync();

        await Assertions.Expect(controlledToggle).ToHaveAttributeAsync("aria-pressed", "true");
        await Assertions.Expect(state).ToContainTextAsync("Pressed: true");

        ILocator disabledSection = page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "Disabled toggle" });
        ILocator disabledToggle = disabledSection.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Toggle disabled italic", Exact = true });

        await Assertions.Expect(disabledToggle).ToHaveAttributeAsync("disabled", "");
        await Assertions.Expect(disabledToggle).ToHaveAttributeAsync("aria-pressed", "false");

        await disabledToggle.ClickAsync(new LocatorClickOptions { Force = true });

        await Assertions.Expect(disabledToggle).ToHaveAttributeAsync("aria-pressed", "false");
    }
}

