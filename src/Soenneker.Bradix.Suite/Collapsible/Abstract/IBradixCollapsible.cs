using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixCollapsible"/>.
/// </summary>
public interface IBradixCollapsible
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
    /// Gets or sets whether interaction is disabled.
    /// </summary>
    bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the controlled open state changes.
    /// </summary>
    EventCallback<bool> OpenChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the open state changes.
    /// </summary>
    EventCallback<bool> OnOpenChange { get; set; }
}
