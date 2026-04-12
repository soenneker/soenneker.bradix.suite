using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixSelectItem"/>.
/// </summary>
public interface IBradixSelectItem
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

    /// <summary>Gets or sets the option value.</summary>
    string Value { get; set; }

    /// <summary>Gets or sets whether the item is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Gets or sets the text value used for typeahead matching.</summary>
    string? TextValue { get; set; }

    /// <summary>Releases item registration and script hooks.</summary>
    ValueTask DisposeAsync();
}
