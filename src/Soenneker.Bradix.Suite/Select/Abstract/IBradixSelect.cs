using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixSelect"/>.
/// </summary>
public interface IBradixSelect
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

    /// <summary>Gets or sets the controlled open state.</summary>
    bool? Open { get; set; }

    /// <summary>Gets or sets the initial open state when uncontrolled.</summary>
    bool DefaultOpen { get; set; }

    /// <summary>Gets or sets the callback invoked when open state changes (two-way bind).</summary>
    EventCallback<bool> OpenChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when open state changes.</summary>
    EventCallback<bool> OnOpenChange { get; set; }

    /// <summary>Gets or sets the controlled selected value.</summary>
    string? Value { get; set; }

    /// <summary>Gets or sets the initial value when uncontrolled.</summary>
    string? DefaultValue { get; set; }

    /// <summary>Gets or sets the callback invoked when the value changes (two-way bind).</summary>
    EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when the value changes.</summary>
    EventCallback<string?> OnValueChange { get; set; }

    /// <summary>Gets or sets the text direction override.</summary>
    string? Dir { get; set; }

    /// <summary>Gets or sets the native select <c>name</c> for form integration.</summary>
    string? Name { get; set; }

    /// <summary>Gets or sets the native <c>autocomplete</c> hint.</summary>
    string? AutoComplete { get; set; }

    /// <summary>Gets or sets whether the select is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Gets or sets whether a value is required.</summary>
    bool Required { get; set; }

    /// <summary>Gets or sets the <c>form</c> attribute for detached native inputs.</summary>
    string? Form { get; set; }
}
