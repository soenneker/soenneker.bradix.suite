using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Bradix;

/// <summary>
/// Bradix element base that double-buffers attribute dictionaries between render-tree builds.
/// Blazor can retain a dictionary in the previous render tree, so adjacent renders must not
/// mutate the same instance. Alternating buffers removes steady-state allocations safely.
/// </summary>
public abstract class BradixElement : LeptonElement
{
    private Dictionary<string, object>? _attributesA;
    private Dictionary<string, object>? _attributesB;
    private BradixAttributeDictionary? _additionalAttributeDictionaries;
    private bool _useAttributesA;
    private string? _mergedAdditionalClass;
    private string? _mergedAdditionalStyle;

    protected virtual string? AttributeId => null;

    protected override Dictionary<string, object> BuildAttributes()
    {
        Dictionary<string, object> attributes = BeginAttributes();
        return CompleteAttributes(attributes);
    }

    protected override Dictionary<string, object> BuildAttributes(string key, object? value)
    {
        Dictionary<string, object> attributes = BeginAttributes();
        SetAttribute(attributes, key, value);
        return CompleteAttributes(attributes);
    }

    protected override Dictionary<string, object> BuildAttributes(string key1, object? value1, string key2, object? value2)
    {
        Dictionary<string, object> attributes = BeginAttributes();
        SetAttribute(attributes, key1, value1);
        SetAttribute(attributes, key2, value2);
        return CompleteAttributes(attributes);
    }

    protected override Dictionary<string, object> BuildAttributes(ReadOnlySpan<KeyValuePair<string, object?>> values)
    {
        Dictionary<string, object> attributes = BeginAttributes();

        foreach (KeyValuePair<string, object?> pair in values)
            SetAttribute(attributes, pair.Key, pair.Value);

        return CompleteAttributes(attributes);
    }

    // Older Lepton packages do not expose this overload yet.
#pragma warning disable CS0109
    protected new Dictionary<string, object> BuildAttributes(params ReadOnlySpan<(string Key, object? Value)> values)
#pragma warning restore CS0109
    {
        Dictionary<string, object> attributes = BeginAttributes();
        foreach (var value in values)
            SetAttribute(attributes, value.Key, value.Value);
        return CompleteAttributes(attributes);
    }

    protected override Dictionary<string, object> BuildAttributes(params (string Key, object? Value)[] values)
    {
        Dictionary<string, object> attributes = BeginAttributes();

        for (var i = 0; i < values.Length; i++)
            SetAttribute(attributes, values[i].Key, values[i].Value);

        return CompleteAttributes(attributes);
    }

    protected new Dictionary<string, object> BuildAttributes((string Key, object? Value) value1)
    {
        Dictionary<string, object> attributes = BeginAttributes();
        SetAttribute(attributes, value1.Key, value1.Value);
        return CompleteAttributes(attributes);
    }

    protected new Dictionary<string, object> BuildAttributes((string Key, object? Value) value1, (string Key, object? Value) value2)
    {
        Dictionary<string, object> attributes = BeginAttributes();
        SetAttribute(attributes, value1.Key, value1.Value);
        SetAttribute(attributes, value2.Key, value2.Value);
        return CompleteAttributes(attributes);
    }

    protected Dictionary<string, object> BuildAttributes((string Key, object? Value) value1, (string Key, object? Value) value2,
        (string Key, object? Value) value3)
    {
        Dictionary<string, object> attributes = BeginAttributes();
        SetAttribute(attributes, value1.Key, value1.Value);
        SetAttribute(attributes, value2.Key, value2.Value);
        SetAttribute(attributes, value3.Key, value3.Value);
        return CompleteAttributes(attributes);
    }

    protected Dictionary<string, object> BuildAttributes((string Key, object? Value) value1, (string Key, object? Value) value2,
        (string Key, object? Value) value3, (string Key, object? Value) value4)
    {
        Dictionary<string, object> attributes = BeginAttributes();
        SetAttribute(attributes, value1.Key, value1.Value);
        SetAttribute(attributes, value2.Key, value2.Value);
        SetAttribute(attributes, value3.Key, value3.Value);
        SetAttribute(attributes, value4.Key, value4.Value);
        return CompleteAttributes(attributes);
    }

    protected Dictionary<string, object> BuildAttributes((string Key, object? Value) value1, (string Key, object? Value) value2,
        (string Key, object? Value) value3, (string Key, object? Value) value4, (string Key, object? Value) value5)
    {
        Dictionary<string, object> attributes = BeginAttributes();
        SetAttribute(attributes, value1.Key, value1.Value);
        SetAttribute(attributes, value2.Key, value2.Value);
        SetAttribute(attributes, value3.Key, value3.Value);
        SetAttribute(attributes, value4.Key, value4.Value);
        SetAttribute(attributes, value5.Key, value5.Value);
        return CompleteAttributes(attributes);
    }

    protected Dictionary<string, object> BuildAttributes((string Key, object? Value) value1, (string Key, object? Value) value2,
        (string Key, object? Value) value3, (string Key, object? Value) value4, (string Key, object? Value) value5,
        (string Key, object? Value) value6)
    {
        Dictionary<string, object> attributes = BeginAttributes();
        SetAttribute(attributes, value1.Key, value1.Value);
        SetAttribute(attributes, value2.Key, value2.Value);
        SetAttribute(attributes, value3.Key, value3.Value);
        SetAttribute(attributes, value4.Key, value4.Value);
        SetAttribute(attributes, value5.Key, value5.Value);
        SetAttribute(attributes, value6.Key, value6.Value);
        return CompleteAttributes(attributes);
    }

    protected Dictionary<string, object> BuildAttributes((string Key, object? Value) value1, (string Key, object? Value) value2,
        (string Key, object? Value) value3, (string Key, object? Value) value4, (string Key, object? Value) value5,
        (string Key, object? Value) value6, (string Key, object? Value) value7)
    {
        Dictionary<string, object> attributes = BeginAttributes();
        SetAttribute(attributes, value1.Key, value1.Value);
        SetAttribute(attributes, value2.Key, value2.Value);
        SetAttribute(attributes, value3.Key, value3.Value);
        SetAttribute(attributes, value4.Key, value4.Value);
        SetAttribute(attributes, value5.Key, value5.Value);
        SetAttribute(attributes, value6.Key, value6.Value);
        SetAttribute(attributes, value7.Key, value7.Value);
        return CompleteAttributes(attributes);
    }

    protected Dictionary<string, object> BuildAttributes((string Key, object? Value) value1, (string Key, object? Value) value2,
        (string Key, object? Value) value3, (string Key, object? Value) value4, (string Key, object? Value) value5,
        (string Key, object? Value) value6, (string Key, object? Value) value7, (string Key, object? Value) value8)
    {
        Dictionary<string, object> attributes = BeginAttributes();
        SetAttribute(attributes, value1.Key, value1.Value);
        SetAttribute(attributes, value2.Key, value2.Value);
        SetAttribute(attributes, value3.Key, value3.Value);
        SetAttribute(attributes, value4.Key, value4.Value);
        SetAttribute(attributes, value5.Key, value5.Value);
        SetAttribute(attributes, value6.Key, value6.Value);
        SetAttribute(attributes, value7.Key, value7.Value);
        SetAttribute(attributes, value8.Key, value8.Value);
        return CompleteAttributes(attributes);
    }

    /// <summary>
    /// Builds a double-buffered copy of unmatched attributes for a child component.
    /// </summary>
    protected Dictionary<string, object> BuildAdditionalAttributes(int extraCapacity = 0)
    {
        return BuildAdditionalAttributes(AdditionalAttributes, extraCapacity);
    }

    /// <summary>
    /// Builds a double-buffered copy of the supplied attributes for a child component.
    /// </summary>
    protected Dictionary<string, object> BuildAdditionalAttributes(IReadOnlyDictionary<string, object>? attributes, int extraCapacity = 0)
    {
        _additionalAttributeDictionaries ??= new BradixAttributeDictionary();
        return _additionalAttributeDictionaries.Create(attributes, extraCapacity);
    }

    private Dictionary<string, object> BeginAttributes()
    {
        _useAttributesA = !_useAttributesA;
        ref Dictionary<string, object>? buffer = ref (_useAttributesA ? ref _attributesA : ref _attributesB);
        Dictionary<string, object> attributes = buffer ??= new(StringComparer.OrdinalIgnoreCase);
        attributes.Clear();
        MergeClassAttribute(attributes, Class);
        MergeStyleAttribute(attributes, Style);
        return attributes;
    }

    private Dictionary<string, object> CompleteAttributes(Dictionary<string, object> attributes)
    {
        SetAttribute(attributes, "id", AttributeId);
        MergeBradixAdditionalAttributes(attributes);
        return attributes;
    }

    private void MergeBradixAdditionalAttributes(Dictionary<string, object> attributes)
    {
        if (AdditionalAttributes is not { Count: > 0 })
            return;

        if (AdditionalAttributes is Dictionary<string, object> dictionary)
        {
            foreach (KeyValuePair<string, object> pair in dictionary)
                MergeAdditionalAttribute(attributes, pair.Key, pair.Value);

            return;
        }

        foreach ((string key, object value) in AdditionalAttributes)
            MergeAdditionalAttribute(attributes, key, value);
    }

    private void MergeAdditionalAttribute(Dictionary<string, object> attributes, string key, object? value)
    {
        if (value is null)
            return;

        if (key.Equals("class", StringComparison.OrdinalIgnoreCase))
        {
            string? text = value as string ?? value.ToString();
            if (string.IsNullOrWhiteSpace(text)) return;
            ref object? slot = ref CollectionsMarshal.GetValueRefOrAddDefault(attributes, "class", out _);
            slot = BradixStringCache.MergeClass(slot as string ?? slot?.ToString(), text, ref _mergedAdditionalClass);
            return;
        }

        if (key.Equals("style", StringComparison.OrdinalIgnoreCase))
        {
            string? text = value as string ?? value.ToString();
            if (string.IsNullOrWhiteSpace(text)) return;
            ref object? slot = ref CollectionsMarshal.GetValueRefOrAddDefault(attributes, "style", out _);
            slot = BradixStringCache.MergeStyle(slot as string ?? slot?.ToString(), text, ref _mergedAdditionalStyle);
            return;
        }

        attributes[key] = value;
    }
}
