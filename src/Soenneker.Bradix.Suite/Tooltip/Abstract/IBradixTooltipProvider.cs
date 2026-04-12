using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Coordinates delay skipping and single-open behavior across nested tooltips.
/// </summary>
public interface IBradixTooltipProvider
{
    /// <summary>Default open delay in milliseconds.</summary>
    int DelayDuration { get; set; }

    /// <summary>Duration to skip open delay after closing a tooltip.</summary>
    int SkipDelayDuration { get; set; }

    /// <summary>When true, content is not hover-pinned between trigger and surface.</summary>
    bool DisableHoverableContent { get; set; }

    /// <summary>Provider subtree containing tooltips.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Releases skip-delay timers.</summary>
    ValueTask DisposeAsync();
}
