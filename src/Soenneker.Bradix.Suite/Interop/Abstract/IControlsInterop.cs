using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public interface IControlsInterop : IAsyncDisposable
{
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    ValueTask SyncCheckboxBubbleInputState(ElementReference element, bool isChecked, bool isIndeterminate, bool dispatchEvent, bool bubbles = true,
        CancellationToken cancellationToken = default);

    ValueTask ClickElement(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask FocusElementDeferred(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask SelectInputText(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask SyncInputValue(ElementReference element, string? value, CancellationToken cancellationToken = default);

    ValueTask<bool> IsDirectionRtl(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterSliderPointerBridge(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    ValueTask UnregisterSliderPointerBridge(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask SyncSliderBubbleInputValue(ElementReference element, double value, bool dispatchEvent, CancellationToken cancellationToken = default);

    ValueTask RegisterSelectBubbleInput(ElementReference element, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterSelectBubbleInput(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask SyncSelectBubbleInputValue(ElementReference element, string? value, bool dispatchEvent, CancellationToken cancellationToken = default);

    ValueTask CapturePointer(ElementReference element, long pointerId, CancellationToken cancellationToken = default);

    ValueTask ReleasePointer(ElementReference element, long pointerId, CancellationToken cancellationToken = default);

    ValueTask SuppressNextClick(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask FocusElementById(string? elementId, CancellationToken cancellationToken = default);

    ValueTask FocusElementByIdDeferred(string? elementId, CancellationToken cancellationToken = default);

    ValueTask FocusElementPreventScroll(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask<bool> FocusFirstMatchingDescendant(ElementReference element, string selector, CancellationToken cancellationToken = default);

    ValueTask ScrollElementIntoViewNearest(ElementReference element, CancellationToken cancellationToken = default);

    ValueTask RegisterOneTimePasswordInput(ElementReference element, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    ValueTask UnregisterOneTimePasswordInput(ElementReference element, CancellationToken cancellationToken = default);
}
