using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;

namespace Soenneker.Bradix;

/// <inheritdoc cref="IHoverCardAvatarInterop"/>
public sealed class HoverCardAvatarInterop : IHoverCardAvatarInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;

    private const string _modulePath = "./_content/Soenneker.Bradix.Suite/js/bradix/hoverCardAvatar.js";

    public HoverCardAvatarInterop(IModuleImportUtil moduleImportUtil)
    {
        _moduleImportUtil = moduleImportUtil;
    }

    public async ValueTask DisableHoverCardContentTabNavigation(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("disableHoverCardContentTabNavigation", cancellationToken, content);
    }

    public ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default)
    {
        return _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask RegisterHoverCardSelectionContainment(ElementReference content, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerHoverCardSelectionContainment", cancellationToken, content, dotNetReference);
    }

    public async ValueTask BeginHoverCardSelectionContainment(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("beginHoverCardSelectionContainment", cancellationToken, content);
    }

    public async ValueTask UnregisterHoverCardSelectionContainment(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterHoverCardSelectionContainment", cancellationToken, content);
    }

    public ValueTask RegisterAvatarImageLoadingStatus(string? src, string? crossOrigin, string? referrerPolicy,
        DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask UnregisterAvatarImageLoadingStatus(DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
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
