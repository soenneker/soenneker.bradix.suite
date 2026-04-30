using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Root primitive for a hover-activated floating card.
/// </summary>
public interface IBradixHoverCard : IAsyncDisposable {
    /// <summary>Controlled open state; null for uncontrolled usage.</summary>
    bool? Open { get; set; }

    /// <summary>Initial open state for uncontrolled usage.</summary>
    bool DefaultOpen { get; set; }

    /// <summary>Raised when the open state changes (two-way bind).</summary>
    EventCallback<bool> OpenChanged { get; set; }

    /// <summary>Raised when the open state changes.</summary>
    EventCallback<bool> OnOpenChange { get; set; }

    /// <summary>Delay in milliseconds before opening on hover.</summary>
    int OpenDelay { get; set; }

    /// <summary>Delay in milliseconds before closing after leaving hover.</summary>
    int CloseDelay { get; set; }
/// <summary>Child structure (trigger, content, portal).</summary>
    RenderFragment? ChildContent { get; set; }
}
