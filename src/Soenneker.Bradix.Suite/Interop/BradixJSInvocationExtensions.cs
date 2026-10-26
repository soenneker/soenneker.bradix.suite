using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

internal static class BradixJSInvocationExtensions
{
    internal static ValueTask InvokeBradixVoidAsync(this IJSObjectReference module, string identifier, params object?[]? args)
        => module.InvokeBradixVoidAsync(identifier, CancellationToken.None, args);

    internal static async ValueTask InvokeBradixVoidAsync(this IJSObjectReference module, string identifier, CancellationToken cancellationToken,
        params object?[]? args)
    {
        try
        {
            await module.InvokeVoidAsync(identifier, cancellationToken, args);
        }
        catch (JSException exception)
        {
            throw new JSException($"Bradix JavaScript operation '{identifier}' failed: {exception.Message}", exception);
        }
    }

    internal static async ValueTask<TValue> InvokeBradixAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(this IJSObjectReference module, string identifier, CancellationToken cancellationToken,
        params object?[]? args)
    {
        try
        {
            return await module.InvokeAsync<TValue>(identifier, cancellationToken, args);
        }
        catch (JSException exception)
        {
            throw new JSException($"Bradix JavaScript operation '{identifier}' failed: {exception.Message}", exception);
        }
    }
}
