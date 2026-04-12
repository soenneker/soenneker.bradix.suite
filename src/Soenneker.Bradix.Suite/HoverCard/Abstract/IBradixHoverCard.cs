using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Root primitive for a hover-activated floating card.
/// </summary>
public interface IBradixHoverCard
{
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

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Child structure (trigger, content, portal).</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Cancels pending open/close delays.</summary>
    ValueTask DisposeAsync();
}
