using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Bradix;

/// <summary>Connects a custom Blazor trigger to its owning primitive without an extra button.</summary>
public sealed class BradixTriggerContext
{
    private readonly Action<ElementReference> _capture;
    private ElementReference _element;

    internal BradixTriggerContext(IReadOnlyDictionary<string, object> attributes,
        EventCallback<MouseEventArgs> onClick, Action<ElementReference> capture, ElementReference element,
        EventCallback<KeyboardEventArgs> onKeyDown = default)
    {
        Attributes = attributes;
        OnClick = onClick;
        OnKeyDown = onKeyDown;
        _capture = capture;
        _element = element;
    }

    /// <summary>Attributes to forward to the actual interactive element, including its id and ARIA state.</summary>
    public IReadOnlyDictionary<string, object> Attributes { get; }

    /// <summary>Forward the element's click event here; native buttons generate clicks for keyboard activation too.</summary>
    public EventCallback<MouseEventArgs> OnClick { get; }

    /// <summary>Forward the element's keydown event here when the trigger exposes keyboard callbacks.</summary>
    public EventCallback<KeyboardEventArgs> OnKeyDown { get; }

    /// <summary>Capture the actual DOM element with <c>@ref="context.Element"</c>, or assign it from a custom component's reference callback.</summary>
    public ElementReference Element
    {
        get => _element;
        set { _element = value; _capture(value); }
    }
}
