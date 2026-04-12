using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixRadioGroup"/>.
/// </summary>
public interface IBradixRadioGroup
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

    /// <summary>Gets or sets the <c>name</c> attribute shared by native inputs in the group.</summary>
    string? Name { get; set; }

    /// <summary>Gets or sets whether the group is required.</summary>
    bool Required { get; set; }

    /// <summary>Gets or sets whether the entire group is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Gets or sets the controlled selected value.</summary>
    string? Value { get; set; }

    /// <summary>Gets or sets the initial value when uncontrolled.</summary>
    string? DefaultValue { get; set; }

    /// <summary>Gets or sets the callback invoked when the selected value changes (two-way bind).</summary>
    EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when the selected value changes.</summary>
    EventCallback<string?> OnValueChange { get; set; }

    /// <summary>Gets or sets the orientation.</summary>
    BradixOrientation? Orientation { get; set; }

    /// <summary>Gets or sets the text direction override.</summary>
    string? Dir { get; set; }

    /// <summary>Gets or sets whether keyboard navigation wraps at the ends of the group.</summary>
    bool Loop { get; set; }
}
