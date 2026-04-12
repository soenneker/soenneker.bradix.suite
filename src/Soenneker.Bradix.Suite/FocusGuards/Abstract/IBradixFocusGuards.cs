using System;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Registers Radix-style focus guards for modal/focus-trap scenarios.
/// </summary>
public interface IBradixFocusGuards : IAsyncDisposable {
    /// <summary>Child content rendered inside the focus guard scope.</summary>
    RenderFragment? ChildContent { get; set; }

}
