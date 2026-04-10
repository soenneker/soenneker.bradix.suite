using System;
using System.Collections.Generic;

namespace Soenneker.Bradix.Suite.RovingFocus;

internal static class BradixRovingFocusNavigator
{
    public static bool TryGetTargetIndex(string key, string? orientation, string? dir, int currentIndex, int count, bool loop, out int targetIndex)
    {
        targetIndex = currentIndex;

        if (count <= 0)
            return false;

        string effectiveDir = dir == "rtl" ? "rtl" : "ltr";
        string normalizedKey = GetDirectionAwareKey(key, effectiveDir);

        if (orientation == "vertical" && (normalizedKey == "ArrowLeft" || normalizedKey == "ArrowRight"))
            return false;

        if (orientation == "horizontal" && (normalizedKey == "ArrowUp" || normalizedKey == "ArrowDown"))
            return false;

        switch (normalizedKey)
        {
            case "Home":
            case "PageUp":
                targetIndex = 0;
                return true;
            case "End":
            case "PageDown":
                targetIndex = count - 1;
                return true;
            case "ArrowLeft":
            case "ArrowUp":
                targetIndex = currentIndex - 1;
                if (targetIndex < 0)
                {
                    if (!loop)
                        return false;

                    targetIndex = count - 1;
                }

                return true;
            case "ArrowRight":
            case "ArrowDown":
                targetIndex = currentIndex + 1;
                if (targetIndex >= count)
                {
                    if (!loop)
                        return false;

                    targetIndex = 0;
                }

                return true;
            default:
                return false;
        }
    }

    public static bool IsNavigationKey(string key)
    {
        return NavigationKeys.Contains(key);
    }

    private static string GetDirectionAwareKey(string key, string dir)
    {
        if (dir != "rtl")
            return key;

        return key switch
        {
            "ArrowLeft" => "ArrowRight",
            "ArrowRight" => "ArrowLeft",
            _ => key
        };
    }

    private static readonly HashSet<string> NavigationKeys =
    [
        "ArrowLeft",
        "ArrowRight",
        "ArrowUp",
        "ArrowDown",
        "Home",
        "End",
        "PageUp",
        "PageDown"
    ];
}
