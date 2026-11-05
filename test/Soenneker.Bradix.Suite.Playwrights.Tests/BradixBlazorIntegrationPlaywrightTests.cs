using System.Threading;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
public sealed class BradixBlazorIntegrationPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixBlazorIntegrationPlaywrightTests(BradixPlaywrightHost host) : base(host) { }

    [Test]
    public async ValueTask Custom_component_trigger_supports_keyboard_cancel_and_focus_return(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;
        await page.GotoAsync(BaseUrl.TrimEnd('/') + "/blazorintegration");
        ILocator trigger = page.Locator("#composed-trigger");
        await Assertions.Expect(trigger).ToBeVisibleAsync();
        await Assertions.Expect(page.Locator("button button")).ToHaveCountAsync(0);
        await page.Locator("#block-activation").CheckAsync();
        await trigger.ClickAsync();
        await Assertions.Expect(trigger).ToHaveAttributeAsync("aria-expanded", "false");
        await page.Locator("#block-activation").UncheckAsync();
        await trigger.FocusAsync();
        await trigger.PressAsync("Enter");
        await Assertions.Expect(page.GetByRole(AriaRole.Dialog)).ToBeVisibleAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Close composed dialog", Exact = true }).ClickAsync();
        await Assertions.Expect(page.GetByRole(AriaRole.Dialog)).ToHaveCountAsync(0);
        await Assertions.Expect(trigger).ToBeFocusedAsync();
    }

    [Test]
    public async ValueTask Select_adapter_validates_updates_rich_value_and_announces_save(CancellationToken cancellationToken)
    {
        await using BrowserSession session = await CreateSession(cancellationToken: cancellationToken);
        IPage page = session.Page;
        await page.GotoAsync(BaseUrl.TrimEnd('/') + "/blazorintegration");
        await Assertions.Expect(page.Locator("[aria-live=polite]")).ToHaveCountAsync(1);
        await page.GetByRole(AriaRole.Button, new() { Name = "Save city", Exact = true }).ClickAsync();
        await Assertions.Expect(page.Locator("#city-errors")).ToHaveTextAsync("Choose a city.");
        await Assertions.Expect(page.Locator("#city-trigger")).ToHaveAttributeAsync("aria-invalid", "true");
        await page.Locator("#city-trigger").ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "Paris", Exact = true }).ClickAsync();
        await Assertions.Expect(page.Locator("#city-trigger strong")).ToHaveTextAsync("Paris");
        await Assertions.Expect(page.Locator("#city-errors")).ToBeEmptyAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Save city", Exact = true }).ClickAsync();
        await Assertions.Expect(page.Locator("#saved-city")).ToHaveTextAsync("Saved Paris.");
        await Assertions.Expect(page.Locator("[aria-live=polite]")).ToHaveTextAsync("Saved Paris.");
    }
}
