using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Arrow element for hover card positioning.
/// </summary>
public interface IBradixHoverCardArrow
{
    /// <summary>Arrow wrapper width in pixels.</summary>
    double Width { get; set; }

    /// <summary>Arrow wrapper height in pixels.</summary>
    double Height { get; set; }

    /// <summary>CSS class names for the arrow.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the arrow.</summary>
    string? Style { get; set; }

    /// <summary>Additional unmatched attributes.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Arrow content.</summary>
    RenderFragment? ChildContent { get; set; }
}
