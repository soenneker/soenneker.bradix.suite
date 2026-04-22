using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixCollapsiblePlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixCollapsiblePlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async ValueTask Collapsible_demo_disabled_trigger_stays_open_and_force_mounted_content_remains_hidden_in_dom()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/collapsible"));

        ILocator disabledCard = page.Locator(".website-demo-card").Filter(new LocatorFilterOptions { HasText = "Production deployment" }).First;
        ILocator disabledTrigger = disabledCard.GetByRole(AriaRole.Button);
        ILocator disabledContent = disabledCard.GetByText("Deployment notes stay visible while the panel is locked.", new LocatorGetByTextOptions { Exact = true });

        await Assertions.Expect(disabledTrigger).ToBeDisabledAsync();
        await Assertions.Expect(disabledTrigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(disabledContent).ToBeVisibleAsync();

        await disabledTrigger.ClickAsync(new LocatorClickOptions { Force = true });

        await Assertions.Expect(disabledTrigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(disabledContent).ToBeVisibleAsync();

        ILocator forceMountCard = page.Locator(".website-demo-card").Filter(new LocatorFilterOptions { HasText = "Animation-ready content" }).First;
        ILocator forceMountContent = forceMountCard.Locator(".collapsible-demo__force-mount-content");
        await Assertions.Expect(forceMountContent).ToHaveAttributeAsync("data-state", "closed");
        await Assertions.Expect(forceMountContent).Not.ToHaveAttributeAsync("hidden", ".+");
        await Assertions.Expect(forceMountContent).ToContainTextAsync("Force mounted details remain in the DOM while closed.");
    }

[Test]
    public async ValueTask Collapsible_demo_reveals_additional_repositories_when_opened()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/collapsible"));

        ILocator demoCard = page.Locator(".website-demo-card").Filter(new LocatorFilterOptions { HasText = "@peduarte starred 3 repositories" }).First;
        ILocator trigger = demoCard.GetByRole(AriaRole.Button);
        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "false");

        await trigger.ClickAsync();

        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(demoCard.Locator(".collapsible-demo__root")).ToContainTextAsync("@radix-ui/colors");
        await Assertions.Expect(demoCard.Locator(".collapsible-demo__root")).ToContainTextAsync("@radix-ui/themes");
    }
}

