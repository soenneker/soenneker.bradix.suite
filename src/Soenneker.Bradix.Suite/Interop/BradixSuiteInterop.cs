using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Bradix.Suite.Abstract;

namespace Soenneker.Bradix.Suite.Interop;

public sealed class BradixSuiteInterop : ISuiteInterop
{
    private readonly IJSRuntime _jsRuntime;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private IJSObjectReference? _module;

    public BradixSuiteInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async ValueTask Initialize(CancellationToken cancellationToken = default)
    {
        _ = await GetModule(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask ObserveCollapsibleContent(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("observeCollapsibleContent", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask UnobserveCollapsibleContent(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unobserveCollapsibleContent", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask RegisterRovingFocusNavigationKeys(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerRovingFocusNavigationKeys", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask UnregisterRovingFocusNavigationKeys(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterRovingFocusNavigationKeys", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask RegisterLabelTextSelectionGuard(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("registerLabelTextSelectionGuard", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask UnregisterLabelTextSelectionGuard(ElementReference element, CancellationToken cancellationToken = default)
    {
        IJSObjectReference module = await GetModule(cancellationToken).ConfigureAwait(false);
        await module.InvokeVoidAsync("unregisterLabelTextSelectionGuard", cancellationToken, element).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            try
            {
                await _module.DisposeAsync().ConfigureAwait(false);
            }
            catch (JSDisconnectedException)
            {
            }
        }

        _semaphore.Dispose();
    }

    private async ValueTask<IJSObjectReference> GetModule(CancellationToken cancellationToken)
    {
        if (_module is not null)
            return _module;

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (_module is null)
            {
                _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    cancellationToken,
                    "./_content/Soenneker.Bradix.Suite/js/bradix.js").ConfigureAwait(false);
            }

            return _module;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
