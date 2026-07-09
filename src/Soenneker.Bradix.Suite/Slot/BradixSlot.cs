using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Soenneker.Lepton.Suite;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix slot.
/// </summary>
public sealed class BradixSlot : LeptonIdentifiableContentElement
{
    /// <summary>
    /// Gets or sets element name.
    /// </summary>
    [Parameter, EditorRequired]
    public string ElementName { get; set; } = null!;

    /// <summary>
    /// Gets or sets child attributes.
    /// </summary>
    [Parameter]
    public IReadOnlyDictionary<string, object>? ChildAttributes { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(ElementName))
            throw new InvalidOperationException("BradixSlot requires a non-empty ElementName.");

        builder.OpenElement(0, ElementName);
        var sequence = 1;

        foreach ((string key, object value) in BuildMergedAttributes())
        {
            AddAttribute(builder, sequence++, key, value);
        }

        builder.AddContent(sequence, ChildContent);
        builder.CloseElement();
    }

    private Dictionary<string, object> BuildMergedAttributes()
    {
        Dictionary<string, object> merged = BuildAttributes();

        if (ChildAttributes is null)
            return merged;

        foreach ((string key, object value) in ChildAttributes)
        {
            if (merged.TryGetValue(key, out object? slotValue))
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
        return EventCallback.Factory.Create<object?>(this, async args =>
        {
            await InvokeHandler(childValue, args);
            await InvokeHandler(slotValue, args);
        });
    }

    private static async Task InvokeHandler(object handler, object? argument)
    {
        switch (handler)
        {
            case BradixEventCallback callback:
                await callback.InvokeAsync(argument);
                return;
            case EventCallback eventCallback:
                await eventCallback.InvokeAsync(argument);
                return;
            case EventCallback<object?> callback:
                await callback.InvokeAsync(argument);
                return;
            case EventCallback<EventArgs> callback:
                await callback.InvokeAsync(argument as EventArgs ?? EventArgs.Empty);
                return;
            case Action action:
                action();
                return;
            case Func<Task> callback:
                await callback();
                return;
            case Func<ValueTask> callback:
                await callback();
                return;
            default:
                await InvokeTypedHandler(handler, argument);
                return;
        }
    }

    private static Task InvokeTypedHandler(object handler, object? argument)
    {
        return argument switch
        {
            ChangeEventArgs args => InvokeTypedHandler(handler, args),
            ClipboardEventArgs args => InvokeTypedHandler(handler, args),
            DragEventArgs args => InvokeTypedHandler(handler, args),
            Microsoft.AspNetCore.Components.Web.ErrorEventArgs args => InvokeTypedHandler(handler, args),
            FocusEventArgs args => InvokeTypedHandler(handler, args),
            KeyboardEventArgs args => InvokeTypedHandler(handler, args),
            PointerEventArgs args => InvokeTypedHandler(handler, args),
            WheelEventArgs args => InvokeTypedHandler(handler, args),
            MouseEventArgs args => InvokeTypedHandler(handler, args),
            ProgressEventArgs args => InvokeTypedHandler(handler, args),
            TouchEventArgs args => InvokeTypedHandler(handler, args),
            EventArgs args => InvokeTypedHandler(handler, args),
            null => InvokeTypedHandler<object?>(handler, null),
            _ => ThrowUnsupportedHandler(handler)
        };
    }

    private static async Task InvokeTypedHandler<TArgument>(object handler, TArgument argument)
    {
        switch (handler)
        {
            case EventCallback<TArgument> callback:
                await callback.InvokeAsync(argument);
                return;
            case Action<TArgument> callback:
                callback(argument);
                return;
            case Func<TArgument, Task> callback:
                await callback(argument);
                return;
            case Func<TArgument, ValueTask> callback:
                await callback(argument);
                return;
            default:
                await ThrowUnsupportedHandler(handler);
                return;
        }
    }

    private static Task ThrowUnsupportedHandler(object handler)
    {
        throw new InvalidOperationException($"Unsupported BradixSlot event handler type '{handler.GetType().FullName}'. " +
                                            $"Wrap custom typed handlers with {nameof(BradixEventCallback)}.Create.");
    }

    private static bool IsEventHandler(string key)
    {
        return key.StartsWith("on", StringComparison.OrdinalIgnoreCase);
    }

    private static string MergeStringValues(object slotValue, object childValue)
    {
        return MergeNonEmptyValues(slotValue?.ToString(), childValue?.ToString());
    }

    private static string MergeStyleValues(object slotValue, object childValue)
    {
        return MergeNonEmptyValues(NormalizeStyle(slotValue?.ToString()), NormalizeStyle(childValue?.ToString()));
    }

    private static string MergeNonEmptyValues(string? first, string? second)
    {
        bool hasFirst = !string.IsNullOrWhiteSpace(first);
        bool hasSecond = !string.IsNullOrWhiteSpace(second);

        return (hasFirst, hasSecond) switch
        {
            (true, true) => $"{first} {second}",
            (true, false) => first!,
            (false, true) => second!,
            _ => string.Empty
        };
    }

    private static string? NormalizeStyle(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Trim().TrimEnd(';') + ";";
    }

    private void AddAttribute(RenderTreeBuilder builder, int sequence, string key, object value)
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
            case BradixEventCallback callback:
                builder.AddAttribute(sequence, key, EventCallback.Factory.Create<object?>(this, callback.InvokeAsync));
                return;
        }

        builder.AddAttribute(sequence, key, value);
    }
}
