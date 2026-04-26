using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IFormInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask RegisterCheckboxRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, string? formId = null,
        CancellationToken cancellationToken = default);

    ValueTask RegisterFormRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterFormRoot(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask<BradixFormValiditySnapshot> GetFormControlValidity(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask<BradixFormControlSnapshot> GetFormControlState(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask SetFormControlCustomValidity(ElementReference element, string? validationMessage, CancellationToken cancellationToken = default);

    ValueTask ClearFormCustomValidity(ElementReference formElement, CancellationToken cancellationToken = default);

    ValueTask<bool> FocusServerInvalidFormControl(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask UnregisterCheckboxRoot(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask<bool> IsFormControl(ElementReference element, string? formId = null, CancellationToken cancellationToken = default);

    ValueTask RegisterAssociatedFormReset(ElementReference element, DotNetObjectReference<object> dotNetReference, string? formId = null,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterAssociatedFormReset(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RequestFormSubmit(ElementReference associatedElement, string? formId = null, CancellationToken cancellationToken = default);
}