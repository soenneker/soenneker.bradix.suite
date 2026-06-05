using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;

namespace Soenneker.Bradix;

/// <inheritdoc cref="ICollapsibleInterop"/>
public sealed class CollapsibleInterop : ICollapsibleInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;

    private const string _modulePath = "./_content/Soenneker.Bradix.Suite/js/bradix/collapsible.js";

    public CollapsibleInterop(IModuleImportUtil moduleImportUtil)
    {
        _moduleImportUtil = moduleImportUtil;
    }

    public async ValueTask ObserveCollapsibleContent(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("observeCollapsibleContent", cancellationToken, element);
    }

    public ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default)
    {
        return _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask UnobserveCollapsibleContent(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unobserveCollapsibleContent", cancellationToken, element);
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