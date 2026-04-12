using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Renders children into a portal target in the DOM.
/// </summary>
public interface IBradixPortal : IAsyncDisposable {
    /// <summary>CSS selector for the portal container element.</summary>
    string? ContainerSelector { get; set; }

    /// <summary>Explicit container element reference.</summary>
    ElementReference Container { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Child content portaled to the target.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

}
