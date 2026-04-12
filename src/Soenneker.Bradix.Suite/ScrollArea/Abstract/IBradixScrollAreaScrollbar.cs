using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixScrollAreaScrollbar"/>.
/// </summary>
public interface IBradixScrollAreaScrollbar
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

    /// <summary>Gets or sets the scrollbar orientation (<c>horizontal</c> or <c>vertical</c>).</summary>
    string Orientation { get; set; }

    /// <summary>Gets or sets whether the scrollbar stays mounted when not visible.</summary>
    bool ForceMount { get; set; }

    /// <summary>Releases resources used by the scrollbar registration.</summary>
    ValueTask DisposeAsync();
}
