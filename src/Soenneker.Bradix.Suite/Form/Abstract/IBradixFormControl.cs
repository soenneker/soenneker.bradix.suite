using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Input control wired to <see cref="BradixForm"/> validation and field state.
/// </summary>
public interface IBradixFormControl : IAsyncDisposable {
    /// <summary>Control name submitted with the form.</summary>
    string? Name { get; set; }

    /// <summary>HTML input type; omit for default.</summary>
    string? Type { get; set; }

    /// <summary>Root element id.</summary>
    string? Id { get; set; }

    /// <summary>CSS class names merged onto the root element.</summary>
    string? Class { get; set; }

    /// <summary>Inline style for the root element.</summary>
    string? Style { get; set; }
/// <summary>Additional unmatched attributes applied to the root element.</summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Updates field validity in the parent form.
    /// </summary>
    /// <param name="validity">Validity for the handle validity changed operation.</param>
    /// <returns>A task that completes when the handle validity changed operation is complete.</returns>
    Task HandleValidityChanged(BradixFormValiditySnapshot? validity);

    /// <summary>
    /// Propagates control snapshot changes for custom validation.
    /// </summary>
    /// <param name="snapshot">Snapshot for the handle control state changed operation.</param>
    /// <returns>A task that completes when the handle control state changed operation is complete.</returns>
    Task HandleControlStateChanged(BradixFormControlSnapshot snapshot);

}
