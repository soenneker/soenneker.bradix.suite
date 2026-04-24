using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixLabelPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixLabelPlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

[Test]
    public async Task Label_demo_focuses_input_when_label_is_clicked()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/label"));

        ILocator input = page.Locator("#firstName");
        await page.GetByText("First name", new PageGetByTextOptions { Exact = true }).ClickAsync();

        await Assertions.Expect(input).ToBeFocusedAsync();
    }

[Test]
    public async Task Label_demo_toggles_associated_checkbox_when_label_is_clicked()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/label"));

        ILocator checkbox = page.Locator("#newsletter");
        await Assertions.Expect(checkbox).Not.ToBeCheckedAsync();

        await page.GetByText("Email me product updates", new PageGetByTextOptions { Exact = true }).ClickAsync();

        await Assertions.Expect(checkbox).ToBeCheckedAsync();
    }

    [Test]
    public async Task Label_demo_prevents_double_click_text_selection_default_on_label_text()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/label"));

        ILocator label = page.GetByText("First name", new PageGetByTextOptions { Exact = true });
        bool dispatchResult = await label.EvaluateAsync<bool>(
            "element => element.dispatchEvent(new MouseEvent('mousedown', { bubbles: true, cancelable: true, detail: 2 }))");

        await Assert.That(dispatchResult).IsFalse();
    }
}
