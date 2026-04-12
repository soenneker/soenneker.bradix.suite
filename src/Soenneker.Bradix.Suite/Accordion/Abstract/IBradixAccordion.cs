using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Describes the public API of <see cref="BradixAccordion"/>.
/// </summary>
public interface IBradixAccordion
{
    /// <summary>
    /// Gets or sets the accordion behavior mode (<c>single</c> or <c>multiple</c>).
    /// </summary>
    string Type { get; set; }

    /// <summary>
    /// Gets or sets whether the entire accordion is disabled.
    /// </summary>
    bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the layout orientation (<c>vertical</c> or <c>horizontal</c>).
    /// </summary>
    string Orientation { get; set; }

    /// <summary>
    /// Gets or sets the text direction override for the accordion.
    /// </summary>
    string? Dir { get; set; }

    /// <summary>
    /// Gets or sets whether an open item can be collapsed in single-selection mode.
    /// </summary>
    bool Collapsible { get; set; }

    /// <summary>
    /// Gets or sets the controlled open item value when <see cref="Type"/> is <c>single</c>.
    /// </summary>
    string? Value { get; set; }

    /// <summary>
    /// Gets or sets the initial open item value for uncontrolled single mode.
    /// </summary>
    string? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the controlled single value changes.
    /// </summary>
    EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the open item changes in single mode.
    /// </summary>
    EventCallback<string?> OnValueChange { get; set; }

    /// <summary>
    /// Gets or sets the controlled open item values when <see cref="Type"/> is <c>multiple</c>.
    /// </summary>
    IReadOnlyCollection<string>? Values { get; set; }

    /// <summary>
    /// Gets or sets the initial open item values for uncontrolled multiple mode.
    /// </summary>
    IEnumerable<string>? DefaultValues { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the controlled multiple values change.
    /// </summary>
    EventCallback<IReadOnlyCollection<string>> ValuesChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the open items change in multiple mode.
    /// </summary>
    EventCallback<IReadOnlyCollection<string>> OnValuesChange { get; set; }
}
