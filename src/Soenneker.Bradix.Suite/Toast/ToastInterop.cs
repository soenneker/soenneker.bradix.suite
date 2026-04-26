using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;

namespace Soenneker.Bradix;

public sealed class ToastInterop : IToastInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;

    private const string _modulePath = "./_content/Soenneker.Bradix.Suite/js/bradix/toast.js";

    public ToastInterop(IModuleImportUtil moduleImportUtil)
    {
        _moduleImportUtil = moduleImportUtil;
    }

    public ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default)
    {
        return _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask RegisterToastViewport(ElementReference wrapper, ElementReference viewport, ElementReference headProxy, ElementReference tailProxy,
        IReadOnlyList<string> hotkey, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerToastViewport", cancellationToken, wrapper, viewport, headProxy, tailProxy, hotkey, dotNetReference)
            ;
    }

    public async ValueTask UnregisterToastViewport(ElementReference viewport, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterToastViewport", cancellationToken, viewport);
    }

    public async ValueTask<bool> IsToastFocused(ElementReference toast, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        return await module.InvokeAsync<bool>("isToastFocused", cancellationToken, toast);
    }

    public async ValueTask<string[]> GetToastAnnounceText(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        string[]? textContent = await module.InvokeAsync<string[]>("getToastAnnounceText", cancellationToken, element);
        return textContent ?? [];
    }

    public async ValueTask DisposeAsync()
    {
        await _moduleImportUtil.DisposeContentModule(_modulePath);
    }
}