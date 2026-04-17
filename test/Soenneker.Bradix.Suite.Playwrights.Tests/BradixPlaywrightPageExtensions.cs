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
        await page.OpenPage(baseUrl, spec.Route, async currentPage =>
                  {
                      await Assertions.Expect(currentPage.Locator(".docs-shell__main"))
                                      .ToBeVisibleAsync();
                      await Assertions.Expect(spec.Route == "/"
                                          ? currentPage.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = spec.Heading, Exact = true })
                                          : currentPage.Locator(".demo-page-intro h1"))
                                      .ToHaveTextAsync(spec.Heading);
                      await Assertions.Expect(currentPage.Locator(".docs-shell__main"))
                                      .ToContainTextAsync(spec.Description);
                      await Assertions.Expect(spec.ReadyLocator(currentPage))
                                      .ToBeVisibleAsync();

                      if (spec.Route == "/")
                      {
                          await Assertions.Expect(currentPage)
                                          .ToHaveTitleAsync("Bradix Component Library Demo");
                          await Assertions.Expect(currentPage.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Overview", Exact = true }))
                                          .ToBeVisibleAsync();
                      }
                      else
                      {
                          await Assertions.Expect(currentPage)
                                          .ToHaveTitleAsync(new Regex(Regex.Escape(spec.Title), RegexOptions.IgnoreCase));
                          await Assertions.Expect(currentPage.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = spec.Title, Exact = true }))
                                          .ToBeVisibleAsync();
                      }
                  })
                  .NoSync();
    }
}
