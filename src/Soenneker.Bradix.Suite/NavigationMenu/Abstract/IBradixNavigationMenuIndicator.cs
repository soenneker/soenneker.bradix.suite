using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Sliding indicator aligned to the active navigation menu trigger.
/// </summary>
public interface IBradixNavigationMenuIndicator
{
    /// <summary>When true, keeps the indicator mounted while closed.</summary>
    bool ForceMount { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Indicator content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Unregisters indicator interop.</summary>
    ValueTask DisposeAsync();

    /// <summary>Interop handler when indicator size/position updates.</summary>
    Task HandleIndicatorPositionChanged(double size, double offset);
}
