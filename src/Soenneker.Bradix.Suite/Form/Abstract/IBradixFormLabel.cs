using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Form-specific label wrapper that forwards Radix label props and field state attributes.
/// </summary>
public interface IBradixFormLabel
{
    /// <summary>Value for the label's <c>for</c> attribute when not inferred from the field.</summary>
    string? For { get; set; }

    /// <summary>Raised when a guarded mouse down is forwarded from the underlying label primitive.</summary>
    EventCallback<MouseEventArgs> OnMouseDown { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Label content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
