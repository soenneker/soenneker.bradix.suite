using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;

namespace Soenneker.Bradix;

/// <inheritdoc cref="ITooltipInterop"/>
public sealed class TooltipInterop : ITooltipInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;

    private const string _modulePath = "./_content/Soenneker.Bradix.Suite/js/bradix/tooltip.js";

    public TooltipInterop(IModuleImportUtil moduleImportUtil)
    {
        _moduleImportUtil = moduleImportUtil;
    }

    public ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default)
    {
        return _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask RegisterTooltipTrigger(ElementReference element, object dotNetReference, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerTooltipTrigger", cancellationToken, element, dotNetReference);
    }

    public async ValueTask UnregisterTooltipTrigger(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterTooltipTrigger", cancellationToken, element);
    }

    public async ValueTask RegisterTooltipContent(ElementReference content, ElementReference trigger, object dotNetReference, string contentId,
        bool hoverableContent, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerTooltipContent", cancellationToken, content, trigger, dotNetReference, contentId, hoverableContent);
    }

    public async ValueTask UnregisterTooltipContent(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterTooltipContent", cancellationToken, content);
    }

    public async ValueTask DispatchTooltipOpen(string contentId, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("dispatchTooltipOpen", cancellationToken, contentId);
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