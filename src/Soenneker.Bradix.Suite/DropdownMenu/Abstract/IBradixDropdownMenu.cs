using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixDropdownMenu"/>.</summary>
public interface IBradixDropdownMenu
{
    /// <summary>Gets or sets the controlled open state.</summary>
    bool? Open { get; set; }

    /// <summary>Gets or sets the default open state when uncontrolled.</summary>
    bool DefaultOpen { get; set; }

    /// <summary>Gets or sets the callback invoked when the open state changes.</summary>
    EventCallback<bool> OpenChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when the open state changes.</summary>
    EventCallback<bool> OnOpenChange { get; set; }

    /// <summary>Gets or sets the text direction (e.g. <c>ltr</c> or <c>rtl</c>).</summary>
    string? Dir { get; set; }

    /// <summary>Gets or sets a value indicating whether the menu uses modal behavior.</summary>
    bool Modal { get; set; }

    /// <summary>Gets the base id used for stable trigger and content ids.</summary>
    string? BaseId { get; }

    /// <summary>Gets or sets the root content of the dropdown menu.</summary>
    RenderFragment? ChildContent { get; set; }
}
