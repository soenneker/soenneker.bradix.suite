using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;

namespace Soenneker.Bradix;

public sealed class FocusScopeInterop : IFocusScopeInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;

    private const string _modulePath = "./_content/Soenneker.Bradix.Suite/js/bradix/focusScope.js";

    public FocusScopeInterop(IModuleImportUtil moduleImportUtil)
    {
        _moduleImportUtil = moduleImportUtil;
    }

    public async ValueTask RegisterFocusScope(ElementReference element, DotNetObjectReference<object> dotNetReference, bool loop, bool trapped,
        bool preventMountAutoFocus, bool preventUnmountAutoFocus, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("registerFocusScope", cancellationToken, element, dotNetReference, loop, trapped, preventMountAutoFocus, preventUnmountAutoFocus)
            ;
    }

    public ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default)
    {
        return _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask UpdateFocusScope(ElementReference element, bool loop, bool trapped, bool preventMountAutoFocus, bool preventUnmountAutoFocus,
        CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("updateFocusScope", cancellationToken, element, loop, trapped, preventMountAutoFocus, preventUnmountAutoFocus)
            ;
    }

    public async ValueTask UnregisterFocusScope(ElementReference element, bool unmountAutoFocusPrevented = false, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        await module.InvokeVoidAsync("unregisterFocusScope", cancellationToken, element, unmountAutoFocusPrevented);
    }

    public async ValueTask DisposeAsync()
    {
        await _moduleImportUtil.DisposeContentModule(_modulePath);
    }
}