using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixVisuallyHidden"/>.
/// </summary>
public interface IBradixVisuallyHidden
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

    /// <summary>Gets or sets whether to merge props onto a child element instead of rendering a wrapper.</summary>
    bool AsChild { get; set; }

    /// <summary>Gets or sets the child element tag name when <see cref="AsChild"/> is <c>true</c>.</summary>
    string? ChildElementName { get; set; }

    /// <summary>Gets or sets attributes forwarded to the child element when <see cref="AsChild"/> is <c>true</c>.</summary>
    IReadOnlyDictionary<string, object>? ChildAttributes { get; set; }
}
