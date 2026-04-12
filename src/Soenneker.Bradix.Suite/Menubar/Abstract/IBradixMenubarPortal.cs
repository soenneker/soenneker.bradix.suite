using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixMenubarPortal"/>.</summary>
public interface IBradixMenubarPortal
{
    /// <summary>Gets or sets the CSS selector of the portal container element.</summary>
    string? ContainerSelector { get; set; }

    /// <summary>Gets or sets the portal container element reference.</summary>
    ElementReference Container { get; set; }

    /// <summary>Gets or sets a value indicating whether portal content is mounted while closed for measurement.</summary>
    bool ForceMount { get; set; }

    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }
}
