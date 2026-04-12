using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Viewport host that animates between multiple navigation menu content panels.
/// </summary>
public interface IBradixNavigationMenuViewport
{
    /// <summary>When true, keeps the viewport mounted while closed.</summary>
    bool ForceMount { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Viewport content (usually none; hosts register dynamically).</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Unregisters viewport sizing interop.</summary>
    ValueTask DisposeAsync();

    /// <summary>Interop handler when viewport dimensions change.</summary>
    Task HandleViewportSizeChanged(double width, double height);
}
