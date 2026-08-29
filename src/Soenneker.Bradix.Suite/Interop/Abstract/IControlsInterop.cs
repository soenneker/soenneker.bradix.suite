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
    /// Initializes the Controls so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested javaScript Object Reference.</returns>
    ValueTask<IJSObjectReference> Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes checkbox Bubble Input State.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="isChecked">Whether checked.</param>
    /// <param name="isIndeterminate">Whether indeterminate.</param>
    /// <param name="dispatchEvent">Whether dispatch event.</param>
    /// <param name="bubbles">Whether bubbles.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the sync checkbox bubble input state operation is complete.</returns>
    ValueTask SyncCheckboxBubbleInputState(ElementReference element, bool isChecked, bool isIndeterminate, bool dispatchEvent, bool bubbles = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Clicks element.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the click element operation is complete.</returns>
    ValueTask ClickElement(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Focuses element Deferred.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the focus element deferred operation is complete.</returns>
    ValueTask FocusElementDeferred(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Selects input Text.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the select input text operation is complete.</returns>
    ValueTask SelectInputText(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes input Value.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="value">CSS value used to construct the utility class.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the sync input value operation is complete.</returns>
    ValueTask SyncInputValue(ElementReference element, string? value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the Controls direction Rtl.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>true if the Controls direction Rtl; otherwise, false.</returns>
    ValueTask<bool> IsDirectionRtl(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers slider Pointer Bridge for the Controls.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the slider pointer bridge registration is complete.</returns>
    ValueTask RegisterSliderPointerBridge(ElementReference element, DotNetObjectReference<object> dotNetReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters slider Pointer Bridge for the Controls.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the slider pointer bridge registration has been removed.</returns>
    ValueTask UnregisterSliderPointerBridge(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes slider Bubble Input Value.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="value">CSS value used to construct the utility class.</param>
    /// <param name="dispatchEvent">Whether dispatch event.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the sync slider bubble input value operation is complete.</returns>
    ValueTask SyncSliderBubbleInputValue(ElementReference element, double value, bool dispatchEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers select Bubble Input for the Controls.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the select bubble input registration is complete.</returns>
    ValueTask RegisterSelectBubbleInput(ElementReference element, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters select Bubble Input for the Controls.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the select bubble input registration has been removed.</returns>
    ValueTask UnregisterSelectBubbleInput(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes select Bubble Input Value.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="value">CSS value used to construct the utility class.</param>
    /// <param name="dispatchEvent">Whether dispatch event.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the sync select bubble input value operation is complete.</returns>
    ValueTask SyncSelectBubbleInputValue(ElementReference element, string? value, bool dispatchEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Captures pointer.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="pointerId">Identifier of the pointer to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the capture pointer operation is complete.</returns>
    ValueTask CapturePointer(ElementReference element, long pointerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Releases pointer for the Controls.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="pointerId">Identifier of the pointer to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the release pointer operation is complete.</returns>
    ValueTask ReleasePointer(ElementReference element, long pointerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Suppresses next Click.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the suppress next click operation is complete.</returns>
    ValueTask SuppressNextClick(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Focuses element By ID.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the focus element by id operation is complete.</returns>
    ValueTask FocusElementById(string? elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Focuses element By ID Deferred.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the focus element by id deferred operation is complete.</returns>
    ValueTask FocusElementByIdDeferred(string? elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Focuses element Prevent Scroll.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the focus element prevent scroll operation is complete.</returns>
    ValueTask FocusElementPreventScroll(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Focuses first Matching Descendant.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="selector">CSS selector used by the variant.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>true if focuses first Matching Descendant; otherwise, false.</returns>
    ValueTask<bool> FocusFirstMatchingDescendant(ElementReference element, string selector, CancellationToken cancellationToken = default);

    /// <summary>
    /// Scrolls element Into View Nearest for the Controls.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the scroll element into view nearest operation is complete.</returns>
    ValueTask ScrollElementIntoViewNearest(ElementReference element, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers one Time Password Input for the Controls.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the one time password input registration is complete.</returns>
    ValueTask RegisterOneTimePasswordInput(ElementReference element, DotNetObjectReference<object> dotNetReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters one Time Password Input for the Controls.
    /// </summary>
    /// <param name="element">DOM element to inspect or update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the one time password input registration has been removed.</returns>
    ValueTask UnregisterOneTimePasswordInput(ElementReference element, CancellationToken cancellationToken = default);
}
