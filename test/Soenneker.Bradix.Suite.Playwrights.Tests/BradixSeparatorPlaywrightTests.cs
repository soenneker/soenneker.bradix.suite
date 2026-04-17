using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixSeparatorPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixSeparatorPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

[Fact]
    public async Task Separator_demo_exposes_semantic_horizontal_and_decorative_vertical_metadata()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/separator"));

        ILocator semantic = page.Locator("[role='separator'][data-orientation='horizontal']").First;
        await Assertions.Expect(semantic).Not.ToHaveAttributeAsync("aria-orientation", new System.Text.RegularExpressions.Regex(".+"));

        ILocator decorativeVerticals = page.Locator("[role='none'][data-orientation='vertical']");
        await Assertions.Expect(decorativeVerticals).ToHaveCountAsync(2);
        await Assertions.Expect(decorativeVerticals.First).Not.ToHaveAttributeAsync("aria-orientation", new System.Text.RegularExpressions.Regex(".+"));
    }
}

