using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public sealed class BradixPresence : BradixComponent, IAsyncDisposable
{
    [Inject]
    public IBradixSuiteInterop Interop { get; set; } = null!;

    [Parameter]
    public bool Present { get; set; }

    [Parameter]
    public string Tag { get; set; } = "div";

    [Parameter]
    public EventCallback OnExitComplete { get; set; }

    [Parameter]
    public EventCallback<ElementReference> OnElementReferenceCaptured { get; set; }

    [Parameter]
    public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

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
    private bool _forceExitAnimationFillModeForwards;
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
            _forceExitAnimationFillModeForwards = false;
        }
        else if (_rendered)
        {
            _pendingExitEvaluation = true;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
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
                await Interop.RegisterPresence(_element, _dotNetReference);
                _registered = true;
            }

            if (_pendingExitEvaluation)
            {
                _pendingExitEvaluation = false;
                BradixPresenceSnapshot snapshot = await Interop.GetPresenceState(_element);

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
            await Interop.UnregisterPresence(_element);
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
        builder.AddElementReferenceCapture(4, element => _element = element);
        builder.AddContent(5, ChildContent);
        builder.CloseElement();
    }

    public async ValueTask DisposeAsync()
    {
        if (_registered)
            await Interop.UnregisterPresence(_element);

        _registered = false;
        _dotNetReference?.Dispose();
    }

    [JSInvokable]
    public Task HandleAnimationStart(string animationName, string? currentAnimationName = null)
    {
        _previousAnimationName = NormalizeAnimationName(currentAnimationName, animationName);
        return Task.CompletedTask;
    }

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

        _forceExitAnimationFillModeForwards = true;
        await InvokeAsync(StateHasChanged);
        await Task.Yield();
        await CompleteUnmount();
    }

    private async Task CompleteUnmount()
    {
        _exitSuspended = false;

        if (_registered)
        {
            await Interop.UnregisterPresence(_element);
            _registered = false;
        }

        _forceExitAnimationFillModeForwards = false;
        _rendered = false;

        if (OnExitComplete.HasDelegate)
            await OnExitComplete.InvokeAsync();

        await InvokeAsync(StateHasChanged);
    }

    private Dictionary<string, object> BuildRenderAttributes()
    {
        Dictionary<string, object> attributes = BuildAttributes();

        if (_forceExitAnimationFillModeForwards)
        {
            if (attributes.TryGetValue("style", out object? style) && style is string styleValue && !string.IsNullOrWhiteSpace(styleValue))
                attributes["style"] = $"{styleValue.TrimEnd(';')}; animation-fill-mode: forwards;";
            else
                attributes["style"] = "animation-fill-mode: forwards;";
        }

        return attributes;
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

        string[] currentAnimations = currentAnimationName.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return currentAnimations.Any(name => string.Equals(name, eventAnimationName, StringComparison.Ordinal));
    }
}
