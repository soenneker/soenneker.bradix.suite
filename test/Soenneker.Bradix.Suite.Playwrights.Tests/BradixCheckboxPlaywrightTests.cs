using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Session;
using Xunit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[Collection("Collection")]
public sealed class BradixCheckboxPlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixCheckboxPlaywrightTests(BradixPlaywrightFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

[Fact]
    public async ValueTask Checkbox_demo_form_reset_restores_default_checked_and_indeterminate_states()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/checkbox"));

        ILocator emailUpdates = page.Locator("#email-updates-checkbox");
        ILocator projectPermissions = page.Locator("#project-permissions-checkbox");
        ILocator emailUpdatesInput = page.Locator("#email-updates-checkbox + input[type='checkbox']");
        ILocator projectPermissionsInput = page.Locator("#project-permissions-checkbox + input[type='checkbox']");

        await Assertions.Expect(emailUpdatesInput).ToBeCheckedAsync();
        await page.WaitForFunctionAsync(
            "() => {" +
            "const input = document.querySelector('#project-permissions-checkbox + input[type=\"checkbox\"]');" +
            "return input instanceof HTMLInputElement && input.indeterminate === true;" +
            "}");

        await Assertions.Expect(emailUpdates).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(projectPermissions).ToHaveAttributeAsync("aria-checked", "mixed");

        await emailUpdates.ClickAsync();
        await projectPermissions.ClickAsync();

        await Assertions.Expect(emailUpdates).ToHaveAttributeAsync("aria-checked", "false");
        await Assertions.Expect(projectPermissions).ToHaveAttributeAsync("aria-checked", "true");

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Reset form", Exact = true }).ClickAsync();

        await Assertions.Expect(emailUpdates).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(projectPermissions).ToHaveAttributeAsync("aria-checked", "mixed");
        await Assertions.Expect(projectPermissions).ToHaveAttributeAsync("data-state", "indeterminate");
    }

[Fact]
    public async ValueTask Checkbox_demo_indeterminate_click_sets_checked_state()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/checkbox"));

        ILocator checkbox = page.Locator("#ownership-checkbox");

        await Assertions.Expect(checkbox).ToHaveAttributeAsync("aria-checked", "mixed");
        await Assertions.Expect(checkbox).ToHaveAttributeAsync("data-state", "indeterminate");

        await checkbox.ClickAsync();

        await Assertions.Expect(checkbox).ToHaveAttributeAsync("aria-checked", "true");
        await Assertions.Expect(checkbox).ToHaveAttributeAsync("data-state", "checked");
        await Assertions.Expect(page.Locator("section.card").Filter(new LocatorFilterOptions { HasText = "Indeterminate" })).ToContainTextAsync("State: checked");
    }

[Fact]
    public async ValueTask Checkbox_demo_is_checked_by_default_and_can_toggle()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;

        await page.OpenDemoPage(BaseUrl, DemoPageSpecs.Get("/checkbox"));

        ILocator checkbox = page.Locator("#terms-checkbox");
        await Assertions.Expect(checkbox).ToHaveAttributeAsync("aria-checked", "true");

        await checkbox.ClickAsync();

        await Assertions.Expect(checkbox).ToHaveAttributeAsync("aria-checked", "false");
    }
}

