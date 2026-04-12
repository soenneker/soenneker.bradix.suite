using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Single OTP cell input participating in the OTP field group.
/// </summary>
public interface IBradixOneTimePasswordFieldInput
{
    /// <summary>Explicit cell index; otherwise auto-assigned.</summary>
    int? Index { get; set; }

    /// <summary>Raised when input fails built-in validation.</summary>
    EventCallback<string> OnInvalidChange { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Child content (typically unused for a self-closing input).</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Unregisters OTP input interop.</summary>
    ValueTask DisposeAsync();

    /// <summary>Interop handler for paste events from script.</summary>
    Task HandlePasteAsync(string value);
}
