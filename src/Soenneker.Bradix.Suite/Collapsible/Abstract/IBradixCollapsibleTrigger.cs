using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixCollapsibleTrigger"/>.
/// </summary>
public interface IBradixCollapsibleTrigger {
    /// <summary>
    /// Gets or sets the callback invoked when the trigger is clicked.
    /// </summary>
    EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when a key is pressed on the trigger.
    /// </summary>
    EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

    /// <summary>
    /// Gets or sets the <c>aria-disabled</c> attribute value for the trigger.
    /// </summary>
    string? AriaDisabled { get; set; }

    /// <summary>
    /// Gets or sets whether this trigger is disabled independently of the root collapsible.
    /// </summary>
    bool Disabled { get; set; }
}
