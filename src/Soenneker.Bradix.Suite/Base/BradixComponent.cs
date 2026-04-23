using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Soenneker.Extensions.String;

namespace Soenneker.Bradix;

///<inheritdoc cref="IBradixComponent"/>
public abstract class BradixComponent : ComponentBase, IBradixComponent
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
        var attributes = new Dictionary<string, object>();

        SetAttribute(attributes, "id", Id);

        MergeClassAttribute(attributes, Class);
        MergeStyleAttribute(attributes, Style);

        foreach ((var key, var value) in values)
        {
            SetAttribute(attributes, key, value);
        }

        if (AdditionalAttributes is null || AdditionalAttributes.Count == 0)
            return attributes;

        foreach ((var key, var value) in AdditionalAttributes)
        {
            if (string.Equals(key, "class", StringComparison.OrdinalIgnoreCase))
            {
                MergeClassAttribute(attributes, value?.ToString());
                continue;
            }

            if (string.Equals(key, "style", StringComparison.OrdinalIgnoreCase))
            {
                MergeStyleAttribute(attributes, value?.ToString());
                continue;
            }

            SetAttribute(attributes, key, value);
        }

        return attributes;
    }

    protected static string OpenDataState(bool open) => open ? "open" : "closed";

    protected static void MergeClassAttribute(IDictionary<string, object> attributes, string? value)
    {
        if (value.IsNullOrWhiteSpace())
            return;

        if (attributes.TryGetValue("class", out var existing) && existing is not null)
        {
            var existingText = existing.ToString() ?? string.Empty;

            attributes["class"] = existingText.IsNullOrWhiteSpace() ? value : $"{existingText} {value}";
            return;
        }

        attributes["class"] = value;
    }

    protected static void MergeStyleAttribute(IDictionary<string, object> attributes, string? value)
    {
        if (value.IsNullOrWhiteSpace())
            return;

        if (attributes.TryGetValue("style", out var existing) && existing is not null)
        {
            var existingText = existing.ToString() ?? string.Empty;

            if (existingText.IsNullOrWhiteSpace())
            {
                attributes["style"] = value;
                return;
            }

            var trimmed = existingText.TrimEnd();

            attributes["style"] = trimmed.EndsWith(';') ? $"{trimmed} {value}" : $"{trimmed}; {value}";
            return;
        }

        attributes["style"] = value;
    }

    internal static string? MergeStyleValues(string? existingValue, string? newValue)
    {
        if (existingValue.IsNullOrWhiteSpace())
            return newValue;

        if (newValue.IsNullOrWhiteSpace())
            return existingValue;

        var trimmed = existingValue!.TrimEnd();
        return trimmed.EndsWith(';') ? $"{trimmed} {newValue}" : $"{trimmed}; {newValue}";
    }

    protected static void SetAttribute(IDictionary<string, object> attributes, string key, object? value)
    {
        if (value is null)
            return;

        attributes[key] = value;
    }
}
