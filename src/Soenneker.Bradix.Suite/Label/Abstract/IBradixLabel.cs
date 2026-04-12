using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>
/// Associates label text with a form control and guards text selection on mouse down.
/// </summary>
public interface IBradixLabel
{
    /// <summary>Value for the label's <c>for</c> attribute.</summary>
    string? For { get; set; }

    /// <summary>Raised when a guarded mouse down is forwarded from script.</summary>
    EventCallback<MouseEventArgs> OnMouseDown { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Label content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Unregisters label interop handlers.</summary>
    ValueTask DisposeAsync();

    /// <summary>Interop handler for delegated mouse down events.</summary>
    Task HandleMouseDownFromJsAsync(BradixDelegatedMouseEvent args);
}
