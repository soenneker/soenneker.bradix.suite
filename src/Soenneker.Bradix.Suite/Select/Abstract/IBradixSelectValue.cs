using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixSelectValue"/>.
/// </summary>
public interface IBradixSelectValue
{
/// <summary>Gets or sets the CSS class.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }

    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets additional attributes merged onto the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Gets or sets the placeholder shown when no value is selected.</summary>
    RenderFragment? Placeholder { get; set; }

    /// <summary>Renders the selected value as rich Blazor content. ChildContent takes precedence; Placeholder is used when no value is selected. TextValue still supplies typeahead and native option text.</summary>
    RenderFragment<string?>? ValueTemplate { get; set; }
}
