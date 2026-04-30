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
/// <summary>Tooltip structure content.</summary>
    RenderFragment? ChildContent { get; set; }
}
