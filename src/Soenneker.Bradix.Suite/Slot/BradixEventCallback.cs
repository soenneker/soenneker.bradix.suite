using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix;

/// <summary>
/// Adapts a custom typed event handler for reflection-free composition by <see cref="BradixSlot"/>.
/// Standard Blazor browser event callbacks do not need to be wrapped.
/// </summary>
public sealed class BradixEventCallback
{
    private readonly Func<object?, Task> _callback;

    private BradixEventCallback(Func<object?, Task> callback)
    {
        _callback = callback;
    }

    /// <summary>
    /// Wraps a typed Blazor event callback.
    /// </summary>
    public static BradixEventCallback Create<TArgument>(EventCallback<TArgument> callback)
    {
        return new BradixEventCallback(argument => callback.InvokeAsync(GetArgument<TArgument>(argument)));
    }

    /// <summary>
    /// Wraps a synchronous typed event handler.
    /// </summary>
    public static BradixEventCallback Create<TArgument>(Action<TArgument> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        return new BradixEventCallback(argument =>
        {
            callback(GetArgument<TArgument>(argument));
            return Task.CompletedTask;
        });
    }

    /// <summary>
    /// Wraps an asynchronous typed event handler.
    /// </summary>
    public static BradixEventCallback Create<TArgument>(Func<TArgument, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        return new BradixEventCallback(argument => callback(GetArgument<TArgument>(argument)));
    }

    /// <summary>
    /// Wraps an asynchronous typed event handler that returns a <see cref="ValueTask"/>.
    /// </summary>
    public static BradixEventCallback Create<TArgument>(Func<TArgument, ValueTask> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        return new BradixEventCallback(argument => callback(GetArgument<TArgument>(argument)).AsTask());
    }

    internal Task InvokeAsync(object? argument)
    {
        return _callback(argument);
    }

    private static TArgument GetArgument<TArgument>(object? argument)
    {
        if (argument is TArgument typedArgument)
            return typedArgument;

        if (argument is null && default(TArgument) is null)
            return default!;

        throw new InvalidOperationException($"Cannot invoke a {nameof(BradixEventCallback)}<{typeof(TArgument).FullName}> with an argument of type " +
                                            $"'{argument?.GetType().FullName ?? "null"}'.");
    }
}
