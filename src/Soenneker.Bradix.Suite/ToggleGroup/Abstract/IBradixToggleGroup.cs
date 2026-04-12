using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixToggleGroup"/>.
/// </summary>
public interface IBradixToggleGroup
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

    /// <summary>Gets or sets the selection mode.</summary>
    BradixSelectionMode Type { get; set; }

    /// <summary>Gets or sets whether the entire group is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Gets or sets whether roving tabindex is enabled.</summary>
    bool RovingFocus { get; set; }

    /// <summary>Gets or sets whether keyboard navigation wraps at the ends.</summary>
    bool Loop { get; set; }

    /// <summary>Gets or sets the orientation.</summary>
    BradixOrientation? Orientation { get; set; }

    /// <summary>Gets or sets the text direction override.</summary>
    string? Dir { get; set; }

    /// <summary>Gets or sets the controlled single value.</summary>
    string? Value { get; set; }

    /// <summary>Gets or sets the initial single value when uncontrolled.</summary>
    string? DefaultValue { get; set; }

    /// <summary>Gets or sets the callback invoked when the single value changes (two-way bind).</summary>
    EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when the single value changes.</summary>
    EventCallback<string?> OnValueChange { get; set; }

    /// <summary>Gets or sets the controlled multiple values.</summary>
    IReadOnlyCollection<string>? Values { get; set; }

    /// <summary>Gets or sets the initial multiple values when uncontrolled.</summary>
    IEnumerable<string>? DefaultValues { get; set; }

    /// <summary>Gets or sets the callback invoked when multiple values change (two-way bind).</summary>
    EventCallback<IReadOnlyCollection<string>> ValuesChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when multiple values change.</summary>
    EventCallback<IReadOnlyCollection<string>> OnValuesChange { get; set; }
}
