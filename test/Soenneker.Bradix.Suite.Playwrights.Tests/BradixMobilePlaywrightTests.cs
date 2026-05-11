using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.Playwright;
using Soenneker.Playwrights.Extensions.TestPages;
using Soenneker.Playwrights.Session;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

[ClassDataSource<BradixPlaywrightHost>(Shared = SharedType.PerTestSession)]
[NotInParallel]
public sealed class BradixMobilePlaywrightTests : BradixComponentPlaywrightTest
{
    public BradixMobilePlaywrightTests(BradixPlaywrightHost host) : base(host)
    {
    }

    [Test]
    public async ValueTask Demo_routes_render_at_mobile_viewport_without_browser_or_layout_errors()
    {
        await using BrowserSession session = await CreateSession();
        IPage page = session.Page;
        var runtimeErrors = new List<string>();
        var failures = new List<string>();

        page.Console += (_, message) =>
        {
            if (message.Type == "error" && !IsIgnoredConsoleError(message.Text))
                runtimeErrors.Add($"console: {message.Text}");
        };

        page.PageError += (_, error) => runtimeErrors.Add($"page: {error}");

        await page.SetViewportSizeAsync(390, 844);

        foreach (DemoPageSpec spec in DemoPageSpecs.All)
        {
            runtimeErrors.Clear();

            try
            {
                await page.OpenDemoPage(BaseUrl, spec);

                string bodyText = await page.Locator("body").InnerTextAsync(new LocatorInnerTextOptions { Timeout = 5000 });

                if (string.IsNullOrWhiteSpace(bodyText))
                    failures.Add($"{spec.Route}: empty page body");

                bool hasVisibleBlazorError = await page.EvaluateAsync<bool>(
                    @"() => {
                        const errorUi = document.querySelector('#blazor-error-ui');
                        if (!errorUi) return false;

                        const style = getComputedStyle(errorUi);
                        return style.display !== 'none' && style.visibility !== 'hidden' && errorUi.offsetParent !== null;
                    }");

                if (hasVisibleBlazorError)
                    failures.Add($"{spec.Route}: visible Blazor error UI");

                double overflow = await page.EvaluateAsync<double>(
                    @"() => Math.max(0, document.documentElement.scrollWidth - window.innerWidth)");

                if (overflow > 24)
                {
                    string offenders = await page.EvaluateAsync<string>(
                        @"() => Array.from(document.querySelectorAll('body *'))
                            .map(element => {
                                const rect = element.getBoundingClientRect();
                                return { element, rect };
                            })
                            .filter(item => item.rect.right > window.innerWidth + 1)
                            .slice(0, 5)
                            .map(item => `${item.element.tagName.toLowerCase()}${item.element.id ? '#' + item.element.id : ''}${item.element.className ? '.' + String(item.element.className).trim().replace(/\s+/g, '.') : ''} right=${Math.round(item.rect.right)}`)
                            .join(', ')");
                    failures.Add($"{spec.Route}: horizontal overflow {overflow}px at 390px viewport ({offenders})");
                }

                if (spec.Route == "/")
                    await Assertions.Expect(page.Locator(".docs-shell__main")).ToContainTextAsync("A low-level Blazor component library for building accessible design systems and web apps.");

                if (runtimeErrors.Count > 0)
                    failures.Add($"{spec.Route}: {string.Join(" | ", runtimeErrors.Distinct())}");
            }
            catch (Exception exception)
            {
                failures.Add($"{spec.Route}: {exception.Message}");
            }
        }

        failures.Should().BeEmpty();
    }

    private static bool IsIgnoredConsoleError(string text)
    {
        return text.Contains("favicon", StringComparison.OrdinalIgnoreCase) ||
               text.Contains("net::ERR_NAME_NOT_RESOLVED", StringComparison.OrdinalIgnoreCase);
    }
}
