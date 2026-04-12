using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixToolbarToggleItem"/>.
/// </summary>
public interface IBradixToolbarToggleItem
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

    /// <summary>Gets or sets the item value.</summary>
    string Value { get; set; }

    /// <summary>Gets or sets whether this item is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Gets the stable id used for roving tabindex.</summary>
    string? TabStopId { get; }

    /// <summary>Releases roving-focus registrations.</summary>
    ValueTask DisposeAsync();

    /// <summary>Moves focus to the toolbar toggle item.</summary>
    ValueTask Focus();

    /// <summary>Called from script when the roving-focus bridge is ready.</summary>
    Task HandleRovingFocusBridgeReady();
}
