using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixMenu"/>.</summary>
public interface IBradixMenu
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

    /// <summary>Gets the base id used for stable content and trigger ids.</summary>
    string? BaseId { get; }

    /// <summary>Gets or sets the root content of the menu.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets the element id.</summary>
    string? Id { get; set; }

    /// <summary>Gets or sets the CSS class names.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }

    /// <summary>Gets or sets additional attributes spread onto the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
