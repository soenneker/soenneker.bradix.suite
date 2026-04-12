using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixDialog"/>.
/// </summary>
public interface IBradixDialog
{
    /// <summary>
    /// Gets or sets the controlled open state.
    /// </summary>
    bool? Open { get; set; }

    /// <summary>
    /// Gets or sets the initial open state when uncontrolled.
    /// </summary>
    bool DefaultOpen { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the controlled open state changes.
    /// </summary>
    EventCallback<bool> OpenChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the open state changes.
    /// </summary>
    EventCallback<bool> OnOpenChange { get; set; }

    /// <summary>
    /// Gets or sets whether the dialog behaves as a modal.
    /// </summary>
    bool Modal { get; set; }
}
