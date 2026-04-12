using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixContextMenuSub"/>.</summary>
public interface IBradixContextMenuSub
{
    /// <summary>Gets or sets the controlled open state.</summary>
    bool? Open { get; set; }

    /// <summary>Gets or sets the default open state when uncontrolled.</summary>
    bool DefaultOpen { get; set; }

    /// <summary>Gets or sets the callback invoked when the open state changes.</summary>
    EventCallback<bool> OpenChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when the open state changes.</summary>
    EventCallback<bool> OnOpenChange { get; set; }

    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }
}
