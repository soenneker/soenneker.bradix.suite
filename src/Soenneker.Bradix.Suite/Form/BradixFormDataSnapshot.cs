using System;
using System.Collections.Generic;

namespace Soenneker.Bradix;

public sealed class BradixFormDataSnapshot
{
    public Dictionary<string, string[]> Values { get; set; } = new(StringComparer.Ordinal);

    public bool Contains(string name)
    {
        return Values.ContainsKey(name);
    }

    public string? Get(string name)
    {
        return Values.TryGetValue(name, out string[]? values) && values.Length > 0
            ? values[0]
            : null;
    }

    public IReadOnlyList<string> GetAll(string name)
    {
        return Values.TryGetValue(name, out string[]? values)
            ? values
            : Array.Empty<string>();
    }
}
