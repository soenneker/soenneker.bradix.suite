using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.Playwright.Tests;

internal static class PlaywrightPageExtensions
{
    public static async Task GotoAndWaitForReadyAsync(this IPage page, string url, Func<IPage, ILocator> readyLocatorFactory, string? expectedTitle = null)
    {
        await page.GotoAsync(url, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.Load
        });

        if (!string.IsNullOrWhiteSpace(expectedTitle))
            await Assertions.Expect(page).ToHaveTitleAsync(expectedTitle);

        await Assertions.Expect(readyLocatorFactory(page)).ToBeVisibleAsync();
    }
}
