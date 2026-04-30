using System.Collections.Generic;

namespace Soenneker.Bradix.Suite.Tests;

internal sealed class BradixExposedComponent : BradixComponent
{
    public void Configure(string id, string @class, string style, IReadOnlyDictionary<string, object> additionalAttributes)
    {
        Id = id;
        Class = @class;
        Style = style;
        AdditionalAttributes = additionalAttributes;
    }

    public Dictionary<string, object> ExposeBuildAttributes(string key1, object? value1, string key2, object? value2)
    {
        return BuildAttributes(key1, value1, key2, value2);
    }
}
