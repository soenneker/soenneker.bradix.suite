using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using Soenneker.Atomics.ValueBools;
using Soenneker.Lepton.Suite;

namespace Soenneker.Bradix;

/// <summary>
/// Represents the bradix presence.
/// </summary>
public sealed class BradixPresence : LeptonIdentifiableContentElement, IAsyncDisposable
{
    /// <summary>
    /// Gets or sets presence overlay interop.
    /// </summary>
    [Inject]
    public IPresenceOverlayInterop PresenceOverlayInterop { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether present.
    /// </summary>
    [Parameter]
    public bool Present { get; set; }

    /// <summary>
    /// Gets or sets tag.
    /// </summary>
    [Parameter]
    public string Tag { get; set; } = "div";

    /// <summary>
    /// Gets or sets on exit complete.
    /// </summary>
    [Parameter]
    public EventCallback OnExitComplete { get; set; }

    /// <summary>
    /// Gets or sets on element reference captured.
    /// </summary>
    [Parameter]
    public EventCallback<ElementReference> OnElementReferenceCaptured { get; set; }

    /// <summary>
    /// Gets or sets on key down.
    /// </summary>
    [Parameter]
    public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

    /// <summary>
    /// Gets or sets on pointer enter.
    /// </summary>
    [Parameter]
    public EventCallback<PointerEventArgs> OnPointerEnter { get; set; }

    /// <summary>
    /// Gets or sets on pointer leave.
    /// </summary>
    [Parameter]
    public EventCallback<PointerEventArgs> OnPointerLeave { get; set; }

    /// <summary>
    /// Gets or sets on pointer down.
    /// </summary>
    [Parameter]
    public EventCallback<PointerEventArgs> OnPointerDown { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether prevent key down default.
    /// </summary>
    [Parameter]
    public bool PreventKeyDownDefault { get; set; }

    private ElementReference _element;
    private DotNetObjectReference<object>? _dotNetReference;
    private bool _registered;
    private bool _rendered;
    private bool _initialized;
    private bool _pendingExitEvaluation;
    private bool _exitSuspended;
    private bool _elementReferenceCaptured;
    private ValueAtomicBool _disposed;
    private string _previousAnimationName = "none";

    protected override void OnParametersSet()
    {
        if (!_initialized)
        {
            _rendered = Present;
            _initialized = true;
            return;
        }

        if (Present)
        {
            _rendered = true;
            _pendingExitEvaluation = false;
            _exitSuspended = false;
        }
        else if (_rendered)
        {
            _pendingExitEvaluation = true;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_disposed.Read())
            return;

        if (_rendered)
        {
            if (!_elementReferenceCaptured)
            {
                _elementReferenceCaptured = true;

                if (OnElementReferenceCaptured.HasDelegate)
                    await OnElementReferenceCaptured.InvokeAsync(_element);
            }

            if (!_registered)
            {
                _dotNetReference ??= DotNetObjectReference.Create<object>(this);
                try
                {
                    await PresenceOverlayInterop.RegisterPresence(_element, _dotNetReference);
                    _registered = true;
                }
                catch (Exception ex) when (ShouldIgnoreInteropException(ex))
                {
                    return;
                }
            }

            if (_pendingExitEvaluation)
            {
                _pendingExitEvaluation = false;
                BradixPresenceSnapshot snapshot = await PresenceOverlayInterop.GetPresenceState(_element);

                bool hasExitAnimation = snapshot.Display != "none"
                                        && !string.Equals(snapshot.AnimationName, "none", StringComparison.Ordinal)
                                        && !string.Equals(snapshot.AnimationName, _previousAnimationName, StringComparison.Ordinal);

                if (hasExitAnimation)
                {
                    _exitSuspended = true;
                }
                else
                {
                    await CompleteUnmount();
                }
            }
        }
        else if (_registered)
        {
            try
            {
                await PresenceOverlayInterop.UnregisterPresence(_element);
            }
            catch (Exception ex) when (ShouldIgnoreInteropException(ex))
            {
            }

            _registered = false;
        }

        if (!_rendered)
            _elementReferenceCaptured = false;

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!_rendered)
            return;

        builder.OpenElement(0, string.IsNullOrWhiteSpace(Tag) ? "div" : Tag);
        builder.AddMultipleAttributes(1, BuildRenderAttributes());
        if (OnKeyDown.HasDelegate)
        {
            builder.AddAttribute(2, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDown));
            if (PreventKeyDownDefault)
                builder.AddEventPreventDefaultAttribute(3, "onkeydown", true);
        }
        if (OnPointerEnter.HasDelegate)
            builder.AddAttribute(4, "onpointerenter", OnPointerEnter);
        if (OnPointerLeave.HasDelegate)
            builder.AddAttribute(5, "onpointerleave", OnPointerLeave);
        if (OnPointerDown.HasDelegate)
            builder.AddAttribute(6, "onpointerdown", OnPointerDown);
        builder.AddElementReferenceCapture(7, element => _element = element);
        builder.AddContent(8, ChildContent);
        builder.CloseElement();
    }

    /// <summary>
    /// Asynchronously releases resources used by the current instance.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask DisposeAsync()
    {
        if (!_disposed.TrySetTrue())
            return;

        if (_registered)
        {
            try
            {
                await PresenceOverlayInterop.UnregisterPresence(_element);
            }
            catch (Exception ex) when (ShouldIgnoreInteropException(ex))
            {
            }
        }

        _registered = false;
        _dotNetReference?.Dispose();
        _dotNetReference = null;
    }

    /// <summary>
    /// Executes the handle animation start operation.
    /// </summary>
    /// <param name="animationName">The animation name.</param>
    /// <param name="currentAnimationName">The current animation name.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task HandleAnimationStart(string animationName, string? currentAnimationName = null)
    {
        _previousAnimationName = NormalizeAnimationName(currentAnimationName, animationName);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Executes the handle animation end operation.
    /// </summary>
    /// <param name="animationName">The animation name.</param>
    /// <param name="currentAnimationName">The current animation name.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public async Task HandleAnimationEnd(string animationName, string? currentAnimationName = null)
    {
        string normalizedEventAnimation = NormalizeAnimationName(animationName);
        string activeAnimationName = NormalizeAnimationName(currentAnimationName);

        if (!string.IsNullOrWhiteSpace(activeAnimationName) &&
            !string.Equals(activeAnimationName, "none", StringComparison.Ordinal) &&
            !MatchesCurrentAnimation(activeAnimationName, normalizedEventAnimation))
        {
            return;
        }

        _previousAnimationName = string.Equals(normalizedEventAnimation, "none", StringComparison.Ordinal)
            ? _previousAnimationName
            : normalizedEventAnimation;

        if (!_exitSuspended || Present)
            return;

        await InvokeAsync(CompleteUnmount);
    }

    private async Task CompleteUnmount()
    {
        _exitSuspended = false;

        if (_registered)
        {
            await PresenceOverlayInterop.UnregisterPresence(_element);
            _registered = false;
        }

        _rendered = false;

        if (OnExitComplete.HasDelegate)
            await OnExitComplete.InvokeAsync();

        await InvokeAsync(StateHasChanged);
    }

    private Dictionary<string, object> BuildRenderAttributes()
    {
        return BuildAttributes();
    }

    private Task HandleKeyDown(KeyboardEventArgs args)
    {
        return OnKeyDown.HasDelegate ? OnKeyDown.InvokeAsync(args) : Task.CompletedTask;
    }

    private static string NormalizeAnimationName(string? animationName, string? fallback = null)
    {
        string? value = string.IsNullOrWhiteSpace(animationName) ? fallback : animationName;
        return string.IsNullOrWhiteSpace(value) ? "none" : value;
    }

    private static bool MatchesCurrentAnimation(string currentAnimationName, string eventAnimationName)
    {
        if (string.Equals(eventAnimationName, "none", StringComparison.Ordinal))
            return false;

        ReadOnlySpan<char> remaining = currentAnimationName.AsSpan();

        while (!remaining.IsEmpty)
        {
            int separatorIndex = remaining.IndexOf(',');
            ReadOnlySpan<char> segment = separatorIndex < 0 ? remaining : remaining[..separatorIndex];
            segment = segment.Trim();

            if (!segment.IsEmpty && segment.Equals(eventAnimationName.AsSpan(), StringComparison.Ordinal))
                return true;

            if (separatorIndex < 0)
                break;

            remaining = remaining[(separatorIndex + 1)..];
        }

        return false;
    }

    private static bool ShouldIgnoreInteropException(Exception ex)
    {
        return ex is ObjectDisposedException or JSDisconnectedException;
    }
}
