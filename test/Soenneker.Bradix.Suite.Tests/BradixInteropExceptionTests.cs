using System;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using Microsoft.JSInterop;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixInteropExceptionTests : BunitContext
{
    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task JavaScript_errors_include_operation_and_original_exception(bool returnsValue)
    {
        var original = new JSException("Browser operation failed");
        Exception? actual = await InvokeFailingOperation(returnsValue, original);

        await Assert.That(actual is JSException).IsTrue();
        await Assert.That(ReferenceEquals(actual?.InnerException, original)).IsTrue();
        await Assert.That(actual!.Message.Contains(returnsValue ? "getTextContent" : "unobserveTextContent")).IsTrue();
    }

    [Test]
    [Arguments(false, false)]
    [Arguments(true, false)]
    [Arguments(false, true)]
    [Arguments(true, true)]
    public async Task Disconnection_and_cancellation_propagate_unchanged(bool returnsValue, bool canceled)
    {
        Exception original = canceled ? new OperationCanceledException() : new JSDisconnectedException("Disconnected");
        Exception? actual = await InvokeFailingOperation(returnsValue, original);
        await Assert.That(ReferenceEquals(actual, original)).IsTrue();
    }

    private async Task<Exception?> InvokeFailingOperation(bool returnsValue, Exception exception)
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        if (returnsValue)
            module.Setup<string>("getTextContent", _ => true).SetException(exception);
        else
            module.SetupVoid("unobserveTextContent", _ => true).SetException(exception);

        await using var interop = new DomInterop(new BradixTestModuleImportUtil(JSInterop.JSRuntime));
        try
        {
            if (returnsValue)
                await interop.GetTextContent(default, CancellationToken.None);
            else
                await interop.UnobserveTextContent(default, CancellationToken.None);
            return null;
        }
        catch (Exception actual)
        {
            return actual;
        }
    }
}
