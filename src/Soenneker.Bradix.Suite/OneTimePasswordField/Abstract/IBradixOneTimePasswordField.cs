using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// One-time password / OTP input group coordinating segmented inputs and hidden submission.
/// </summary>
public interface IBradixOneTimePasswordField : IAsyncDisposable{
    /// <summary>Value for the autocomplete attribute on supported inputs.</summary>
    string AutoComplete { get; set; }

    /// <summary>When true, focuses the first input on mount.</summary>
    bool AutoFocus { get; set; }

    /// <summary>When true, submits the owning form when all slots are filled.</summary>
    bool AutoSubmit { get; set; }

    /// <summary>Controlled OTP string.</summary>
    string? Value { get; set; }

    /// <summary>Initial value for uncontrolled usage.</summary>
    string? DefaultValue { get; set; }

    /// <summary>Raised when the value changes (two-way bind).</summary>
    EventCallback<string> ValueChanged { get; set; }

    /// <summary>Raised when the value changes.</summary>
    EventCallback<string> OnValueChange { get; set; }

    /// <summary>Raised when auto-submit fires.</summary>
    EventCallback<string> OnAutoSubmit { get; set; }

    /// <summary>When true, inputs are disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Explicit text direction.</summary>
    string? Dir { get; set; }

    /// <summary><c>horizontal</c> or <c>vertical</c> layout.</summary>
    string Orientation { get; set; }

    /// <summary>Optional form id association for the hidden input.</summary>
    string? Form { get; set; }

    /// <summary>Name submitted with the hidden input.</summary>
    string? Name { get; set; }

    /// <summary>Per-cell placeholder characters.</summary>
    string? Placeholder { get; set; }

    /// <summary>When true, inputs are read-only.</summary>
    bool ReadOnly { get; set; }

    /// <summary>HTML input type for cells.</summary>
    string Type { get; set; }

    /// <summary>Built-in validation mode (<c>numeric</c>, <c>alpha</c>, etc.).</summary>
    string ValidationType { get; set; }

    /// <summary>Optional custom sanitizer applied when <see cref="ValidationType"/> is <c>none</c>.</summary>
    Func<string, string>? SanitizeValue { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>OTP inputs and hidden field.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }


    /// <summary>Interop handler when the associated form resets.</summary>
    Task HandleFormReset();
}
