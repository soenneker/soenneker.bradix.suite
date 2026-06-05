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
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register checkbox root operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="formId">The form id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterCheckboxRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, string? formId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register form root operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterFormRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister form root operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterFormRoot(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets form control validity.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<BradixFormValiditySnapshot> GetFormControlValidity(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets form control state.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<BradixFormControlSnapshot> GetFormControlState(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets form control custom validity.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="validationMessage">The validation message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask SetFormControlCustomValidity(ElementReference element, string? validationMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the clear form custom validity operation.
    /// </summary>
    /// <param name="formElement">The form element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ClearFormCustomValidity(ElementReference formElement, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the focus server invalid form control operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<bool> FocusServerInvalidFormControl(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister checkbox root operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterCheckboxRoot(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the is form control operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="formId">The form id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<bool> IsFormControl(ElementReference element, string? formId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register associated form reset operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="formId">The form id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterAssociatedFormReset(ElementReference element, DotNetObjectReference<object> dotNetReference, string? formId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister associated form reset operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterAssociatedFormReset(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the request form submit operation.
    /// </summary>
    /// <param name="associatedElement">The associated element.</param>
    /// <param name="formId">The form id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RequestFormSubmit(ElementReference associatedElement, string? formId = null, CancellationToken cancellationToken = default);
}