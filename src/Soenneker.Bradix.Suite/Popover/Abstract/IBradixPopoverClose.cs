using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Button that closes its parent popover.
/// </summary>
public interface IBradixPopoverClose : IAsyncDisposable {
    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>When true, the close button ignores close interactions.</summary>
    bool Disabled { get; set; }

    /// <summary>Callback invoked when the close button is clicked.</summary>
    EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>Close button content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }


    /// <summary>Interop handler when delegated interaction is ready.</summary>
    Task HandleDelegatedInteractionReady();

    /// <summary>Interop handler for delegated click events.</summary>
    Task HandleDelegatedClick(BradixDelegatedMouseEvent args);
}
