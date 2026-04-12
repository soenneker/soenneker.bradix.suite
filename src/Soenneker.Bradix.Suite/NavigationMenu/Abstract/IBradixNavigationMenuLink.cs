using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Link inside navigation menu content with roving tabindex support.
/// </summary>
public interface IBradixNavigationMenuLink
{
    /// <summary>When true, marks the link as the active page.</summary>
    bool Active { get; set; }

    /// <summary>Raised when the link is activated.</summary>
    EventCallback OnSelect { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Link content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Unregisters roving focus interop and focus group membership.</summary>
    ValueTask DisposeAsync();

    /// <summary>Interop handler when roving focus bridge is ready.</summary>
    Task HandleRovingFocusBridgeReady();
}
