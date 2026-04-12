using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Portals hover card content with optional force-mount cascading.
/// </summary>
public interface IBradixHoverCardPortal
{
    /// <summary>CSS selector for the portal container element.</summary>
    string? ContainerSelector { get; set; }

    /// <summary>Explicit container element reference.</summary>
    ElementReference Container { get; set; }

    /// <summary>When true, cascades force-mount to hosted content.</summary>
    bool ForceMount { get; set; }

    /// <summary>Child content portaled to the target.</summary>
    RenderFragment? ChildContent { get; set; }
}
