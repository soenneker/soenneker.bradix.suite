using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixToastPortal"/>.
/// </summary>
public interface IBradixToastPortal
{
/// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }
/// <summary>Gets or sets whether portal behavior is disabled for descendants.</summary>
    bool Disabled { get; set; }
}
