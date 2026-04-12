using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Hosts a single viewport-registered navigation menu content instance with focus bridging.
/// </summary>
public interface IBradixNavigationMenuViewportContentHost : IAsyncDisposable {
    /// <summary>Registration metadata for this viewport slot.</summary>
    BradixNavigationMenuViewportRegistration Registration { get; set; }

    /// <summary>Whether the presence subtree should be mounted.</summary>
    bool Present { get; set; }

    /// <summary>Whether this slot represents the open item.</summary>
    bool IsOpen { get; set; }

    /// <summary>Motion hint for directional transitions.</summary>
    string Motion { get; set; }

    /// <summary>Raised when the content element reference is captured.</summary>
    EventCallback<ElementReference> OnContentElementCaptured { get; set; }

    /// <summary>Raised on pointer enter over the content region.</summary>
    EventCallback OnPointerEnterContent { get; set; }

    /// <summary>Raised on pointer leave from the content region.</summary>
    EventCallback OnPointerLeaveContent { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Optional direct child content (registration supplies primary content).</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

}
