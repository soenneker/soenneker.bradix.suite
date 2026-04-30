using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Optional custom anchor region for a popover.
/// </summary>
public interface IBradixPopoverAnchor : IAsyncDisposable {
/// <summary>Anchor content.</summary>
    RenderFragment? ChildContent { get; set; }
}
