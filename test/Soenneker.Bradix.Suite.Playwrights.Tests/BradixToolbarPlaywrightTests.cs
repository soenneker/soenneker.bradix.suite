using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixToolbarPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixToolbarPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async Task Toolbar_demo_roves_focus_across_groups_and_skips_disabled_items()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toolbar"));

        ILocator bold = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "B", Exact = true }).First;
        ILocator italic = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "I", Exact = true }).First;
        ILocator strikethrough = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "S", Exact = true }).First;
        ILocator left = page.GetByRole(AriaRole.Radio, new PageGetByRoleOptions { Name = "L", Exact = true }).First;
        ILocator center = page.GetByRole(AriaRole.Radio, new PageGetByRoleOptions { Name = "C", Exact = true }).First;
        ILocator right = page.GetByRole(AriaRole.Radio, new PageGetByRoleOptions { Name = "R", Exact = true }).First;
        ILocator edited = page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Edited 2 hours ago", Exact = true });
        ILocator share = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Share", Exact = true });
        ILocator print = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Print", Exact = true });

        await Assertions.Expect(print).ToBeDisabledAsync();

        await bold.FocusAsync();
        await Assertions.Expect(bold).ToBeFocusedAsync();

        await bold.PressAsync("ArrowRight");
        await Assertions.Expect(italic).ToBeFocusedAsync();

        await italic.PressAsync("ArrowRight");
        await Assertions.Expect(strikethrough).ToBeFocusedAsync();

        await strikethrough.PressAsync("ArrowRight");
        await Assertions.Expect(left).ToBeFocusedAsync();

        await left.PressAsync("ArrowRight");
        await Assertions.Expect(center).ToBeFocusedAsync();

        await center.PressAsync("ArrowRight");
        await Assertions.Expect(right).ToBeFocusedAsync();

        await right.PressAsync("ArrowRight");
        await Assertions.Expect(edited).ToBeFocusedAsync();

        await edited.PressAsync("ArrowRight");
        await Assertions.Expect(share).ToBeFocusedAsync();

        await share.PressAsync("ArrowRight");
        await Assertions.Expect(bold).ToBeFocusedAsync();

        await bold.PressAsync("ArrowLeft");
        await Assertions.Expect(share).ToBeFocusedAsync();
    }

[Test]
    public async Task Toolbar_demo_updates_pressed_states_for_toggle_groups()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/toolbar"));

        ILocator bold = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "B", Exact = true }).First;
        ILocator left = page.GetByRole(AriaRole.Radio, new PageGetByRoleOptions { Name = "L", Exact = true }).First;
        ILocator center = page.GetByRole(AriaRole.Radio, new PageGetByRoleOptions { Name = "C", Exact = true }).First;

        await bold.ClickAsync();
        await left.ClickAsync();

        await Assertions.Expect(bold).ToHaveAttributeAsync("aria-pressed", "true");
        await Assertions.Expect(left).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(center).ToHaveAttributeAsync("aria-checked", "false");
    }
}

