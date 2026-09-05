using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

internal sealed class BradixTextObserver(IDomInterop interop, Func<string, Task> onChanged) : IAsyncDisposable
{
    private DotNetObjectReference<BradixTextObserver>? _reference;
    private ElementReference _element;
    private Task? _registration;
    private bool _disposed;

    public Task Observe(ElementReference element)
    {
        if (_disposed)
            return Task.CompletedTask;

        return _registration ??= Register(element);
    }

    private async Task Register(ElementReference element)
    {
        _element = element;
        _reference = DotNetObjectReference.Create(this);
        string text = await interop.ObserveTextContent(element, _reference);
        if (!_disposed)
            await onChanged(text);
    }

    [JSInvokable]
    public Task OnTextContentChanged(string text) => _disposed ? Task.CompletedTask : onChanged(text);

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;
        try
        {
            if (_registration is not null)
            {
                await _registration;
                await interop.UnobserveTextContent(_element);
            }
        }
        catch (JSDisconnectedException)
        {
            // A disconnected circuit cannot remove its browser observer.
        }
        finally
        {
            _reference?.Dispose();
        }
    }
}
