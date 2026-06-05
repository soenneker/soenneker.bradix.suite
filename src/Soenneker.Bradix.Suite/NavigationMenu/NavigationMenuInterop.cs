using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;

namespace Soenneker.Bradix;

/// <inheritdoc cref="INavigationMenuInterop"/>
public sealed class NavigationMenuInterop : INavigationMenuInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;

    private const string _modulePath = "./_content/Soenneker.Bradix.Suite/js/bradix/navigationMenu.js";

    public NavigationMenuInterop(IModuleImportUtil moduleImportUtil)
    {
        _moduleImportUtil = moduleImportUtil;
    }

    public ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default)
    {
        return _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask RegisterNavigationMenuTriggerInteraction(ElementReference element, object dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerNavigationMenuTriggerInteraction", cancellationToken, element, dotNetReference);
    }

    public async ValueTask UnregisterNavigationMenuTriggerInteraction(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterNavigationMenuTriggerInteraction", cancellationToken, element);
    }

    public async ValueTask RegisterNavigationMenuIndicator(ElementReference indicator, ElementReference activeTrigger, ElementReference track,
        object dotNetReference, string orientation, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerNavigationMenuIndicator", cancellationToken, indicator, activeTrigger, track, dotNetReference, orientation)
            ;
    }

    public async ValueTask UpdateNavigationMenuIndicator(ElementReference indicator, ElementReference activeTrigger, ElementReference track,
        string orientation, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("updateNavigationMenuIndicator", cancellationToken, indicator, activeTrigger, track, orientation);
    }

    public async ValueTask UnregisterNavigationMenuIndicator(ElementReference indicator, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterNavigationMenuIndicator", cancellationToken, indicator);
    }

    public async ValueTask RegisterNavigationMenuContentFocusBridge(ElementReference content, ElementReference trigger, ElementReference startProxy,
        ElementReference endProxy, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerNavigationMenuContentFocusBridge", cancellationToken, content, trigger, startProxy, endProxy);
    }

    public async ValueTask UpdateNavigationMenuContentFocusBridge(ElementReference content, ElementReference trigger, ElementReference startProxy,
        ElementReference endProxy, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("updateNavigationMenuContentFocusBridge", cancellationToken, content, trigger, startProxy, endProxy);
    }

    public async ValueTask<bool> FocusNavigationMenuContent(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        return await module.InvokeAsync<bool>("focusNavigationMenuContent", cancellationToken, content);
    }

    public async ValueTask UnregisterNavigationMenuContentFocusBridge(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterNavigationMenuContentFocusBridge", cancellationToken, content);
    }

    public async ValueTask RegisterNavigationMenuViewport(ElementReference viewport, ElementReference content, object dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerNavigationMenuViewport", cancellationToken, viewport, content, dotNetReference);
    }

    public async ValueTask UpdateNavigationMenuViewport(ElementReference viewport, ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("updateNavigationMenuViewport", cancellationToken, viewport, content);
    }

    public async ValueTask UnregisterNavigationMenuViewport(ElementReference viewport, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterNavigationMenuViewport", cancellationToken, viewport);
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
