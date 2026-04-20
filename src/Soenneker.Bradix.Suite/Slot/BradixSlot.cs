using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Soenneker.Bradix;

public sealed class BradixSlot : BradixComponent
{
    [Parameter, EditorRequired]
    public string ElementName { get; set; } = null!;

    [Parameter]
    public IReadOnlyDictionary<string, object>? ChildAttributes { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(ElementName))
            throw new InvalidOperationException("BradixSlot requires a non-empty ElementName.");

        builder.OpenElement(0, ElementName);
        var sequence = 1;

        foreach ((var key, var value) in BuildMergedAttributes())
        {
            AddAttribute(builder, sequence++, key, value);
        }

        builder.AddContent(sequence, ChildContent);
        builder.CloseElement();
    }

    private Dictionary<string, object> BuildMergedAttributes()
    {
        var merged = BuildAttributes();

        if (ChildAttributes is null)
            return merged;

        foreach ((var key, var value) in ChildAttributes)
        {
            if (merged.TryGetValue(key, out var slotValue))
            {
                if (IsEventHandler(key))
                {
                    merged[key] = ComposeEventHandlers(childValue: value, slotValue);
                    continue;
                }

                if (string.Equals(key, "class", StringComparison.OrdinalIgnoreCase))
                {
                    merged[key] = MergeStringValues(slotValue, value);
                    continue;
                }

                if (string.Equals(key, "style", StringComparison.OrdinalIgnoreCase))
                {
                    merged[key] = MergeStyleValues(slotValue, value);
                    continue;
                }
            }

            merged[key] = value;
        }

        return merged;
    }

    private object ComposeEventHandlers(object childValue, object slotValue)
    {
        var argumentType = ResolveEventArgumentType(childValue) ??
                           ResolveEventArgumentType(slotValue) ??
                           typeof(object);

        var method = typeof(BradixSlot).GetMethod(nameof(CreateComposedEventCallback), BindingFlags.Instance | BindingFlags.NonPublic)!
                                       .MakeGenericMethod(argumentType);

        return method.Invoke(this, [childValue, slotValue])!;
    }

    private EventCallback<TArgument> CreateComposedEventCallback<TArgument>(object childValue, object slotValue)
    {
        return EventCallback.Factory.Create<TArgument>(this, async (TArgument args) =>
        {
            await InvokeHandler(childValue, args);
            await InvokeHandler(slotValue, args);
        });
    }

    private static async Task InvokeHandler(object handler, object? argument)
    {
        switch (handler)
        {
            case EventCallback eventCallback:
                await eventCallback.InvokeAsync(argument);
                return;
            case MulticastDelegate @delegate:
            {
                var parameters = @delegate.Method.GetParameters();
                var result = parameters.Length == 0
                    ? @delegate.DynamicInvoke()
                    : @delegate.DynamicInvoke(argument);

                if (result is Task task)
                    await task;
                else if (result is ValueTask valueTask)
                    await valueTask;

                return;
            }
            default:
            {
                var invokeAsync = handler.GetType().GetMethod("InvokeAsync", [typeof(object)]);

                if (invokeAsync is null)
                    return;

                var result = invokeAsync.Invoke(handler, [argument]);

                if (result is Task task)
                    await task;
                else if (result is ValueTask valueTask)
                    await valueTask;

                return;
            }
        }
    }

    private static Type? ResolveEventArgumentType(object handler)
    {
        if (handler is MulticastDelegate @delegate)
        {
            var parameters = @delegate.Method.GetParameters();
            return parameters.Length > 0 ? parameters[0].ParameterType : typeof(object);
        }

        var type = handler.GetType();

        if (type == typeof(EventCallback))
            return typeof(object);

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EventCallback<>))
            return type.GetGenericArguments()[0];

        return null;
    }

    private static bool IsEventHandler(string key)
    {
        return key.StartsWith("on", StringComparison.OrdinalIgnoreCase);
    }

    private static string MergeStringValues(object slotValue, object childValue)
    {
        return string.Join(" ", new[] { slotValue?.ToString(), childValue?.ToString() }.Where(static value => !string.IsNullOrWhiteSpace(value)));
    }

    private static string MergeStyleValues(object slotValue, object childValue)
    {
        return string.Join(" ", new[] { NormalizeStyle(slotValue?.ToString()), NormalizeStyle(childValue?.ToString()) }
            .Where(static value => !string.IsNullOrWhiteSpace(value)));
    }

    private static string? NormalizeStyle(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Trim().TrimEnd(';') + ";";
    }

    private static void AddAttribute(RenderTreeBuilder builder, int sequence, string key, object value)
    {
        switch (value)
        {
            case string stringValue:
                builder.AddAttribute(sequence, key, stringValue);
                return;
            case bool boolValue:
                builder.AddAttribute(sequence, key, boolValue);
                return;
            case EventCallback eventCallback:
                builder.AddAttribute(sequence, key, eventCallback);
                return;
            case MulticastDelegate @delegate:
                builder.AddAttribute(sequence, key, @delegate);
                return;
        }

        var type = value.GetType();

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EventCallback<>))
        {
            var method = typeof(BradixSlot).GetMethod(nameof(AddTypedEventCallback), BindingFlags.Static | BindingFlags.NonPublic)!
                                           .MakeGenericMethod(type.GetGenericArguments()[0]);

            method.Invoke(null, [builder, sequence, key, value]);
            return;
        }

        builder.AddAttribute(sequence, key, value);
    }

    private static void AddTypedEventCallback<TArgument>(RenderTreeBuilder builder, int sequence, string key, EventCallback<TArgument> value)
    {
        builder.AddAttribute(sequence, key, value);
    }
}
