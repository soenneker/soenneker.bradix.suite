using System;
using System.Collections.Generic;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix form data snapshot.
/// </summary>
public sealed class BradixFormDataSnapshot
{
    /// <summary>
    /// Gets or sets values.
    /// </summary>
    public Dictionary<string, string[]> Values { get; set; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Returns the value produced by contains.
    /// </summary>
    /// <param name="name">Optional name used to scope the generated variant.</param>
    /// <returns>true if the value is present in the current window; otherwise, false.</returns>
    public bool Contains(string name)
    {
        return Values.ContainsKey(name);
    }

    /// <summary>
    /// Returns the configured resulting text used by the Bradix Form Data Snapshot.
    /// </summary>
    /// <param name="name">Optional name used to scope the generated variant.</param>
    /// <returns>The requested text.</returns>
    public string? Get(string name)
    {
        return Values.TryGetValue(name, out string[]? values) && values.Length > 0
            ? values[0]
            : null;
    }

    /// <summary>
    /// Gets all.
    /// </summary>
    /// <param name="name">Optional name used to scope the generated variant.</param>
    /// <returns>The requested collection.</returns>
    public IReadOnlyList<string> GetAll(string name)
    {
        return Values.TryGetValue(name, out string[]? values)
            ? values
            : Array.Empty<string>();
    }
}
