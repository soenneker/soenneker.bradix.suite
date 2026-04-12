using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixToolbar"/>.
/// </summary>
public interface IBradixToolbar
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

    /// <summary>Gets or sets the toolbar orientation.</summary>
    string Orientation { get; set; }

    /// <summary>Gets or sets whether keyboard navigation wraps at the ends.</summary>
    bool Loop { get; set; }

    /// <summary>Gets or sets the text direction override.</summary>
    string? Dir { get; set; }
}
