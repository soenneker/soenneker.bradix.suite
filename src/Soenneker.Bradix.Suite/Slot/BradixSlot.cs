using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Soenneker.Lepton.Suite;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix slot.
/// </summary>
public sealed class BradixSlot : BradixIdentifiableContentElement
{
    private Dictionary<string, BradixSlotEventHandlers>? _composedHandlers;
    private string? _mergedClass;
    private string? _mergedStyle;
    private int _compositionGeneration;
    private int _composedThisRender;

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
        foreach ((string key, object value) in BuildMergedAttributes())
        {
            AddAttribute(builder, 1, key, value);
        }

        builder.AddContent(2, ChildContent);
        builder.CloseElement();
    }

    private Dictionary<string, object> BuildMergedAttributes()
    {
        Dictionary<string, object> merged = BuildAttributes();
        _compositionGeneration = unchecked(_compositionGeneration + 1);
        _composedThisRender = 0;

        if (ChildAttributes is null)
        {
            _composedHandlers?.Clear();
            return merged;
        }

        if (ChildAttributes is Dictionary<string, object> dictionary)
        {
            foreach (KeyValuePair<string, object> pair in dictionary)
                MergeChildAttribute(merged, pair.Key, pair.Value);

        }
        else
        {
            foreach ((string key, object value) in ChildAttributes)
                MergeChildAttribute(merged, key, value);
        }

        if (_composedHandlers is not null && _composedThisRender != _composedHandlers.Count)
        {
            // Removed events must not retain application handlers. Dictionary removal
            // is supported while enumerating on the targeted runtime.
            foreach (var pair in _composedHandlers)
                if (pair.Value.Generation != _compositionGeneration)
                    _composedHandlers.Remove(pair.Key);
        }

        return merged;
    }

    private void MergeChildAttribute(Dictionary<string, object> merged, string key, object value)
    {
        if (merged.TryGetValue(key, out object? slotValue))
        {
            if (IsEventHandler(key))
            {
                merged[key] = ComposeEventHandlers(key, childValue: value, slotValue);
                return;
            }

            if (string.Equals(key, "class", StringComparison.OrdinalIgnoreCase))
            {
                merged[key] = MergeStringValues(slotValue, value);
                return;
            }

            if (string.Equals(key, "style", StringComparison.OrdinalIgnoreCase))
            {
                merged[key] = MergeStyleValues(slotValue, value);
                return;
            }
        }

        merged[key] = value;
    }

    private object ComposeEventHandlers(string key, object childValue, object slotValue)
    {
        _composedHandlers ??= new(StringComparer.OrdinalIgnoreCase);
        ref var cached = ref CollectionsMarshal.GetValueRefOrAddDefault(_composedHandlers, key, out bool exists);
        if (!exists || cached.Callback is null || !Equals(cached.ChildValue, childValue) || !Equals(cached.SlotValue, slotValue))
        {
            cached = new BradixSlotEventHandlers(childValue, slotValue, CreateComposedCallback(childValue, slotValue));
        }
        cached.Generation = _compositionGeneration;
        _composedThisRender++;
        return cached.Callback;
    }

    private object CreateComposedCallback(object childValue, object slotValue)
    {
        // Allocate the closure only on a cache miss. Capture the pair, so an in-flight
        // event retains its second handler if awaiting the first causes another render.
        return EventCallback.Factory.Create<object?>(this, async args =>
        {
            await InvokeHandler(childValue, args);
            await InvokeHandler(slotValue, args);
        });
    }

    private static Task InvokeHandler(object handler, object? argument)
    {
        switch (handler)
        {
            case BradixEventCallback callback:
                return callback.InvokeAsync(argument);
            case EventCallback eventCallback:
                return eventCallback.InvokeAsync(argument);
            case EventCallback<object?> callback:
                return callback.InvokeAsync(argument);
            case EventCallback<EventArgs> callback:
                return callback.InvokeAsync(argument as EventArgs ?? EventArgs.Empty);
            case Action action:
                action();
                return Task.CompletedTask;
            case Func<Task> callback:
                return callback();
            case Func<ValueTask> callback:
                return callback().AsTask();
            default:
                return InvokeTypedHandler(handler, argument);
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

    private static Task InvokeTypedHandler<TArgument>(object handler, TArgument argument)
    {
        switch (handler)
        {
            case EventCallback<TArgument> callback:
                return callback.InvokeAsync(argument);
            case Action<TArgument> callback:
                callback(argument);
                return Task.CompletedTask;
            case Func<TArgument, Task> callback:
                return callback(argument);
            case Func<TArgument, ValueTask> callback:
                return callback(argument).AsTask();
            default:
                return ThrowUnsupportedHandler(handler);
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

    private string MergeStringValues(object slotValue, object childValue)
    {
        string? first = slotValue?.ToString();
        string? second = childValue?.ToString();
        if (string.IsNullOrWhiteSpace(first) && string.IsNullOrWhiteSpace(second)) return string.Empty;
        return BradixStringCache.MergeClass(first, second, ref _mergedClass)!;
    }

    private string MergeStyleValues(object slotValue, object childValue)
    {
        string? first = slotValue?.ToString();
        string? second = childValue?.ToString();
        bool hasFirst = !string.IsNullOrWhiteSpace(first);
        bool hasSecond = !string.IsNullOrWhiteSpace(second);
        if (!hasFirst && !hasSecond) return string.Empty;
        ReadOnlySpan<char> left = (hasFirst ? first : second).AsSpan().Trim().TrimEnd(';');
        ReadOnlySpan<char> right = hasFirst && hasSecond ? second.AsSpan().Trim().TrimEnd(';') : default;
        ReadOnlySpan<char> separator = hasFirst && hasSecond ? "; " : "";
        int length = left.Length + separator.Length + right.Length + 1;
        if (_mergedStyle is not null && _mergedStyle.Length == length &&
            _mergedStyle.AsSpan(0, left.Length).SequenceEqual(left) &&
            _mergedStyle.AsSpan(left.Length, separator.Length).SequenceEqual(separator) &&
            _mergedStyle.AsSpan(left.Length + separator.Length, right.Length).SequenceEqual(right) &&
            _mergedStyle[^1] == ';')
            return _mergedStyle;
        return _mergedStyle = string.Concat(left, separator, right, ";");
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
                builder.AddAttribute(sequence, key, EventCallback.Factory.Create<object?>(this, callback.Callback));
                return;
        }

        builder.AddAttribute(sequence, key, value);
    }
}
