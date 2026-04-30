using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Popper root providing a cascading anchor/content context.
/// </summary>
public interface IBradixPopper
{
/// <summary>Anchor and content subtree.</summary>
    RenderFragment? ChildContent { get; set; }
}
