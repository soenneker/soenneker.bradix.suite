using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Sliding indicator aligned to the active navigation menu trigger.
/// </summary>
public interface IBradixNavigationMenuIndicator : IAsyncDisposable {
    /// <summary>When true, keeps the indicator mounted while closed.</summary>
    bool ForceMount { get; set; }
/// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Indicator content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }


    /// <summary>
    /// Interop handler when indicator size/position updates.
    /// </summary>
    /// <param name="size">Size for the handle indicator position changed operation.</param>
    /// <param name="offset">Zero-based offset from the start of the input.</param>
    /// <returns>A task that completes when the handle indicator position changed operation is complete.</returns>
    Task HandleIndicatorPositionChanged(double size, double offset);
}
