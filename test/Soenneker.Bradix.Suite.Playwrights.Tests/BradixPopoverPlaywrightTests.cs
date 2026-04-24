using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[NotInParallel]
[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixPopoverPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixPopoverPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
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

        await Assert.That(popupState).IsNotNull();
        await Assert.That(popupState!.role).IsEqualTo("listbox");
        await Assert.That(popupState.ariaLabel).IsEqualTo("Framework choices");
        await Assert.That(popupState.dataState).IsEqualTo("open");
        await Assert.That(popupState.selectedText).IsEqualTo("Astro");
        await Assertions.Expect(page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);
    }

[Test]
    public async ValueTask Popover_demo_closes_from_escape_after_opening_from_trigger()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/popover"));

        ILocator trigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Update dimensions", Exact = true });
        await trigger.ClickAsync();

        ILocator content = page.Locator(".popover-demo__content[data-state='open']").Filter(new LocatorFilterOptions { HasText = "Dimensions" });
        ILocator widthInput = page.Locator("#popover-width");

        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(content).ToBeVisibleAsync();
        await Assertions.Expect(widthInput).ToBeFocusedAsync();

        await widthInput.PressAsync("Escape");

        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "false");
        await Assertions.Expect(content).Not.ToBeVisibleAsync();
    }

[Test]
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

[Test]
    public async Task Popover_demo_positions_content_relative_to_the_trigger()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/popover"));

        ILocator trigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Update dimensions", Exact = true });
        await trigger.ClickAsync();

        ILocator content = page.GetByRole(AriaRole.Dialog).Filter(new LocatorFilterOptions { HasText = "Dimensions" }).First;
        await Assertions.Expect(content).ToBeVisibleAsync();
        await page.WaitForFunctionAsync(
            "element => { const box = element.getBoundingClientRect(); return box.y > 0 && box.width >= 240; }",
            await content.ElementHandleAsync());

        LocatorBoundingBoxResult? triggerBox = await trigger.BoundingBoxAsync();
        LocatorBoundingBoxResult? contentBox = await content.BoundingBoxAsync();

        await Assert.That(triggerBox).IsNotNull();
        await Assert.That(contentBox).IsNotNull();
        await Assert.That(contentBox!.Y > 0).IsTrue();
        await Assert.That(System.Math.Abs(contentBox.X - triggerBox!.X) <= 200).IsTrue();
        await Assert.That(contentBox.Width >= 240).IsTrue();
    }

[Test]
    public async ValueTask Popover_demo_closes_from_outside_click_after_opening_from_trigger()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/popover"));

        ILocator customRoleTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open custom listbox", Exact = true });
        await Assertions.Expect(customRoleTrigger).ToHaveAttributeAsync("aria-expanded", "true");
        await customRoleTrigger.ClickAsync();
        await Assertions.Expect(customRoleTrigger).ToHaveAttributeAsync("aria-expanded", "false");

        ILocator trigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Update dimensions", Exact = true });
        await trigger.ClickAsync();

        ILocator content = page.Locator(".popover-demo__content[data-state='open']").Filter(new LocatorFilterOptions { HasText = "Dimensions" });

        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(content).ToBeVisibleAsync();

        ILocator main = page.Locator(".docs-shell__main");
        var mainBox = await main.BoundingBoxAsync();
        await Assert.That(mainBox).IsNotNull();
        await page.Mouse.ClickAsync(mainBox!.X + mainBox.Width - 10, mainBox.Y + mainBox.Height - 10);

        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "false");
        await Assertions.Expect(content).Not.ToBeVisibleAsync();
    }

[Test]
    public async ValueTask Popover_demo_outside_control_button_opens_controlled_popover_while_another_popover_is_open()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/popover"));

        ILocator customRoleTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open custom listbox", Exact = true });
        await Assertions.Expect(customRoleTrigger).ToHaveAttributeAsync("aria-expanded", "true");

        ILocator toggle = page.Locator("#popover-controlled-toggle");
        ILocator state = page.Locator("#popover-controlled-state");
        ILocator controlledTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit controlled dimensions", Exact = true });
        ILocator controlledContent = page.Locator(".popover-demo__content[data-state='open']").Filter(new LocatorFilterOptions { HasText = "Controlled dimensions" });

        await toggle.ClickAsync();

        await Assertions.Expect(state).ToContainTextAsync("Open: true");
        await Assertions.Expect(controlledTrigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(controlledContent).ToBeVisibleAsync();
    }

[Test]
    public async ValueTask Popover_demo_controlled_open_state_stays_in_sync_with_outside_dismissal()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/popover"));

        ILocator toggle = page.Locator("#popover-controlled-toggle");
        ILocator state = page.Locator("#popover-controlled-state");
        ILocator controlledTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit controlled dimensions", Exact = true });
        ILocator controlledContent = page.Locator(".popover-demo__content[data-state='open']").Filter(new LocatorFilterOptions { HasText = "Controlled dimensions" });

        await toggle.ClickAsync();

        await Assertions.Expect(state).ToContainTextAsync("Open: true");
        await Assertions.Expect(controlledTrigger).ToHaveAttributeAsync("aria-expanded", "true");
        await Assertions.Expect(controlledContent).ToBeVisibleAsync();

        ILocator main = page.Locator(".docs-shell__main");
        var mainBox = await main.BoundingBoxAsync();
        await Assert.That(mainBox).IsNotNull();
        await page.Mouse.ClickAsync(mainBox!.X + mainBox.Width - 10, mainBox.Y + mainBox.Height - 10);

        await Assertions.Expect(state).ToContainTextAsync("Open: false");
        await Assertions.Expect(controlledTrigger).ToHaveAttributeAsync("aria-expanded", "false");
        await Assertions.Expect(controlledContent).Not.ToBeVisibleAsync();
    }

    private sealed class PopoverRoleProbe
    {
        public string? role { get; set; }
        public string? ariaLabel { get; set; }
        public string? dataState { get; set; }
        public string? selectedText { get; set; }
    }
}

