using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixToast"/>.
/// </summary>
public interface IBradixToast
{
    /// <summary>Gets or sets the element identifier.</summary>
    string? Id { get; set; }

    /// <summary>Gets or sets the CSS class.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }

    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets additional attributes merged onto the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Gets or sets the controlled open state.</summary>
    bool? Open { get; set; }

    /// <summary>Gets or sets the initial open state when uncontrolled.</summary>
    bool DefaultOpen { get; set; }

    /// <summary>Gets or sets the callback invoked when open state changes (two-way bind).</summary>
    EventCallback<bool> OpenChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when open state changes.</summary>
    EventCallback<bool> OnOpenChange { get; set; }

    /// <summary>Gets or sets whether the toast stays mounted when closed.</summary>
    bool ForceMount { get; set; }

    /// <summary>Gets or sets the toast type (<c>foreground</c> or <c>background</c>).</summary>
    string Type { get; set; }

    /// <summary>Gets or sets the auto-close duration in milliseconds.</summary>
    int? Duration { get; set; }

    /// <summary>Gets or sets the callback invoked when the close timer pauses.</summary>
    EventCallback OnPause { get; set; }

    /// <summary>Gets or sets the callback invoked when the close timer resumes.</summary>
    EventCallback OnResume { get; set; }

    /// <summary>Gets or sets the callback invoked when escape is pressed.</summary>
    EventCallback OnEscapeKeyDown { get; set; }

    /// <summary>Gets or sets the detailed callback invoked when escape is pressed.</summary>
    EventCallback<BradixEscapeKeyDownEventArgs> OnEscapeKeyDownDetailed { get; set; }

    /// <summary>Gets or sets the callback invoked when a swipe gesture starts.</summary>
    EventCallback<BradixToastSwipeEventArgs> OnSwipeStart { get; set; }

    /// <summary>Gets or sets the callback invoked during a swipe gesture.</summary>
    EventCallback<BradixToastSwipeEventArgs> OnSwipeMove { get; set; }

    /// <summary>Gets or sets the callback invoked when a swipe is cancelled.</summary>
    EventCallback<BradixToastSwipeEventArgs> OnSwipeCancel { get; set; }

    /// <summary>Gets or sets the callback invoked when a swipe completes.</summary>
    EventCallback<BradixToastSwipeEventArgs> OnSwipeEnd { get; set; }

    /// <summary>Releases toast registrations and script hooks.</summary>
    ValueTask DisposeAsync();

    /// <summary>Called from script for delegated escape key handling.</summary>
    Task HandleDelegatedKeyDownAsync(BradixDelegatedKeyboardEvent args);
}
