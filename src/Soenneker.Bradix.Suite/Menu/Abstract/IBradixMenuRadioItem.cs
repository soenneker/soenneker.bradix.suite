using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixMenuRadioItem"/>.</summary>
public interface IBradixMenuRadioItem
{
    /// <summary>Gets or sets the value for this radio item.</summary>
    string Value { get; set; }

    /// <summary>Gets or sets a value indicating whether the item is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Gets or sets the text value used for typeahead.</summary>
    string? TextValue { get; set; }

    /// <summary>Gets or sets a value indicating whether the menu closes after selection.</summary>
    bool CloseOnSelect { get; set; }

    /// <summary>Gets or sets the callback invoked when the item is selected.</summary>
    EventCallback OnSelect { get; set; }

    /// <summary>Gets or sets the callback invoked when the item is selected, with event details.</summary>
    EventCallback<BradixMenuItemSelectEventArgs> OnSelectDetailed { get; set; }

    /// <summary>Gets or sets the element id.</summary>
    string? Id { get; set; }

    /// <summary>Gets or sets the CSS class names.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }

    /// <summary>Gets or sets additional attributes spread onto the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }
}
