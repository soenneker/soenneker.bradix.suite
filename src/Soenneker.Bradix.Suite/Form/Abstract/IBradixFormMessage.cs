using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Displays validation or custom messages for a form field.
/// </summary>
public interface IBradixFormMessage : IDisposable {
    /// <summary>Built-in validity key, custom matcher delegate, or null for generic invalid.</summary>
    object? Match { get; set; }

    /// <summary>When true, the message is shown regardless of match evaluation.</summary>
    bool ForceMatch { get; set; }

    /// <summary>Field name override; defaults to the cascading field name.</summary>
    string? Name { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Message content; default text is used when null.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

}
