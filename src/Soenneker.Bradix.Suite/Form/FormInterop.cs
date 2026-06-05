using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;

namespace Soenneker.Bradix;

/// <inheritdoc cref="IFormInterop"/>
public sealed class FormInterop : IFormInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;

    private const string _modulePath = "./_content/Soenneker.Bradix.Suite/js/bradix/forms.js";

    public FormInterop(IModuleImportUtil moduleImportUtil)
    {
        _moduleImportUtil = moduleImportUtil;
    }

    public async ValueTask RegisterCheckboxRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, string? formId = null,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerCheckboxRoot", cancellationToken, element, dotNetReference, formId);
    }

    public ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default)
    {
        return _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask RegisterFormRoot(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerFormRoot", cancellationToken, element, dotNetReference);
    }

    public async ValueTask UnregisterFormRoot(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterFormRoot", cancellationToken, element);
    }

    public async ValueTask<BradixFormValiditySnapshot> GetFormControlValidity(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        var snapshot = await module.InvokeAsync<BradixFormValiditySnapshot>("getFormControlValidity", cancellationToken, element)
            ;
        return snapshot ?? new BradixFormValiditySnapshot();
    }

    public async ValueTask<BradixFormControlSnapshot> GetFormControlState(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        var snapshot = await module.InvokeAsync<BradixFormControlSnapshot>("getFormControlState", cancellationToken, element)
            ;
        return snapshot ?? new BradixFormControlSnapshot();
    }

    public async ValueTask SetFormControlCustomValidity(ElementReference element, string? validationMessage, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("setFormControlCustomValidity", cancellationToken, element, validationMessage);
    }

    public async ValueTask ClearFormCustomValidity(ElementReference formElement, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("clearFormCustomValidity", cancellationToken, formElement);
    }

    public async ValueTask<bool> FocusServerInvalidFormControl(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        return await module.InvokeAsync<bool>("focusServerInvalidFormControl", cancellationToken, element);
    }

    public async ValueTask UnregisterCheckboxRoot(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterCheckboxRoot", cancellationToken, element);
    }

    public async ValueTask<bool> IsFormControl(ElementReference element, string? formId = null, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        return await module.InvokeAsync<bool>("isFormControl", cancellationToken, element, formId);
    }

    public async ValueTask RegisterAssociatedFormReset(ElementReference element, DotNetObjectReference<object> dotNetReference, string? formId = null,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerAssociatedFormReset", cancellationToken, element, dotNetReference, formId);
    }

    public async ValueTask UnregisterAssociatedFormReset(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterAssociatedFormReset", cancellationToken, element);
    }

    public async ValueTask RequestFormSubmit(ElementReference associatedElement, string? formId = null, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("requestFormSubmit", cancellationToken, associatedElement, formId);
    }

    /// <summary>
    /// Asynchronously releases resources used by the current instance.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await _moduleImportUtil.DisposeContentModule(_modulePath);
    }
}