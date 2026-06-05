using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

/// <summary>
/// Defines the controls interop contract.
/// </summary>
public interface IControlsInterop : IAsyncDisposable
{
    /// <summary>
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the sync checkbox bubble input state operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="isChecked">The is checked.</param>
    /// <param name="isIndeterminate">The is indeterminate.</param>
    /// <param name="dispatchEvent">The dispatch event.</param>
    /// <param name="bubbles">The bubbles.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask SyncCheckboxBubbleInputState(ElementReference element, bool isChecked, bool isIndeterminate, bool dispatchEvent, bool bubbles = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the click element operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ClickElement(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the focus element deferred operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask FocusElementDeferred(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the select input text operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask SelectInputText(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the sync input value operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask SyncInputValue(ElementReference element, string? value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the is direction rtl operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<bool> IsDirectionRtl(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register slider pointer bridge operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterSliderPointerBridge(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister slider pointer bridge operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterSliderPointerBridge(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the sync slider bubble input value operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The value.</param>
    /// <param name="dispatchEvent">The dispatch event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask SyncSliderBubbleInputValue(ElementReference element, double value, bool dispatchEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register select bubble input operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterSelectBubbleInput(ElementReference element, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister select bubble input operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterSelectBubbleInput(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the sync select bubble input value operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The value.</param>
    /// <param name="dispatchEvent">The dispatch event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask SyncSelectBubbleInputValue(ElementReference element, string? value, bool dispatchEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the capture pointer operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="pointerId">The pointer id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask CapturePointer(ElementReference element, long pointerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the release pointer operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="pointerId">The pointer id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ReleasePointer(ElementReference element, long pointerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the suppress next click operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask SuppressNextClick(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the focus element by id operation.
    /// </summary>
    /// <param name="elementId">The element id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask FocusElementById(string? elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the focus element by id deferred operation.
    /// </summary>
    /// <param name="elementId">The element id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask FocusElementByIdDeferred(string? elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the focus element prevent scroll operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask FocusElementPreventScroll(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the focus first matching descendant operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="selector">The selector.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<bool> FocusFirstMatchingDescendant(ElementReference element, string selector, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the scroll element into view nearest operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ScrollElementIntoViewNearest(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the register one time password input operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="dotNetReference">The dot net reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RegisterOneTimePasswordInput(ElementReference element, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the unregister one time password input operation.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnregisterOneTimePasswordInput(ElementReference element, CancellationToken cancellationToken = default);
}
