using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the form interop contract.
/// </summary>
public interface IFormInterop : IAsyncDisposable
{
    /// <summary>
    /// Initializes the Form so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers checkbox Root for the Form.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="formId">ID of the form to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the checkbox root registration is complete.</returns>
    ValueTask RegisterCheckboxRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, string? formId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers form Root for the Form.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the form root registration is complete.</returns>
    ValueTask RegisterFormRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters form Root for the Form.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the form root registration has been removed.</returns>
    ValueTask UnregisterFormRoot(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets form control validity.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested bradix Form Validity Snapshot.</returns>
    ValueTask<BradixFormValiditySnapshot> GetFormControlValidity(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets form control state.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested bradix Form Control Snapshot.</returns>
    ValueTask<BradixFormControlSnapshot> GetFormControlState(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets form control custom validity.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="validationMessage">Validation Message for the set form control custom validity operation.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the form control custom validity has been stored.</returns>
    ValueTask SetFormControlCustomValidity(ElementReference element, string? validationMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the value produced by clear Form Custom Validity.
    /// </summary>
    /// <param name="formElement">Form Element for the clear form custom validity operation.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the Form has been cleared.</returns>
    ValueTask ClearFormCustomValidity(ElementReference formElement, CancellationToken cancellationToken = default);

    /// <summary>
    /// Focuses server Invalid Form Control.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>true if focuses server Invalid Form Control; otherwise, false.</returns>
    ValueTask<bool> FocusServerInvalidFormControl(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters checkbox Root for the Form.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the checkbox root registration has been removed.</returns>
    ValueTask UnregisterCheckboxRoot(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the Form form Control.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="formId">ID of the form to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>true if the Form form Control; otherwise, false.</returns>
    ValueTask<bool> IsFormControl(ElementReference element, string? formId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers associated Form Reset for the Form.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="formId">ID of the form to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the associated form reset registration is complete.</returns>
    ValueTask RegisterAssociatedFormReset(ElementReference element, DotNetObjectReference<object> dotNetReference, string? formId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters associated Form Reset for the Form.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the associated form reset registration has been removed.</returns>
    ValueTask UnregisterAssociatedFormReset(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests form Submit.
    /// </summary>
    /// <param name="associatedElement">Associated Element for the request form submit operation.</param>
    /// <param name="formId">ID of the form to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the request form submit operation is complete.</returns>
    ValueTask RequestFormSubmit(ElementReference associatedElement, string? formId = null, CancellationToken cancellationToken = default);
}
