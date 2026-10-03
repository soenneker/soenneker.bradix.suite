using System;
using System.Collections.Generic;

namespace Soenneker.Bradix;

internal static class BradixStringCollection
{
    internal static List<string> CopyValues(IEnumerable<string>? values, int extraCapacity = 0)
    {
        if (values is null)
            return [];

        if (values is ICollection<string> collection)
        {
            List<string> result = new(collection.Count + extraCapacity);
            result.AddRange(collection);
            return result;
        }

        if (values is IReadOnlyList<string> list)
        {
            List<string> result = new(list.Count + extraCapacity);
            for (var i = 0; i < list.Count; i++) result.Add(list[i]);
            return result;
        }

        if (values is IReadOnlyCollection<string> readOnlyCollection)
        {
            List<string> result = new(readOnlyCollection.Count + extraCapacity);
            foreach (string value in readOnlyCollection)
                result.Add(value);
            return result;
        }

        return [.. values];
    }

    internal static bool ContainsOrdinal(IEnumerable<string> values, string value)
    {
        if (values is IReadOnlyList<string> list)
        {
            for (var i = 0; i < list.Count; i++)
                if (string.Equals(list[i], value, StringComparison.Ordinal)) return true;
            return false;
        }
        foreach (string candidate in values)
        {
            if (string.Equals(candidate, value, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    internal static bool RemoveAllOrdinal(List<string> values, string value)
    {
        var write = 0;
        for (var i = 0; i < values.Count; i++)
        {
            string candidate = values[i];
            if (!string.Equals(candidate, value, StringComparison.Ordinal)) values[write++] = candidate;
        }
        if (write == values.Count) return false;
        values.RemoveRange(write, values.Count - write);
        return true;
    }
}
