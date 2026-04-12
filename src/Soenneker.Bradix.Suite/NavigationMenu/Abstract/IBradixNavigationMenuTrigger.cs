using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Button trigger that toggles its navigation menu item content.
/// </summary>
public interface IBradixNavigationMenuTrigger
{
    /// <summary>When true, the trigger is inactive.</summary>
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

    /// <summary>Unregisters interop and focus proxy wiring.</summary>
    ValueTask DisposeAsync();

    /// <summary>Interop handler when roving focus bridge is ready.</summary>
    Task HandleRovingFocusBridgeReady();
}
