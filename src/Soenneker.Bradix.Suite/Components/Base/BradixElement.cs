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
        MergeAdditionalAttributes(attributes);
        return attributes;
    }
}

public abstract class BradixContentElement : BradixElement, ILeptonContentElement
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}

public abstract class BradixIdentifiableElement : BradixElement, ILeptonIdentifiableElement
{
    [Parameter]
    public string? Id { get; set; }

    protected override string? AttributeId => Id;

    protected IReadOnlyDictionary<string, object> EffectiveAttributes => BuildAttributes();
}

public abstract class BradixIdentifiableContentElement : BradixIdentifiableElement, ILeptonIdentifiableContentElement
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
