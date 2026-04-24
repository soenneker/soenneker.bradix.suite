using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixMenuArrow"/>.</summary>
public interface IBradixMenuArrow
{
    /// <summary>Arrow wrapper width in pixels.</summary>
    double Width { get; set; }

    /// <summary>Arrow wrapper height in pixels.</summary>
    double Height { get; set; }

    /// <summary>Gets or sets the CSS class names.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }

    /// <summary>Gets or sets additional attributes spread onto the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }
}
