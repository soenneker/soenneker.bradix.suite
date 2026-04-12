using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixDialogPortal"/>.
/// </summary>
public interface IBradixDialogPortal
{
    /// <summary>
    /// Gets or sets the CSS selector for the portal container element.
    /// </summary>
    string? ContainerSelector { get; set; }

    /// <summary>
    /// Gets or sets the portal container element reference.
    /// </summary>
    ElementReference Container { get; set; }

    /// <summary>
    /// Gets or sets whether portal children stay mounted while inactive.
    /// </summary>
    bool ForceMount { get; set; }

    /// <summary>
    /// Gets or sets the child content rendered into the portal.
    /// </summary>
    RenderFragment? ChildContent { get; set; }
}
