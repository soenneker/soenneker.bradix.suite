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
    /// Executes the contains operation.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A value indicating whether the operation succeeded.</returns>
    public bool Contains(string name)
    {
        return Values.ContainsKey(name);
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The result of the operation.</returns>
    public string? Get(string name)
    {
        return Values.TryGetValue(name, out string[]? values) && values.Length > 0
            ? values[0]
            : null;
    }

    /// <summary>
    /// Gets all.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The result of the operation.</returns>
    public IReadOnlyList<string> GetAll(string name)
    {
        return Values.TryGetValue(name, out string[]? values)
            ? values
            : Array.Empty<string>();
    }
}
