using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Groups a form field name with optional server invalid state and control id wiring.
/// </summary>
public interface IBradixFormField
{
    /// <summary>Logical field name used for validation wiring.</summary>
    string Name { get; set; }

    /// <summary>When true, the field is treated as invalid from server validation.</summary>
    bool ServerInvalid { get; set; }

    /// <summary>Explicit id for the control; falls back to an auto id when null.</summary>
    string? ControlId { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Child content for the field group.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
