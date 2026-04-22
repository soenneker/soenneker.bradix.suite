using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixSlotPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixSlotPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async Task Slot_demo_merges_child_attributes_into_target_element()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/slot"));

        ILocator button = page.Locator("#slot-button");

        await Assertions.Expect(button).ToHaveAttributeAsync("title", "child target title");
        await Assertions.Expect(button).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("slot-demo__child"));
        await Assertions.Expect(button).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("primitive__trigger"));
    }
}

