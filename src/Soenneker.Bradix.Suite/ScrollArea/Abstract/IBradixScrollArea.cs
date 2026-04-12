using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixScrollArea"/>.
/// </summary>
public interface IBradixScrollArea : IAsyncDisposable {
    /// <summary>Gets or sets the element identifier.</summary>
    string? Id { get; set; }

    /// <summary>Gets or sets the CSS class.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }

    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets additional attributes merged onto the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Gets or sets when scrollbars are shown.</summary>
    BradixScrollAreaType Type { get; set; }

    /// <summary>Gets or sets the text direction override.</summary>
    string? Dir { get; set; }

    /// <summary>Gets or sets the delay before scrollbars hide after activity, in milliseconds.</summary>
    int ScrollHideDelay { get; set; }


    /// <summary>Called from script when hover state over the root changes.</summary>
    Task HandleHoverChanged(bool hovering);

    /// <summary>Called from script when viewport scroll metrics change.</summary>
    Task HandleViewportMetricsChanged(double scrollLeft, double scrollTop, double scrollWidth, double scrollHeight, double viewportWidth, double viewportHeight);

    /// <summary>Called from script when scrollbar element metrics change.</summary>
    Task HandleScrollbarMetricsChanged(string orientation, double clientWidth, double clientHeight, double paddingStart, double paddingEnd);
}
