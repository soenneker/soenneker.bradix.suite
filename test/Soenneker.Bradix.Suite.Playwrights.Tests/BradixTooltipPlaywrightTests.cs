using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixTooltipPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixTooltipPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async ValueTask Tooltip_demo_supports_nested_tooltip_inside_modal_dialog()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/tooltip"));

        ILocator dialogTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open tooltip dialog", Exact = true });
        await dialogTrigger.ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Tooltip dialog", Exact = true });
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        ILocator tooltipTrigger = dialog.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Show nested tooltip", Exact = true });
        await tooltipTrigger.HoverAsync();

        ILocator tooltip = page.GetByRole(AriaRole.Tooltip, new PageGetByRoleOptions { Name = "Nested tooltip", Exact = true });
        await Assertions.Expect(tooltip).ToBeVisibleAsync();
        await Assertions.Expect(dialog).ToBeVisibleAsync();
    }

[Test]
    public async ValueTask Tooltip_demo_hides_content_after_pointer_leaves_trigger_and_content()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/tooltip"));

        ILocator trigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "+", Exact = true });
        ILocator tooltip = page.GetByRole(AriaRole.Tooltip, new PageGetByRoleOptions { Name = "Add to library", Exact = true });

        await trigger.HoverAsync();
        await Assertions.Expect(tooltip).ToBeVisibleAsync();

        await page.Locator("body").HoverAsync(new LocatorHoverOptions
        {
            Position = new Position
            {
                X = 5,
                Y = 5
            }
        });

        await Assertions.Expect(tooltip).Not.ToBeVisibleAsync();
    }

[Test]
    public async ValueTask Tooltip_demo_transfers_open_state_when_hovering_a_sibling_trigger()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/tooltip"));

        ILocator basicTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "+", Exact = true });
        ILocator basicTooltip = page.GetByRole(AriaRole.Tooltip, new PageGetByRoleOptions { Name = "Add to library", Exact = true });
        ILocator siblingTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Top", Exact = true });
        ILocator siblingTooltip = page.GetByRole(AriaRole.Tooltip, new PageGetByRoleOptions { Name = "Default placement", Exact = true });

        await basicTrigger.HoverAsync();
        await Assertions.Expect(basicTooltip).ToBeVisibleAsync();

        await siblingTrigger.HoverAsync();

        await Assertions.Expect(siblingTooltip).ToBeVisibleAsync();
        await Assertions.Expect(basicTooltip).Not.ToBeVisibleAsync();
    }

[Test]
    public async ValueTask Tooltip_demo_reveals_content_on_hover()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/tooltip"));

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "+", Exact = true }).HoverAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Tooltip, new PageGetByRoleOptions { Name = "Add to library", Exact = true })).ToBeVisibleAsync();
    }
}

