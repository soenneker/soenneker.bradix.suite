using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Root popover coordinating open state, modal behavior, and anchor/trigger wiring.
/// </summary>
public interface IBradixPopover
{
    /// <summary>Controlled open state; null for uncontrolled usage.</summary>
    bool? Open { get; set; }

    /// <summary>Initial open state for uncontrolled usage.</summary>
    bool DefaultOpen { get; set; }

    /// <summary>Raised when the open state changes (two-way bind).</summary>
    EventCallback<bool> OpenChanged { get; set; }

    /// <summary>Raised when the open state changes.</summary>
    EventCallback<bool> OnOpenChange { get; set; }

    /// <summary>When true, uses modal focus and scroll lock behavior.</summary>
    bool Modal { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Popover structure content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
