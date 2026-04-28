using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;

namespace Soenneker.Bradix;

///<inheritdoc cref="IPopperInterop"/>
public sealed class PopperInterop : IPopperInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;
    private readonly IResourceLoader _resourceLoader;

    private const string _modulePath = "./_content/Soenneker.Bradix.Suite/js/bradix/popper.js";
    private const string _floatingUiCoreCdnPath = "https://cdn.jsdelivr.net/npm/@floating-ui/core@1.7.2/dist/floating-ui.core.umd.min.js";
    private const string _floatingUiCoreCdnIntegrity = "sha256-OhWDdFHrIg8eNZaNgWL2ax7tjKNFOBQq3WErqxfHdlQ=";
    private const string _floatingUiDomCdnPath = "https://cdn.jsdelivr.net/npm/@floating-ui/dom@1.7.2/dist/floating-ui.dom.umd.min.js";
    private const string _floatingUiDomCdnIntegrity = "sha256-cycZmidLw+l9uWDr4bUhL26YMJg1G6aM0AnUEPG9sME=";
    private const string _floatingUiCoreLocalPath = "./_content/Soenneker.Bradix.Suite/js/vendor/floating-ui.core.umd.min.js";
    private const string _floatingUiDomLocalPath = "./_content/Soenneker.Bradix.Suite/js/vendor/floating-ui.dom.umd.min.js";

    public PopperInterop(IModuleImportUtil moduleImportUtil, IResourceLoader resourceLoader)
    {
        _moduleImportUtil = moduleImportUtil;
        _resourceLoader = resourceLoader;
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

    private async ValueTask EnsureFloatingUi(CancellationToken cancellationToken)
    {
        try
        {
            await _resourceLoader.LoadScriptAndWaitForVariable(_floatingUiCoreCdnPath, "FloatingUICore", _floatingUiCoreCdnIntegrity, cancellationToken: cancellationToken);
            await _resourceLoader.LoadScriptAndWaitForVariable(_floatingUiDomCdnPath, "FloatingUIDOM", _floatingUiDomCdnIntegrity, cancellationToken: cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            await _resourceLoader.LoadScriptAndWaitForVariable(_floatingUiCoreLocalPath, "FloatingUICore", cancellationToken: cancellationToken);
            await _resourceLoader.LoadScriptAndWaitForVariable(_floatingUiDomLocalPath, "FloatingUIDOM", cancellationToken: cancellationToken);
        }
    }
}
