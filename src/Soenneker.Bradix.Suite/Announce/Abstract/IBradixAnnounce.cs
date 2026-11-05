using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Publishes content to the nearest shared announcement provider.</summary>
public interface IBradixAnnounce
{
    /// <summary>Content to announce. Removed from the live region when this component is disposed.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>The live-region priority: polite (default), assertive, or off.</summary>
    string Type { get; set; }
}
