using System.Collections.Generic;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixSelectArrow"/>.
/// </summary>
public interface IBradixSelectArrow
{
    /// <summary>Gets or sets the arrow width.</summary>
    double Width { get; set; }

    /// <summary>Gets or sets the arrow height.</summary>
    double Height { get; set; }

    /// <summary>Gets or sets additional attributes forwarded to the underlying popper arrow.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Gets or sets the CSS class.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }
}
