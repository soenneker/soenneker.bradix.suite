using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixContextMenuTrigger"/>.</summary>
public interface IBradixContextMenuTrigger : IAsyncDisposable {
    /// <summary>Gets or sets a value indicating whether the trigger is disabled.</summary>
    bool Disabled { get; set; }


    /// <summary>Gets or sets the element id.</summary>
    string? Id { get; set; }

    /// <summary>Gets or sets the CSS class names.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the inline style.</summary>
    string? Style { get; set; }

    /// <summary>Gets or sets the child content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets additional attributes spread onto the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
