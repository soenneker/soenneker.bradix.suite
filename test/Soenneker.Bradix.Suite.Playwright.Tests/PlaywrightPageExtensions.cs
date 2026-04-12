using Microsoft.Playwright;
using System;
using System.Text.RegularExpressions;
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

    public static string GetRouteUrl(this Fixture fixture, string route)
    {
        return route == "/" ? fixture.BaseUrl : $"{fixture.BaseUrl}{route.TrimStart('/')}";
    }

    public static async Task OpenDemoPageAsync(this IPage page, Fixture fixture, DemoPageSpec spec)
    {
        await page.GotoAsync(fixture.GetRouteUrl(spec.Route), new PageGotoOptions
        {
            WaitUntil = WaitUntilState.Load
        });

        if (spec.Route == "/")
        {
            await Assertions.Expect(page).ToHaveTitleAsync("Bradix Component Library Demo");
            await Assertions.Expect(page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Overview", Exact = true })).ToBeVisibleAsync();
        }
        else
        {
            await Assertions.Expect(page).ToHaveTitleAsync(new Regex(Regex.Escape(spec.Title), RegexOptions.IgnoreCase));
            await Assertions.Expect(page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = spec.Title, Exact = true })).ToBeVisibleAsync();
        }

        await Assertions.Expect(page.GetByRole(AriaRole.Navigation, new PageGetByRoleOptions { Name = "Bradix primitives" })).ToBeVisibleAsync();
        await Assertions.Expect(spec.Route == "/"
            ? page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = spec.Heading, Exact = true })
            : page.Locator(".demo-page-intro h1")).ToHaveTextAsync(spec.Heading);
        await Assertions.Expect(page.Locator(".docs-shell__main")).ToContainTextAsync(spec.Description);
        await Assertions.Expect(spec.ReadyLocator(page)).ToBeVisibleAsync();
    }

    public static ILocator VisibleMenu(this IPage page)
    {
        return page.Locator("[role='menu']:visible").Last;
    }

    public static ILocator VisibleDialog(this IPage page)
    {
        return page.Locator("[role='dialog']:visible").Last;
    }

    public static ILocator VisibleListBox(this IPage page)
    {
        return page.Locator("[role='listbox']:visible").Last;
    }
}
