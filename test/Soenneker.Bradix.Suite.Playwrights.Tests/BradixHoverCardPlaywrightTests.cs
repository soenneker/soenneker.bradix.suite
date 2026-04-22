using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixHoverCardPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixHoverCardPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async ValueTask Hover_card_demo_supports_nested_hover_card_inside_modal_dialog()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/hover-card"));

        ILocator dialogTrigger = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open hover card dialog", Exact = true });
        await dialogTrigger.ClickAsync();

        ILocator dialog = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Hover card dialog", Exact = true });
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        ILocator hoverCardTrigger = dialog.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Show nested hover card", Exact = true });
        await hoverCardTrigger.HoverAsync();

        ILocator hoverCard = page.GetByText("Hover card content inside dialog", new PageGetByTextOptions { Exact = true });
        await Assertions.Expect(hoverCard).ToBeVisibleAsync();
        await Assertions.Expect(dialog).ToBeVisibleAsync();
    }

[Test]
    public async ValueTask Hover_card_demo_hides_profile_details_after_pointer_leaves_trigger_and_content()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/hover-card"));

        ILocator trigger = page.GetByAltText("Radix UI");
        ILocator card = page.GetByText("@radix_ui", new PageGetByTextOptions { Exact = true });

        await trigger.HoverAsync();
        await Assertions.Expect(card).ToBeVisibleAsync();

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open hover card dialog", Exact = true }).HoverAsync();

        await Assertions.Expect(card).Not.ToBeVisibleAsync();
    }

[Test]
    public async ValueTask Hover_card_demo_shows_profile_details_on_hover()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/hover-card"));

        await page.GetByAltText("Radix UI").HoverAsync();

        await Assertions.Expect(page.GetByText("@radix_ui")).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByText("2,900")).ToBeVisibleAsync();
    }
}

