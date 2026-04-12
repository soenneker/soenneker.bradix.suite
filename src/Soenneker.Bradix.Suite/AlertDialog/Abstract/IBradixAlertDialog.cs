using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAlertDialog"/>.
/// </summary>
public interface IBradixAlertDialog
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
    /// Gets or sets the child content of the alert dialog root.
    /// </summary>
    RenderFragment? ChildContent { get; set; }
}
