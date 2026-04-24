using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixRadioGroupItem"/>.
/// </summary>
public interface IBradixRadioGroupItem : IAsyncDisposable {
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

    /// <summary>Gets or sets the value represented by this item.</summary>
    string Value { get; set; }

    /// <summary>Gets or sets whether this item is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Gets or sets the value submitted with native bubble inputs.</summary>
    string? InputValue { get; set; }

    /// <summary>Gets or sets the <c>form</c> attribute for detached native inputs.</summary>
    string? Form { get; set; }


    /// <summary>Called from script when the roving-focus bridge is ready.</summary>
    Task HandleRovingFocusBridgeReady();
}
