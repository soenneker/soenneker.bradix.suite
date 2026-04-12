using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixRemoveScroll"/>.
/// </summary>
public interface IBradixRemoveScroll
{
    /// <summary>Gets or sets whether pinch-zoom should remain enabled while scroll is locked.</summary>
    bool AllowPinchZoom { get; set; }

    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Releases resources and restores scrolling.</summary>
    ValueTask DisposeAsync();
}
