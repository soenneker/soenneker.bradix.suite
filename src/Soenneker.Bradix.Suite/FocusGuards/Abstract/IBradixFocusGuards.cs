using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Registers Radix-style focus guards for modal/focus-trap scenarios.
/// </summary>
public interface IBradixFocusGuards
{
    /// <summary>Child content rendered inside the focus guard scope.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Releases focus guard registration.</summary>
    ValueTask DisposeAsync();
}
