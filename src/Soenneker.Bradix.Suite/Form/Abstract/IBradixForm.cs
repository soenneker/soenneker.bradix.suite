using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Root form primitive coordinating client validity, custom matchers, and field messaging.
/// </summary>
public interface IBradixForm : IAsyncDisposable {
    /// <summary>Raised on submit after the form begins clearing server-side errors.</summary>
    EventCallback OnClearServerErrors { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }

    /// <summary>Form content.</summary>
    RenderFragment? ChildContent { get; set; }

    /// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }


    /// <summary>Interop handler when invalid controls are reported from script.</summary>
    Task HandleInvalidControls(string[] fieldNames);
}
