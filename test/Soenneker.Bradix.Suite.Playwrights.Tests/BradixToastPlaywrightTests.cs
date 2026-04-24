using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixToastPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixToastPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

    [Test]
    public async ValueTask Toast_demo_shows_scheduled_notification()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;
        var consoleMessages = new List<string>();
        var pageErrors = new List<string>();

        page.Console += (_, message) =>
        {
            if (message.Type is "error" or "warning")
                consoleMessages.Add($"{message.Type}: {message.Text}");
        };
        page.PageError += (_, exception) => pageErrors.Add(exception);

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toast"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add to calendar", Exact = true }).ClickAsync();

        await Assertions.Expect(page.GetByText("Scheduled: Catch up", new PageGetByTextOptions { Exact = true })).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Undo", Exact = true })).ToBeVisibleAsync();
        await Assertions.Expect(page.Locator("ol.toast-demo__viewport")).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByRole(AriaRole.Region, new PageGetByRoleOptions { Name = "Notifications (F8)", Exact = true }))
            .ToHaveAttributeAsync("tabindex", "-1");
        await Assert.That(pageErrors).IsEmpty();
        await Assert.That(consoleMessages).IsEmpty();
    }

    [Test]
    public async ValueTask Toast_demo_action_dismisses_the_current_notification()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toast"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add to calendar", Exact = true }).ClickAsync();

        ILocator toast = page.Locator(".toast-demo__root[data-state='open']").Filter(new LocatorFilterOptions { HasText = "Scheduled: Catch up" });
        await Assertions.Expect(toast).ToBeVisibleAsync();

        ILocator action = toast.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Undo", Exact = true });
        await Assertions.Expect(action).ToBeVisibleAsync();
        await Assertions.Expect(action).Not.ToHaveAttributeAsync("aria-label", "Goto schedule to undo");
        await action.ClickAsync();

        await Assertions.Expect(toast).Not.ToBeVisibleAsync();
    }

    [Test]
    public async ValueTask Toast_demo_hotkey_focuses_viewport_and_escape_closes_focused_toast()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toast"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add to calendar", Exact = true }).ClickAsync();

        ILocator viewport = page.Locator("ol.toast-demo__viewport");
        ILocator toast = page.Locator(".toast-demo__root[data-state='open']").Filter(new LocatorFilterOptions { HasText = "Scheduled: Catch up" });
        await Assertions.Expect(toast).ToBeVisibleAsync();

        await page.EvaluateAsync("document.dispatchEvent(new KeyboardEvent('keydown', { code: 'F8', key: 'F8', bubbles: true }))");
        await Assertions.Expect(viewport).ToBeFocusedAsync();

        await page.Keyboard.PressAsync("Tab");
        await Assertions.Expect(toast).ToBeFocusedAsync();

        await page.Keyboard.PressAsync("Escape");
        await Assertions.Expect(toast).Not.ToBeVisibleAsync();
        await Assertions.Expect(viewport).ToBeFocusedAsync();
    }

    [Test]
    public async ValueTask Toast_demo_swipe_right_dismisses_toast()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toast"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add to calendar", Exact = true }).ClickAsync();

        ILocator toast = page.Locator(".toast-demo__root[data-state='open']").Filter(new LocatorFilterOptions { HasText = "Scheduled: Catch up" });
        await Assertions.Expect(toast).ToBeVisibleAsync();

        await toast.DispatchEventAsync("pointerdown", new
        {
            button = 0,
            clientX = 10,
            clientY = 10,
            pointerId = 21,
            pointerType = "mouse"
        });
        await toast.DispatchEventAsync("pointermove", new
        {
            button = 0,
            clientX = 90,
            clientY = 10,
            pointerId = 21,
            pointerType = "mouse"
        });
        await toast.DispatchEventAsync("pointerup", new
        {
            button = 0,
            clientX = 90,
            clientY = 10,
            pointerId = 21,
            pointerType = "mouse"
        });

        await Assertions.Expect(toast).Not.ToBeVisibleAsync();
    }
}

