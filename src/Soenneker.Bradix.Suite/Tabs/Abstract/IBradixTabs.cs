using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixTabs"/>.
/// </summary>
public interface IBradixTabs
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

    /// <summary>Gets or sets the controlled active tab value.</summary>
    string? Value { get; set; }

    /// <summary>Gets or sets the initial active tab when uncontrolled.</summary>
    string? DefaultValue { get; set; }

    /// <summary>Gets or sets the callback invoked when the active tab changes (two-way bind).</summary>
    EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when the active tab changes.</summary>
    EventCallback<string?> OnValueChange { get; set; }

    /// <summary>Gets or sets the tab list orientation.</summary>
    string Orientation { get; set; }

    /// <summary>Gets or sets the text direction override.</summary>
    string? Dir { get; set; }

    /// <summary>Gets or sets how tabs are activated (<c>automatic</c> or <c>manual</c>).</summary>
    string ActivationMode { get; set; }
}
