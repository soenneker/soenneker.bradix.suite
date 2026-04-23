using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixAccordionPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixAccordionPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
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

[Test]
    public async ValueTask Accordion_demo_unmounts_closed_content_by_default_and_keeps_force_mounted_content_in_dom()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/accordion"));

        ILocator singleDemo = page.GetByRole(AriaRole.Region, new PageGetByRoleOptions { Name = "Single accordion demo", Exact = true });
        ILocator accessibleTrigger = singleDemo.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Is it accessible?", Exact = true });
        ILocator unstyledTrigger = singleDemo.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Is it unstyled?", Exact = true });
        string? accessiblePanelId = await accessibleTrigger.GetAttributeAsync("aria-controls");
        string? unstyledPanelId = await unstyledTrigger.GetAttributeAsync("aria-controls");

        Xunit.Assert.False(string.IsNullOrWhiteSpace(accessiblePanelId));
        Xunit.Assert.False(string.IsNullOrWhiteSpace(unstyledPanelId));

        ILocator accessiblePanel = page.Locator($"#{accessiblePanelId}");
        ILocator unstyledPanel = page.Locator($"#{unstyledPanelId}");

        await Assertions.Expect(accessiblePanel).ToBeVisibleAsync();
        await Assertions.Expect(unstyledPanel).ToHaveCountAsync(0);

        await accessibleTrigger.ClickAsync();

        await Assertions.Expect(accessibleTrigger).ToHaveAttributeAsync("aria-expanded", "false");
        await Assertions.Expect(accessiblePanel).ToHaveCountAsync(0);
        await Assertions.Expect(unstyledPanel).ToHaveCountAsync(0);

        ILocator forceMountDemo = page.GetByRole(AriaRole.Region, new PageGetByRoleOptions { Name = "Force mounted accordion demo", Exact = true });
        ILocator forceMountContent = forceMountDemo.Locator(".accordion-demo__force-mount-content");

        await Assertions.Expect(forceMountContent).ToHaveAttributeAsync("data-state", "closed");
        await Assertions.Expect(forceMountContent).Not.ToHaveAttributeAsync("hidden", ".+");
        await Assertions.Expect(forceMountContent).ToContainTextAsync("Force mounted accordion details remain in the DOM while closed.");
    }

[Test]
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

[Test]
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
}

