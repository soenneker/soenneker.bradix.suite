using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the base contract for all Bradix primitive components.
/// Provides common parameters for DOM identity, styling, and attribute forwarding,
/// aligned with Radix-style composition patterns.
/// </summary>
public interface IBradixComponent
{
    /// <summary>
    /// Gets or sets the DOM <c>id</c> attribute for the root element.
    /// </summary>
    /// <remarks>
    /// This is typically used for accessibility relationships (e.g. <c>aria-labelledby</c>)
    /// or direct DOM targeting.
    /// </remarks>
    [Parameter]
    string? Id { get; set; }

    /// <summary>
    /// Gets or sets the CSS class string applied to the root element.
    /// </summary>
    /// <remarks>
    /// This value is merged with any <c>class</c> value provided via <see cref="AdditionalAttributes"/>.
    /// </remarks>
    [Parameter]
    string? Class { get; set; }

    /// <summary>
    /// Gets or sets the inline CSS style string applied to the root element.
    /// </summary>
    /// <remarks>
    /// This value is merged with any <c>style</c> value provided via <see cref="AdditionalAttributes"/>.
    /// Style declarations are concatenated using proper CSS syntax.
    /// </remarks>
    [Parameter]
    string? Style { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the component.
    /// </summary>
    /// <remarks>
    /// This typically represents the child elements of the primitive.
    /// </remarks>
    [Parameter]
    RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be splatted
    /// onto the root element.
    /// </summary>
    /// <remarks>
    /// Attributes provided here will override owned attributes except for
    /// <c>class</c> and <c>style</c>, which are merged.
    /// This enables flexible extension for data attributes, ARIA attributes,
    /// and other DOM properties.
    /// </remarks>
    [Parameter(CaptureUnmatchedValues = true)]
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}