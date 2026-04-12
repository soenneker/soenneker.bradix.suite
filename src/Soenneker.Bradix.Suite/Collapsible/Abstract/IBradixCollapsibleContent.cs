using System;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixCollapsibleContent"/>.
/// </summary>
public interface IBradixCollapsibleContent : IAsyncDisposable {
    /// <summary>
    /// Gets or sets whether the content stays mounted while closed.
    /// </summary>
    bool ForceMount { get; set; }

    /// <summary>
    /// Gets or sets the ARIA role applied to the content region.
    /// </summary>
    string? Role { get; set; }

    /// <summary>
    /// Gets or sets the id of the element that labels this region.
    /// </summary>
    string? AriaLabelledBy { get; set; }

    /// <summary>
    /// Gets or sets the id of the element that describes this region.
    /// </summary>
    string? AriaDescribedBy { get; set; }

}
