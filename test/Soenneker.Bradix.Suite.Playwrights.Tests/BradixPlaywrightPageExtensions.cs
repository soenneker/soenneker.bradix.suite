using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Extensions.ValueTask;
using Soenneker.Playwrights.Extensions.TestPages;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

internal static class BradixPlaywrightPageExtensions
{
    public static async ValueTask OpenDemoPage(this IPage page, string baseUrl, DemoPageSpec spec)
    {
        string route = spec.Route == "/" ? spec.Route : $"{spec.Route}?testScenarios=true";

        await page.OpenPage(baseUrl, route, async currentPage =>
                  {
                      await Assertions.Expect(currentPage.Locator("#main-content"))
                                      .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 15000 });
                      await Assertions.Expect(spec.ReadyLocator(currentPage))
                                      .ToBeVisibleAsync();

                      if (spec.Route == "/")
                      {
                          await Assertions.Expect(currentPage.Locator("#main-content h1"))
                                          .ToHaveTextAsync(spec.Heading);
                          await Assertions.Expect(currentPage.Locator("#main-content"))
                                          .ToContainTextAsync(spec.Description);
                          await Assertions.Expect(currentPage)
                                          .ToHaveTitleAsync("Headless Blazor UI Components & Radix UI Patterns | Bradix");

                      }
                      else
                      {
                          await Assertions.Expect(currentPage)
                                          .ToHaveTitleAsync(new Regex("Bradix", RegexOptions.IgnoreCase));
                          await Assertions.Expect(currentPage.Locator(".bradix-page-heading h1"))
                                          .ToBeVisibleAsync();
                          await Assertions.Expect(currentPage.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "View Code", Exact = true }).First)
                                          .ToBeVisibleAsync();
                      }
                  })
                  .NoSync();
    }
}
