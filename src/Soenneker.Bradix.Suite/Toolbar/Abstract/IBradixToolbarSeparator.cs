using System.Collections.Generic;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixToolbarSeparator"/>.
/// </summary>
public interface IBradixToolbarSeparator
{
    /// <summary>Gets or sets the element identifier.</summary>
    string? Id { get; set; }

    /// <summary>Gets or sets the CSS class.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }

    /// <summary>Gets or sets additional attributes forwarded to the underlying separator.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
