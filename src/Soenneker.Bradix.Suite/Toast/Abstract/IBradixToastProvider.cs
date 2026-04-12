using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixToastProvider"/>.
/// </summary>
public interface IBradixToastProvider
{
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

    /// <summary>Gets or sets the accessible label prefix used for toast announcements.</summary>
    string Label { get; set; }

    /// <summary>Gets or sets the default auto-close duration in milliseconds.</summary>
    int Duration { get; set; }

    /// <summary>Gets or sets the swipe direction.</summary>
    BradixSwipeDirection SwipeDirection { get; set; }

    /// <summary>Gets or sets the swipe distance threshold.</summary>
    double SwipeThreshold { get; set; }
}
