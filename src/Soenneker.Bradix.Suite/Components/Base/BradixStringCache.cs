using System;
using System.Buffers;
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

    internal static string ChildValueId(string parent, string value, ref string? previous)
    {
        // Option values are arbitrary text, unlike structural ID segments. Escape
        // into a reusable buffer so unchanged IDs do not allocate temporary strings.
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(value);
        Span<char> escaped = stackalloc char[256];
        char[]? rented = null;
        try
        {
            if (!Uri.TryEscapeDataString(value, escaped, out int written))
            {
                escaped = rented = ArrayPool<char>.Shared.Rent(checked(value.Length * 12));
                if (!Uri.TryEscapeDataString(value, escaped, out written))
                    throw new InvalidOperationException("Unable to encode the component value as a DOM ID.");
            }
            var suffix = escaped[..written];
            if (!Matches(previous, parent, "-", suffix))
                previous = string.Concat(parent.AsSpan(), "-", suffix);
            return previous!;
        }
        finally
        {
            if (rented is not null) ArrayPool<char>.Shared.Return(rented);
        }
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
