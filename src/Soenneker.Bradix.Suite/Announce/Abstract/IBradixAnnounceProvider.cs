using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Hosts persistent polite and assertive live regions for descendant announcements. Place outside inert or aria-hidden containers.</summary>
public interface IBradixAnnounceProvider
{
    /// <summary>The application content that can publish announcements.</summary>
    RenderFragment? ChildContent { get; set; }
}
