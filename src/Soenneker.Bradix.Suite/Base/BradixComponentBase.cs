using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix.Suite.Base;

/// <summary>
/// Minimal primitive base for id/class/style/attribute composition.
/// </summary>
public abstract class BradixComponentBase : ComponentBase
{
    [Parameter]
    public string? Id { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected Dictionary<string, object> BuildAttributes(params (string Key, object? Value)[] values)
    {
        Dictionary<string, object> attributes = AdditionalAttributes is not null
            ? new Dictionary<string, object>(AdditionalAttributes)
            : new Dictionary<string, object>();

        SetAttribute(attributes, "id", Id);
        MergeStringAttribute(attributes, "class", Class);
        MergeStringAttribute(attributes, "style", Style);

        foreach ((string key, object? value) in values)
        {
            SetAttribute(attributes, key, value);
        }

        return attributes;
    }

    protected static string DataState(bool open) => open ? "open" : "closed";

    private static void MergeStringAttribute(IDictionary<string, object> attributes, string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        if (attributes.TryGetValue(key, out object? existing) && existing is not null)
        {
            string merged = $"{existing} {value}".Trim();
            attributes[key] = merged;
            return;
        }

        attributes[key] = value;
    }

    private static void SetAttribute(IDictionary<string, object> attributes, string key, object? value)
    {
        if (value is null)
            return;

        attributes[key] = value;
    }
}
