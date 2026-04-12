using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixToolbarLink"/>.
/// </summary>
public interface IBradixToolbarLink
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

    /// <summary>Gets or sets the link URL.</summary>
    string? Href { get; set; }

    /// <summary>Gets or sets the link target.</summary>
    string? Target { get; set; }

    /// <summary>Gets or sets the link rel attribute.</summary>
    string? Rel { get; set; }

    /// <summary>Gets or sets the callback invoked on click.</summary>
    EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>Gets the stable id used for roving tabindex.</summary>
    string? TabStopId { get; }

    /// <summary>Releases roving-focus registrations.</summary>
    ValueTask DisposeAsync();

    /// <summary>Moves focus to the toolbar link.</summary>
    ValueTask FocusAsync();

    /// <summary>Called from script when the roving-focus bridge is ready.</summary>
    Task HandleRovingFocusBridgeReadyAsync();
}
