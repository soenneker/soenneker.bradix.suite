using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixRadioGroupPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixRadioGroupPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
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

[Test]
    public async ValueTask Radio_group_demo_home_and_end_keys_move_selection_to_enabled_edges()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/radio-group"));

        ILocator densityGroup = page.GetByRole(AriaRole.Radiogroup, new PageGetByRoleOptions { Name = "View density", Exact = true });
        ILocator defaultRadio = densityGroup.GetByRole(AriaRole.Radio).Nth(0);
        ILocator compactRadio = densityGroup.GetByRole(AriaRole.Radio).Nth(2);

        await compactRadio.FocusAsync();
        await page.Keyboard.PressAsync("Home");

        await Assertions.Expect(defaultRadio).ToBeFocusedAsync();
        await Assertions.Expect(defaultRadio).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(compactRadio).ToHaveAttributeAsync("aria-checked", "false");

        await page.Keyboard.PressAsync("End");

        await Assertions.Expect(compactRadio).ToBeFocusedAsync();
        await Assertions.Expect(defaultRadio).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(compactRadio).ToHaveAttributeAsync("aria-checked", "false");

        ILocator disabledSection = page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "Disabled" });
        ILocator starterRadio = disabledSection.GetByRole(AriaRole.Radio).Nth(0);
        ILocator proRadio = disabledSection.GetByRole(AriaRole.Radio).Nth(1);
        ILocator enterpriseRadio = disabledSection.GetByRole(AriaRole.Radio).Nth(2);

        await proRadio.FocusAsync();
        await page.Keyboard.PressAsync("Home");

        await Assertions.Expect(proRadio).ToBeFocusedAsync();
        await Assertions.Expect(proRadio).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(starterRadio).ToHaveAttributeAsync("aria-checked", "false");

        await page.Keyboard.PressAsync("End");

        await Assertions.Expect(enterpriseRadio).ToBeFocusedAsync();
        await Assertions.Expect(proRadio).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(enterpriseRadio).ToHaveAttributeAsync("aria-checked", "false");
    }

[Test]
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

[Test]
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
}

