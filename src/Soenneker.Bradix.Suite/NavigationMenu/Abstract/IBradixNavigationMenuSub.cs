using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Nested navigation menu that is not the root landmark menu.
/// </summary>
public interface IBradixNavigationMenuSub
{
    /// <summary>Controlled value for the open item.</summary>
    string? Value { get; set; }

    /// <summary>Initial value for uncontrolled usage.</summary>
    string? DefaultValue { get; set; }

    /// <summary>Raised when the value changes (two-way bind).</summary>
    EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Raised when the value changes.</summary>
    EventCallback<string?> OnValueChange { get; set; }

    /// <summary>Layout orientation.</summary>
    BradixOrientation Orientation { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Submenu structure content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes forwarded to the inner menu.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
