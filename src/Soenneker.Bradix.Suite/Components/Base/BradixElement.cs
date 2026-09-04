using System;
using System.Collections.Generic;
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
    private readonly Dictionary<string, object> _attributesA = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, object> _attributesB = new(StringComparer.OrdinalIgnoreCase);
    private BradixAttributeDictionary? _additionalAttributeDictionaries;
    private bool _useAttributesA;

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

    protected override Dictionary<string, object> BuildAttributes(params (string Key, object? Value)[] values)
    {
        Dictionary<string, object> attributes = BeginAttributes();

        for (var i = 0; i < values.Length; i++)
            SetAttribute(attributes, values[i].Key, values[i].Value);

        return CompleteAttributes(attributes);
    }

    protected Dictionary<string, object> BuildAttributes((string Key, object? Value) value1)
    {
        Dictionary<string, object> attributes = BeginAttributes();
        SetAttribute(attributes, value1.Key, value1.Value);
        return CompleteAttributes(attributes);
    }

    protected Dictionary<string, object> BuildAttributes((string Key, object? Value) value1, (string Key, object? Value) value2)
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
        Dictionary<string, object> attributes = _useAttributesA ? _attributesA : _attributesB;
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

    private static void MergeAdditionalAttribute(Dictionary<string, object> attributes, string key, object? value)
    {
        if (value is null)
            return;

        if (key.Equals("class", StringComparison.OrdinalIgnoreCase))
        {
            MergeClassAttribute(attributes, value as string ?? value.ToString());
            return;
        }

        if (key.Equals("style", StringComparison.OrdinalIgnoreCase))
        {
            MergeStyleAttribute(attributes, value as string ?? value.ToString());
            return;
        }

        attributes[key] = value;
    }
}

/// <inheritdoc cref="ILeptonContentElement" />
public abstract class BradixContentElement : BradixElement, ILeptonContentElement
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}

/// <inheritdoc cref="ILeptonIdentifiableElement" />
public abstract class BradixIdentifiableElement : BradixElement, ILeptonIdentifiableElement
{
    [Parameter]
    public string? Id { get; set; }

    protected override string? AttributeId => Id;

    protected IReadOnlyDictionary<string, object> EffectiveAttributes => BuildAttributes();
}

/// <inheritdoc cref="ILeptonIdentifiableContentElement" />
public abstract class BradixIdentifiableContentElement : BradixIdentifiableElement, ILeptonIdentifiableContentElement
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
