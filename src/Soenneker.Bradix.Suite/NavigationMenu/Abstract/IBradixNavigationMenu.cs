using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Root navigation menu coordinating triggers, content, and optional viewport mode.
/// </summary>
public interface IBradixNavigationMenu
{
    /// <summary>Controlled value for the open item.</summary>
    string? Value { get; set; }

    /// <summary>Initial value for uncontrolled usage.</summary>
    string? DefaultValue { get; set; }

    /// <summary>Raised when the value changes (two-way bind).</summary>
    EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Raised when the value changes.</summary>
    EventCallback<string?> OnValueChange { get; set; }

    /// <summary><c>horizontal</c> or <c>vertical</c> layout.</summary>
    string Orientation { get; set; }

    /// <summary>Explicit text direction override.</summary>
    string? Dir { get; set; }

    /// <summary>Delay before opening on pointer hover (root menus).</summary>
    int DelayDuration { get; set; }

    /// <summary>Delay before hover open is allowed again after closing.</summary>
    int SkipDelayDuration { get; set; }

    /// <summary>When true, renders a nav landmark root; nested menus use false.</summary>
    bool IsRootMenu { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Menu structure content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
