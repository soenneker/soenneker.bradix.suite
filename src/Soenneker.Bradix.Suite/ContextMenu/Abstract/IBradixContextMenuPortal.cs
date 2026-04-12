using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixContextMenuPortal"/>.</summary>
public interface IBradixContextMenuPortal
{
    /// <summary>Gets or sets the CSS selector for the portal container.</summary>
    string? ContainerSelector { get; set; }

    /// <summary>Gets or sets the element reference for the portal container.</summary>
    ElementReference Container { get; set; }

    /// <summary>Gets or sets a value indicating whether the portal content is force-mounted.</summary>
    bool ForceMount { get; set; }

    /// <summary>Gets or sets the child content rendered into the portal.</summary>
    RenderFragment? ChildContent { get; set; }
}
