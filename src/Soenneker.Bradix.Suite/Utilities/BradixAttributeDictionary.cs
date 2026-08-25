using System.Collections.Generic;

namespace Soenneker.Bradix;

/// <summary>
/// Double-buffers attribute dictionaries that are passed to child components. Blazor retains
/// the previous render tree, so a single reused dictionary would be mutated too early.
/// </summary>
internal sealed class BradixAttributeDictionary
{
    private Dictionary<string, object>? _first;
    private Dictionary<string, object>? _second;
    private bool _useFirst;

    public Dictionary<string, object> Create(IReadOnlyDictionary<string, object>? additionalAttributes, int extraCapacity = 0)
    {
        int count = additionalAttributes?.Count ?? 0;
        int capacity = count + extraCapacity;
        _useFirst = !_useFirst;

        ref Dictionary<string, object>? buffer = ref (_useFirst ? ref _first : ref _second);
        Dictionary<string, object> attributes;

        if (buffer is null)
        {
            attributes = new Dictionary<string, object>(capacity);
            buffer = attributes;
        }
        else
        {
            attributes = buffer;
            attributes.Clear();
            attributes.EnsureCapacity(capacity);
        }

        if (additionalAttributes is null)
            return attributes;

        if (additionalAttributes is Dictionary<string, object> dictionary)
        {
            foreach (KeyValuePair<string, object> pair in dictionary)
                attributes[pair.Key] = pair.Value;

            return attributes;
        }

        foreach ((string key, object value) in additionalAttributes)
        {
            attributes[key] = value;
        }

        return attributes;
    }
}
