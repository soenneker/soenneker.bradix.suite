using System.Threading.Tasks;
using Microsoft.Playwright;
using Soenneker.Playwrights.Tests.Unit;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

public abstract class BradixComponentPlaywrightTest : PlaywrightUnitTest
{
    protected BradixComponentPlaywrightTest(BradixPlaywrightHost host) : base(host)
    {
    }

    protected static async Task ClickJustOutsideActiveDialogAsync(IPage page, ILocator dialog)
    {
        var box = await dialog.BoundingBoxAsync();
        await Assert.That(box).IsNotNull();
        float x = box!.X > 40 ? box.X - 20 : box.X + box.Width + 20;
        float y = box.Y > 40 ? box.Y - 20 : box.Y + 20;
        await page.Mouse.ClickAsync(x, y);
    }

    protected static async Task ExpectActiveElementAsync(IPage page, ILocator locator)
    {
        string? id = await locator.GetAttributeAsync("id");
        await Assert.That(string.IsNullOrWhiteSpace(id)).IsFalse();

        await page.WaitForFunctionAsync(
            "(expectedId) => document.activeElement?.id === expectedId",
            id);
    }

    protected static async Task<bool> WaitForDialogTabBoundaryAsync(ILocator dialog, bool first, int attempts = 12)
    {
        for (var i = 0; i < attempts; i++)
        {
            if (await ActiveElementMatchesDialogTabBoundaryAsync(dialog, first))
                return true;

            await Task.Delay(25);
        }

        return false;
    }

    private static Task<bool> ActiveElementMatchesDialogTabBoundaryAsync(ILocator dialog, bool first)
    {
        return dialog.EvaluateAsync<bool>(
            @"(element, isFirst) => {
                const selector = [
                    'button:not([disabled])',
                    '[href]',
                    'input:not([disabled]):not([type=""hidden""])',
                    'select:not([disabled])',
                    'textarea:not([disabled])',
                    '[tabindex]:not([tabindex=""-1""])'
                ].join(',');

                const tabbables = Array.from(element.querySelectorAll(selector)).filter(node => {
                    if (!(node instanceof HTMLElement)) {
                        return false;
                    }

                    if (node.getAttribute('aria-hidden') === 'true') {
                        return false;
                    }

                    const style = window.getComputedStyle(node);
                    if (style.display === 'none' || style.visibility === 'hidden') {
                        return false;
                    }

                    return node.getClientRects().length > 0;
                });

                if (tabbables.length === 0) {
                    return false;
                }

                const boundary = isFirst ? tabbables[0] : tabbables[tabbables.length - 1];
                return document.activeElement === boundary;
            }",
            first);
    }
}
