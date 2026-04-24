using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the public API for <see cref="BradixToastAction"/>.
/// </summary>
public interface IBradixToastAction
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

    /// <summary>Gets or sets the alternate announcement text for the action.</summary>
    string AltText { get; set; }

    /// <summary>Gets or sets whether the action is disabled.</summary>
    bool Disabled { get; set; }

    /// <summary>Callback invoked before the action closes the toast.</summary>
    EventCallback<Microsoft.AspNetCore.Components.Web.MouseEventArgs> OnClick { get; set; }
}
