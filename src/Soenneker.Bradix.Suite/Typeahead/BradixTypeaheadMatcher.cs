using System;
using System.Collections.Generic;

namespace Soenneker.Bradix;

/// <summary>
/// Implements the matching rules Radix uses for transient typeahead selection.
/// </summary>
public static class BradixTypeaheadMatcher
{
    /// <summary>
    /// Finds next Match.
    /// </summary>
    /// <param name="values">Values supplied to find next match.</param>
    /// <param name="search">Search text or criteria to apply.</param>
    /// <param name="currentMatch">Current Match for the find next match operation.</param>
    /// <returns>The resulting text.</returns>
    public static string? FindNextMatch(IReadOnlyList<string> values, string search, string? currentMatch = null)
    {
        if (values.Count == 0 || string.IsNullOrEmpty(search))
        {
            return null;
        }

        ReadOnlySpan<char> normalizedSearch = GetNormalizedSearch(search);
        int currentMatchIndex = currentMatch is null ? -1 : IndexOf(values, currentMatch, StringComparer.Ordinal);
        int startIndex = Math.Max(currentMatchIndex, 0);
        bool excludeCurrentMatch = normalizedSearch.Length == 1;
        string? nextMatch = null;
        int valueIndex = startIndex;

        for (int i = 0; i < values.Count; i++)
        {
            string value = values[valueIndex];

            if (excludeCurrentMatch && string.Equals(value, currentMatch, StringComparison.Ordinal))
            {
                valueIndex++;
                if (valueIndex == values.Count)
                    valueIndex = 0;
                continue;
            }

            if (value.AsSpan().StartsWith(normalizedSearch, StringComparison.OrdinalIgnoreCase))
            {
                nextMatch = value;
                break;
            }

            valueIndex++;
            if (valueIndex == values.Count)
                valueIndex = 0;
        }

        return string.Equals(nextMatch, currentMatch, StringComparison.Ordinal) ? null : nextMatch;
    }

    /// <summary>
    /// Finds the next item whose selected text starts with the search text, wrapping around the list as needed.
    /// </summary>
    /// <typeparam name="TItem">Type of item being searched.</typeparam>
    /// <param name="items">Ordered items to search.</param>
    /// <param name="search">Search text or criteria to apply.</param>
    /// <param name="currentItem">Currently selected item, used as the starting point.</param>
    /// <param name="textSelector">Callback that returns the searchable text for an item.</param>
    /// <param name="comparer">Comparer used to identify the current item.</param>
    /// <returns>The next matching item, or <see langword="null"/> when no different match is found.</returns>
    public static TItem? FindNextItem<TItem>(IReadOnlyList<TItem> items, string search, TItem? currentItem, Func<TItem, string?> textSelector,
        IEqualityComparer<TItem>? comparer = null)
    {
        if (items.Count == 0 || string.IsNullOrEmpty(search))
        {
            return default;
        }

        comparer ??= EqualityComparer<TItem>.Default;

        ReadOnlySpan<char> normalizedSearch = GetNormalizedSearch(search);
        int currentItemIndex = currentItem is null ? -1 : IndexOf(items, currentItem, comparer);
        int startIndex = Math.Max(currentItemIndex, 0);
        bool excludeCurrentItem = normalizedSearch.Length == 1 && currentItem is not null;
        TItem? nextItem = default;
        int itemIndex = startIndex;

        for (int i = 0; i < items.Count; i++)
        {
            TItem item = items[itemIndex];

            if (excludeCurrentItem && currentItem is not null && comparer.Equals(item, currentItem))
            {
                itemIndex++;
                if (itemIndex == items.Count)
                    itemIndex = 0;
                continue;
            }

            string textValue = textSelector(item) ?? string.Empty;
            if (textValue.AsSpan().StartsWith(normalizedSearch, StringComparison.OrdinalIgnoreCase))
            {
                nextItem = item;
                break;
            }

            itemIndex++;
            if (itemIndex == items.Count)
                itemIndex = 0;
        }

        return currentItem is not null && nextItem is not null && comparer.Equals(nextItem, currentItem) ? default : nextItem;
    }

    /// <summary>
    /// Normalizes search.
    /// </summary>
    /// <param name="search">Search text or criteria to apply.</param>
    /// <returns>The resulting text.</returns>
    public static string NormalizeSearch(string search)
    {
        if (string.IsNullOrEmpty(search))
            return string.Empty;

        ReadOnlySpan<char> normalized = GetNormalizedSearch(search);
        return normalized.Length == search.Length ? search : normalized.ToString();
    }

    private static ReadOnlySpan<char> GetNormalizedSearch(string search)
    {
        if (search.Length <= 1)
            return search;

        char firstCharacter = search[0];

        if (char.IsSurrogate(firstCharacter))
            return search;

        for (var i = 1; i < search.Length; i++)
        {
            if (search[i] != firstCharacter)
                return search;
        }

        return search.AsSpan(0, 1);
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
