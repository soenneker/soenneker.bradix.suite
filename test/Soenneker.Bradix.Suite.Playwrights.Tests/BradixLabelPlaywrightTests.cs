using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixLabelPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixLabelPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

[Fact]
    public async Task Label_demo_focuses_input_when_label_is_clicked()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/label"));

        ILocator input = page.Locator("#firstName");
        await page.GetByText("First name", new PageGetByTextOptions { Exact = true }).ClickAsync();

        await Assertions.Expect(input).ToBeFocusedAsync();
    }

[Fact]
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
}

