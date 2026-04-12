using System.Collections.Generic;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixContextMenuSeparator"/>.</summary>
public interface IBradixContextMenuSeparator
{
    /// <summary>Gets or sets the element id.</summary>
    string? Id { get; set; }

    /// <summary>Gets or sets the CSS class names.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }

    /// <summary>Gets or sets additional attributes spread onto the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
