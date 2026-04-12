using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Tooltip root coordinating open state, delays, and provider coordination.
/// </summary>
public interface IBradixTooltip : IAsyncDisposable {
    /// <summary>Controlled open state; null for uncontrolled usage.</summary>
    bool? Open { get; set; }

    /// <summary>Initial open state for uncontrolled usage.</summary>
    bool DefaultOpen { get; set; }

    /// <summary>Raised when the open state changes (two-way bind).</summary>
    EventCallback<bool> OpenChanged { get; set; }

    /// <summary>Raised when the open state changes.</summary>
    EventCallback<bool> OnOpenChange { get; set; }

    /// <summary>Override for open delay in milliseconds.</summary>
    int? DelayDuration { get; set; }

    /// <summary>Override for disabling hoverable content behavior.</summary>
    bool? DisableHoverableContent { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Tooltip structure content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

}
