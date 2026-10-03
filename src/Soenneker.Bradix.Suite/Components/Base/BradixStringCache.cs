using System;
using Soenneker.Blazor.Utils.Ids;

namespace Soenneker.Bradix;

internal static class BradixStringCache
{
    internal static string? MergeStyle(string? first, string? second, ref string? previous)
    {
        if (string.IsNullOrWhiteSpace(first)) return previous = second;
        if (string.IsNullOrWhiteSpace(second)) return previous = first;
        ReadOnlySpan<char> prefix = first.AsSpan().TrimEnd();
        string separator = prefix[^1] == ';' ? " " : "; ";
        if (!Matches(previous, prefix, separator, second))
            previous = string.Concat(prefix, separator, second);
        return previous;
    }

    internal static string? MergeClass(string? first, string? second, ref string? previous)
    {
        if (string.IsNullOrWhiteSpace(first)) return previous = second;
        if (string.IsNullOrWhiteSpace(second)) return previous = first;
        if (!Matches(previous, first, " ", second)) previous = string.Concat(first, " ", second);
        return previous;
    }

    internal static string ChildId(string parent, string suffix, ref string? previous)
    {
        if (parent is null || suffix is null || !Matches(previous, parent, "-", suffix))
            previous = BlazorIdGenerator.Child(parent!, suffix!);
        return previous!;
    }

    internal static string Concat(string first, string second, ref string? previous)
    {
        if (!Matches(previous, first, "", second)) previous = string.Concat(first, second);
        return previous!;
    }

    private static bool Matches(string? value, ReadOnlySpan<char> first, ReadOnlySpan<char> separator, ReadOnlySpan<char> second) =>
        value is not null && value.Length == first.Length + separator.Length + second.Length &&
        value.AsSpan(0, first.Length).SequenceEqual(first) &&
        value.AsSpan(first.Length, separator.Length).SequenceEqual(separator) &&
        value.AsSpan(first.Length + separator.Length).SequenceEqual(second);
}
