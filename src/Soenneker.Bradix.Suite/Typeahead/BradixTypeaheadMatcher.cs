using System;
using System.Collections.Generic;
using System.Text;

namespace Soenneker.Bradix;

/// <summary>
/// Implements the matching rules Radix uses for transient typeahead selection.
/// </summary>
public static class BradixTypeaheadMatcher
{
    public static string? FindNextMatch(IReadOnlyList<string> values, string search, string? currentMatch = null)
    {
        if (values.Count == 0 || string.IsNullOrEmpty(search))
        {
            return null;
        }

        string normalizedSearch = NormalizeSearch(search);
        int currentMatchIndex = currentMatch is null ? -1 : IndexOf(values, currentMatch, StringComparer.Ordinal);
        int startIndex = Math.Max(currentMatchIndex, 0);
        bool excludeCurrentMatch = normalizedSearch.Length == 1;
        string? nextMatch = null;

        for (int i = 0; i < values.Count; i++)
        {
            string value = values[(startIndex + i) % values.Count];

            if (excludeCurrentMatch && string.Equals(value, currentMatch, StringComparison.Ordinal))
                continue;

            if (value.StartsWith(normalizedSearch, StringComparison.OrdinalIgnoreCase))
            {
                nextMatch = value;
                break;
            }
        }

        return string.Equals(nextMatch, currentMatch, StringComparison.Ordinal) ? null : nextMatch;
    }

    public static TItem? FindNextItem<TItem>(IReadOnlyList<TItem> items, string search, TItem? currentItem, Func<TItem, string?> textSelector,
        IEqualityComparer<TItem>? comparer = null)
    {
        if (items.Count == 0 || string.IsNullOrEmpty(search))
        {
            return default;
        }

        comparer ??= EqualityComparer<TItem>.Default;

        string normalizedSearch = NormalizeSearch(search);
        int currentItemIndex = currentItem is null ? -1 : IndexOf(items, currentItem, comparer);
        int startIndex = Math.Max(currentItemIndex, 0);
        bool excludeCurrentItem = normalizedSearch.Length == 1 && currentItem is not null;
        TItem? nextItem = default;

        for (int i = 0; i < items.Count; i++)
        {
            TItem item = items[(startIndex + i) % items.Count];

            if (excludeCurrentItem && currentItem is not null && comparer.Equals(item, currentItem))
                continue;

            string textValue = textSelector(item) ?? string.Empty;
            if (textValue.StartsWith(normalizedSearch, StringComparison.OrdinalIgnoreCase))
            {
                nextItem = item;
                break;
            }
        }

        return currentItem is not null && nextItem is not null && comparer.Equals(nextItem, currentItem) ? default : nextItem;
    }

    public static string NormalizeSearch(string search)
    {
        if (string.IsNullOrEmpty(search))
        {
            return string.Empty;
        }

        if (search.Length <= 1)
            return search;

        string firstCharacter = search[0].ToString();

        foreach (string character in EnumerateJavaScriptStringIterator(search))
        {
            if (!string.Equals(character, firstCharacter, StringComparison.Ordinal))
                return search;
        }

        return firstCharacter;
    }

    private static IEnumerable<string> EnumerateJavaScriptStringIterator(string value)
    {
        foreach (Rune rune in value.EnumerateRunes())
        {
            yield return rune.ToString();
        }
    }

    private static int IndexOf<TItem>(IReadOnlyList<TItem> items, TItem item, IEqualityComparer<TItem> comparer)
    {
        for (var i = 0; i < items.Count; i++)
        {
            if (comparer.Equals(items[i], item))
            {
                return i;
            }
        }

        return -1;
    }

}
