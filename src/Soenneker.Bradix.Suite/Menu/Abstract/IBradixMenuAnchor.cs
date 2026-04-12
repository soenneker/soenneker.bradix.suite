using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixMenuAnchor"/>.</summary>
public interface IBradixMenuAnchor
{
    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }
}
