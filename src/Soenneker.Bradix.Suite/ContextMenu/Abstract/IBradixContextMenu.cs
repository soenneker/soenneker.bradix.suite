using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixContextMenu"/>.</summary>
public interface IBradixContextMenu
{
    /// <summary>Gets or sets the callback invoked when the open state changes.</summary>
    EventCallback<bool> OnOpenChange { get; set; }

    /// <summary>Gets or sets the text direction (e.g. <c>ltr</c> or <c>rtl</c>).</summary>
    string? Dir { get; set; }

    /// <summary>Gets or sets a value indicating whether the menu uses modal behavior.</summary>
    bool Modal { get; set; }

    /// <summary>Gets or sets the root content of the context menu.</summary>
    RenderFragment? ChildContent { get; set; }
}
