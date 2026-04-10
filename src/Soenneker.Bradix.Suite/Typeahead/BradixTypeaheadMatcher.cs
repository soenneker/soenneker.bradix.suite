using System;
using System.Collections.Generic;
using System.Linq;

namespace Soenneker.Bradix.Suite.Typeahead;

/// <summary>
/// Implements the matching rules Radix uses for transient typeahead selection.
/// </summary>
public static class BradixTypeaheadMatcher
{
    public static string? FindNextMatch(IReadOnlyList<string> values, string search, string? currentMatch = null)
    {
        if (values.Count == 0 || string.IsNullOrWhiteSpace(search))
        {
            return null;
        }

        string normalizedSearch = NormalizeSearch(search);
        int currentMatchIndex = currentMatch is null ? -1 : IndexOf(values, currentMatch, StringComparer.Ordinal);
        IReadOnlyList<string> wrappedValues = WrapArray(values, Math.Max(currentMatchIndex, 0));
        bool excludeCurrentMatch = normalizedSearch.Length == 1;

        string? nextMatch = wrappedValues
            .Where(value => !excludeCurrentMatch || !string.Equals(value, currentMatch, StringComparison.Ordinal))
            .FirstOrDefault(value => value.StartsWith(normalizedSearch, StringComparison.OrdinalIgnoreCase));

        return string.Equals(nextMatch, currentMatch, StringComparison.Ordinal) ? null : nextMatch;
    }

    public static TItem? FindNextItem<TItem>(IReadOnlyList<TItem> items, string search, TItem? currentItem, Func<TItem, string?> textSelector,
        IEqualityComparer<TItem>? comparer = null)
    {
        if (items.Count == 0 || string.IsNullOrWhiteSpace(search))
        {
            return default;
        }

        comparer ??= EqualityComparer<TItem>.Default;

        string normalizedSearch = NormalizeSearch(search);
        int currentItemIndex = currentItem is null ? -1 : IndexOf(items, currentItem, comparer);
        IReadOnlyList<TItem> wrappedItems = WrapArray(items, Math.Max(currentItemIndex, 0));
        bool excludeCurrentItem = normalizedSearch.Length == 1 && currentItem is not null;

        TItem? nextItem = wrappedItems.FirstOrDefault(item =>
        {
            if (excludeCurrentItem && currentItem is not null && comparer.Equals(item, currentItem))
            {
                return false;
            }

            string textValue = textSelector(item)?.Trim() ?? string.Empty;
            return textValue.StartsWith(normalizedSearch, StringComparison.OrdinalIgnoreCase);
        });

        return currentItem is not null && nextItem is not null && comparer.Equals(nextItem, currentItem) ? default : nextItem;
    }

    public static string NormalizeSearch(string search)
    {
        if (string.IsNullOrEmpty(search))
        {
            return string.Empty;
        }

        bool isRepeated = search.Length > 1 && search.All(character => character == search[0]);
        return isRepeated ? search[0].ToString() : search;
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

    private static IReadOnlyList<TItem> WrapArray<TItem>(IReadOnlyList<TItem> items, int startIndex)
    {
        if (items.Count == 0)
        {
            return Array.Empty<TItem>();
        }

        TItem[] wrapped = new TItem[items.Count];

        for (var i = 0; i < items.Count; i++)
        {
            wrapped[i] = items[(startIndex + i) % items.Count];
        }

        return wrapped;
    }
}
