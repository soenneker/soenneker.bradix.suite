using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>Defines the public API for <see cref="BradixMenubar"/>.</summary>
public interface IBradixMenubar
{
    /// <summary>Gets or sets the controlled value identifying the open menu.</summary>
    string? Value { get; set; }

    /// <summary>Gets or sets the default value when uncontrolled.</summary>
    string? DefaultValue { get; set; }

    /// <summary>Gets or sets the callback invoked when the value changes.</summary>
    EventCallback<string?> ValueChanged { get; set; }

    /// <summary>Gets or sets the callback invoked when the value changes.</summary>
    EventCallback<string?> OnValueChange { get; set; }

    /// <summary>Gets or sets a value indicating whether roving focus loops at the ends of the menubar.</summary>
    bool Loop { get; set; }

    /// <summary>Gets or sets the text direction (e.g. <c>ltr</c> or <c>rtl</c>).</summary>
    string? Dir { get; set; }

    /// <summary>Releases resources used by the menubar.</summary>
    ValueTask DisposeAsync();

    /// <summary>Handles a document-level pointer down outside interaction from JavaScript interop.</summary>
    Task HandleDocumentPointerDownOutside();

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
