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

    public Dictionary<string, object> ExposeBuildAttributes(params (string Key, object? Value)[] values)
    {
        return BuildAttributes(values);
    }
}
