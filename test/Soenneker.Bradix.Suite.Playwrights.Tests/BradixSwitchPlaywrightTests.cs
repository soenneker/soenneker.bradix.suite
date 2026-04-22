using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixSwitchPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixSwitchPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async ValueTask Switch_demo_controlled_buttons_and_clicks_stay_in_sync()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/switch"));

        ILocator controlledSection = page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "Controlled" });
        ILocator toggle = controlledSection.Locator("#sync-mode");

        await Assertions.Expect(toggle).ToHaveAttributeAsync("aria-checked", "true");

        await controlledSection.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Disable sync", Exact = true }).ClickAsync();

        await Assertions.Expect(toggle).ToHaveAttributeAsync("aria-checked", "false");
        await Assertions.Expect(controlledSection).ToContainTextAsync("State: unchecked");

        await toggle.ClickAsync();

        await Assertions.Expect(toggle).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(controlledSection).ToContainTextAsync("State: checked");
    }

[Test]
    public async ValueTask Switch_demo_form_reset_restores_default_checked_state()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/switch"));

        ILocator marketing = page.Locator("#marketing-switch");
        ILocator product = page.Locator("#product-switch");

        await Assertions.Expect(marketing).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(product).ToHaveAttributeAsync("aria-checked", "false");

        await marketing.ClickAsync();
        await product.ClickAsync();

        await Assertions.Expect(marketing).ToHaveAttributeAsync("aria-checked", "false");
        await Assertions.Expect(product).ToHaveAttributeAsync("aria-checked", "true");

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Reset form", Exact = true }).ClickAsync();

        await Assertions.Expect(marketing).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(product).ToHaveAttributeAsync("aria-checked", "false");
    }

[Test]
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
}

