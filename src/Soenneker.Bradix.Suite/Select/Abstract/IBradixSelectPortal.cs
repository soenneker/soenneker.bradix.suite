using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixSelectPortal"/>.
/// </summary>
public interface IBradixSelectPortal
{
    /// <summary>Gets or sets a CSS selector for the portal container.</summary>
    string? ContainerSelector { get; set; }

    /// <summary>Gets or sets the portal container element reference.</summary>
    ElementReference Container { get; set; }

    /// <summary>Gets or sets the child content rendered into the portal.</summary>
    RenderFragment? ChildContent { get; set; }
}
