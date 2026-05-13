using System.Collections.Generic;

namespace Soenneker.Bradix;

internal static class BradixAttributeDictionary
{
    public static Dictionary<string, object> Create(IReadOnlyDictionary<string, object>? additionalAttributes, int extraCapacity = 0)
    {
        int count = additionalAttributes?.Count ?? 0;
        var attributes = new Dictionary<string, object>(count + extraCapacity);

        if (additionalAttributes is null)
            return attributes;

        foreach ((string key, object value) in additionalAttributes)
        {
            attributes[key] = value;
        }

        return attributes;
    }
}
