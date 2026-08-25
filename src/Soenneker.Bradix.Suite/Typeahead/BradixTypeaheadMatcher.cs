using System;
using System.Collections.Generic;

namespace Soenneker.Bradix;

/// <summary>
/// Implements the matching rules Radix uses for transient typeahead selection.
/// </summary>
public static class BradixTypeaheadMatcher
{
    /// <summary>
    /// Executes the find next match operation.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="search">The search.</param>
    /// <param name="currentMatch">The current match.</param>
    /// <returns>The result of the operation.</returns>
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
    /// Executes the find next item operation.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="search">The search.</param>
    /// <param name="currentItem">The current item.</param>
    /// <param name="textSelector">The text selector.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns>The result of the operation.</returns>
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
    /// Executes the normalize search operation.
    /// </summary>
    /// <param name="search">The search.</param>
    /// <returns>The result of the operation.</returns>
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
