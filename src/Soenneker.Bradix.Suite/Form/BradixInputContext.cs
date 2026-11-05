using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Typed binding and validation state supplied by a BradixInput adapter.</summary>
public sealed class BradixInputContext<TValue>
{
    internal BradixInputContext(TValue? value, EventCallback<TValue> valueChanged,
        IReadOnlyDictionary<string, object> attributes, IReadOnlyList<string> validationMessages)
    {
        Value = value;
        ValueChanged = valueChanged;
        Attributes = attributes;
        ValidationMessages = validationMessages;
    }

    /// <summary>The bound model value. Forward this to the primitive's Value, Checked, or Values parameter.</summary>
    public TValue? Value { get; }

    /// <summary>Forward the primitive's typed change callback here to update the model and notify EditContext.</summary>
    public EventCallback<TValue> ValueChanged { get; }

    /// <summary>Forward to the interactive control. Contains the field name, validation CSS class, and aria-invalid state.</summary>
    public IReadOnlyDictionary<string, object> Attributes { get; }

    /// <summary>Current EditContext validation messages for this field.</summary>
    public IReadOnlyList<string> ValidationMessages { get; }
}
