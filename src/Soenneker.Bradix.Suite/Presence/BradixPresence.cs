using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Soenneker.Bradix;

public sealed class BradixPresence : BradixComponentBase, IAsyncDisposable
{
    [Inject]
    public BradixSuiteInterop Interop { get; set; } = null!;

    [Parameter]
    public bool Present { get; set; }

    [Parameter]
    public string Tag { get; set; } = "div";

    [Parameter]
    public EventCallback OnExitComplete { get; set; }

    [Parameter]
    public EventCallback<ElementReference> OnElementReferenceCaptured { get; set; }

    private ElementReference _element;
    private DotNetObjectReference<object>? _dotNetReference;
    private bool _registered;
    private bool _rendered;
    private bool _initialized;
    private bool _pendingExitEvaluation;
    private bool _exitSuspended;
    private bool _elementReferenceCaptured;
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
                    await CompleteUnmountAsync();
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
        builder.AddMultipleAttributes(1, BuildAttributes());
        builder.AddElementReferenceCapture(2, element => _element = element);
        builder.AddContent(3, ChildContent);
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
    public Task HandleAnimationStartAsync(string animationName)
    {
        _previousAnimationName = string.IsNullOrWhiteSpace(animationName) ? "none" : animationName;
        return Task.CompletedTask;
    }

    [JSInvokable]
    public async Task HandleAnimationEndAsync(string animationName)
    {
        _previousAnimationName = string.IsNullOrWhiteSpace(animationName) ? _previousAnimationName : animationName;

        if (!_exitSuspended || Present)
            return;

        await CompleteUnmountAsync();
    }

    private async Task CompleteUnmountAsync()
    {
        _exitSuspended = false;

        if (_registered)
        {
            await Interop.UnregisterPresence(_element);
            _registered = false;
        }

        _rendered = false;

        if (OnExitComplete.HasDelegate)
            await OnExitComplete.InvokeAsync();

        await InvokeAsync(StateHasChanged);
    }
}
