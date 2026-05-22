using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Soenneker.Blazor.Interops.Floating.Abstract;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;
using Soenneker.Bradix.Configuration;

namespace Soenneker.Bradix;

///<inheritdoc cref="IPopperInterop"/>
public sealed class PopperInterop : IPopperInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;
    private readonly IFloatingUiInterop _floatingUiInterop;
    private readonly IOptions<BradixSuiteOptions> _options;

    private const string _modulePath = "./_content/Soenneker.Bradix.Suite/js/bradix/popper.js";

    public PopperInterop(IModuleImportUtil moduleImportUtil, IFloatingUiInterop floatingUiInterop, IOptions<BradixSuiteOptions> options)
    {
        _moduleImportUtil = moduleImportUtil;
        _floatingUiInterop = floatingUiInterop;
        _options = options;
    }

    public ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default)
    {
        return InitializeCore(cancellationToken);
    }

    private async ValueTask<IJSObjectReference> InitializeCore(CancellationToken cancellationToken)
    {
        await EnsureFloatingUi(cancellationToken);
        return await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask RegisterPopperContent(ElementReference anchor, ElementReference content, ElementReference arrow,
        DotNetObjectReference<object> dotNetReference, object options, CancellationToken cancellationToken = default)
    {
        await EnsureFloatingUi(cancellationToken);
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerPopperContent", cancellationToken, anchor, content, arrow, dotNetReference, options);
    }

    public async ValueTask RegisterPopperContentBySelector(string anchorSelector, ElementReference content, ElementReference arrow,
        DotNetObjectReference<object> dotNetReference, object options, CancellationToken cancellationToken = default)
    {
        await EnsureFloatingUi(cancellationToken);
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerPopperContentBySelector", cancellationToken, anchorSelector, content, arrow, dotNetReference, options);
    }

    public async ValueTask RegisterVirtualPopperContent(ElementReference content, ElementReference arrow, DotNetObjectReference<object> dotNetReference,
        double x, double y, object options, CancellationToken cancellationToken = default)
    {
        await EnsureFloatingUi(cancellationToken);
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerVirtualPopperContent", cancellationToken, content, arrow, dotNetReference, x, y, options);
    }

    public async ValueTask UpdatePopperContent(ElementReference content, ElementReference arrow, object options, CancellationToken cancellationToken = default)
    {
        await EnsureFloatingUi(cancellationToken);
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("updatePopperContent", cancellationToken, content, arrow, options);
    }

    public async ValueTask UpdateVirtualPopperContent(ElementReference content, ElementReference arrow, double x, double y, object options,
        CancellationToken cancellationToken = default)
    {
        await EnsureFloatingUi(cancellationToken);
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("updateVirtualPopperContent", cancellationToken, content, arrow, x, y, options);
    }

    public async ValueTask UnregisterPopperContent(ElementReference content, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterPopperContent", cancellationToken, content);
    }

    public async ValueTask DisposeAsync()
    {
        await _moduleImportUtil.DisposeContentModule(_modulePath);
    }

    private ValueTask EnsureFloatingUi(CancellationToken cancellationToken)
    {
        return _floatingUiInterop.Initialize(_options.Value.UseCdn, cancellationToken);
    }
}
