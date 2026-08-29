using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Viewport host that animates between multiple navigation menu content panels.
/// </summary>
public interface IBradixNavigationMenuViewport : IAsyncDisposable {
    /// <summary>When true, keeps the viewport mounted while closed.</summary>
    bool ForceMount { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }
/// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }


    /// <summary>
    /// Interop handler when viewport dimensions change.
    /// </summary>
    /// <param name="width">Width to apply.</param>
    /// <param name="height">Height to apply.</param>
    /// <returns>A task that completes when the handle viewport size changed operation is complete.</returns>
    Task HandleViewportSizeChanged(double width, double height);
}
