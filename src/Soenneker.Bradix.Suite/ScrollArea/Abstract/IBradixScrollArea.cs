using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixScrollArea"/>.
/// </summary>
public interface IBradixScrollArea : IAsyncDisposable {
/// <summary>Gets or sets the CSS class.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }

    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets additional attributes merged onto the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Gets or sets when scrollbars are shown.</summary>
    ScrollAreaType Type { get; set; }

    /// <summary>Gets or sets the text direction override.</summary>
    string? Dir { get; set; }

    /// <summary>Gets or sets the delay before scrollbars hide after activity, in milliseconds.</summary>
    int ScrollHideDelay { get; set; }


    /// <summary>
    /// Called from script when hover state over the root changes.
    /// </summary>
    /// <param name="hovering">Whether hovering.</param>
    /// <returns>A task that completes when the handle hover changed operation is complete.</returns>
    Task HandleHoverChanged(bool hovering);

    /// <summary>
    /// Called from script when viewport scroll metrics change.
    /// </summary>
    /// <param name="scrollLeft">Scroll Left for the handle viewport metrics changed operation.</param>
    /// <param name="scrollTop">Scroll Top for the handle viewport metrics changed operation.</param>
    /// <param name="scrollWidth">Scroll Width for the handle viewport metrics changed operation.</param>
    /// <param name="scrollHeight">Scroll Height for the handle viewport metrics changed operation.</param>
    /// <param name="viewportWidth">Viewport Width for the handle viewport metrics changed operation.</param>
    /// <param name="viewportHeight">Viewport Height for the handle viewport metrics changed operation.</param>
    /// <returns>A task that completes when the handle viewport metrics changed operation is complete.</returns>
    Task HandleViewportMetricsChanged(double scrollLeft, double scrollTop, double scrollWidth, double scrollHeight, double viewportWidth, double viewportHeight);

    /// <summary>
    /// Called from script when scrollbar element metrics change.
    /// </summary>
    /// <param name="orientation">Layout orientation to apply.</param>
    /// <param name="clientWidth">client Width used to communicate with the external service.</param>
    /// <param name="clientHeight">client Height used to communicate with the external service.</param>
    /// <param name="paddingStart">Padding Start for the handle scrollbar metrics changed operation.</param>
    /// <param name="paddingEnd">Padding End for the handle scrollbar metrics changed operation.</param>
    /// <returns>A task that completes when the handle scrollbar metrics changed operation is complete.</returns>
    Task HandleScrollbarMetricsChanged(string orientation, double clientWidth, double clientHeight, double paddingStart, double paddingEnd);
}
