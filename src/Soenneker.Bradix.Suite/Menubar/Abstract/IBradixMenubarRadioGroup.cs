using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixMenubarRadioGroup"/>.</summary>
public interface IBradixMenubarRadioGroup
{
    /// <summary>Gets or sets the element id.</summary>
    string? Id { get; set; }

    /// <summary>Gets or sets the CSS class names.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }

    /// <summary>Gets or sets the controlled value of the selected radio item.</summary>
    string? Value { get; set; }

    /// <summary>Gets or sets the default value when uncontrolled.</summary>
    string? DefaultValue { get; set; }

    /// <summary>Gets or sets a value indicating whether the group is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Gets or sets the callback invoked when the value changes.</summary>
    EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when the value changes.</summary>
    EventCallback<string?> OnValueChange { get; set; }

    /// <summary>Gets or sets additional attributes spread onto the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }
}
