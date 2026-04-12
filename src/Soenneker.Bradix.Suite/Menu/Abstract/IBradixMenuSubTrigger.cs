using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixMenuSubTrigger"/>.</summary>
public interface IBradixMenuSubTrigger
{
    /// <summary>Gets or sets a value indicating whether the trigger is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Gets or sets the text value used for typeahead.</summary>
    string? TextValue { get; set; }

    /// <summary>Gets the tab stop id assigned to this trigger.</summary>
    string? TabStopId { get; }

    /// <summary>Releases resources used by the trigger.</summary>
    ValueTask DisposeAsync();

    /// <summary>Moves keyboard focus to this trigger without scrolling the page.</summary>
    ValueTask FocusAsync();

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
