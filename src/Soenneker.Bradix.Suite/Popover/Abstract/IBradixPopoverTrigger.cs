using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Button trigger that toggles the popover open state.
/// </summary>
public interface IBradixPopoverTrigger
{
    /// <summary>When true, the trigger ignores interactions.</summary>
    bool Disabled { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Trigger content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Unregisters delegated interaction interop.</summary>
    ValueTask DisposeAsync();

    /// <summary>Interop handler when delegated interaction is ready.</summary>
    Task HandleDelegatedInteractionReady();

    /// <summary>Interop handler for delegated click events.</summary>
    Task HandleDelegatedClick(BradixDelegatedMouseEvent args);
}
