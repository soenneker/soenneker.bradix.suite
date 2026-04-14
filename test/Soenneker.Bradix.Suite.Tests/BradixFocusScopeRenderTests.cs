using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixFocusScopeRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixFocusScopeRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("updateFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusScope", _ => true).SetVoidResult();

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Focus_scope_renders_with_negative_tabindex()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateFocusScope());

        IElement scope = cut.Find("[tabindex='-1']");
        await Assert.That(scope).IsNotNull();
    }

    [Test]
    public async Task Focus_scope_mount_callback_can_be_invoked()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateFocusScope(EventCallback.Factory.Create(this, () => _mounted = true)));
        IRenderedComponent<BradixFocusScope> scope = cut.FindComponent<BradixFocusScope>();

        await scope.Instance.HandleMountAutoFocus();

        await Assert.That(_mounted).IsTrue();
    }

    [Test]
    public async Task Focus_scope_unmount_callback_can_be_invoked()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateFocusScope(default, EventCallback.Factory.Create(this, () => _unmounted = true)));
        IRenderedComponent<BradixFocusScope> scope = cut.FindComponent<BradixFocusScope>();

        await scope.Instance.HandleUnmountAutoFocus();

        await Assert.That(_unmounted).IsTrue();
    }

    [Test]
    public async Task Focus_scope_disposal_invokes_unmount_callback_before_unregistering()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateFocusScope(default, EventCallback.Factory.Create(this, () => _unmounted = true)));
        IRenderedComponent<BradixFocusScope> scope = cut.FindComponent<BradixFocusScope>();

        await scope.Instance.DisposeAsync();

        await Assert.That(_unmounted).IsTrue();

        JSRuntimeInvocation invocation = _module.Invocations.Last(call => call.Identifier == "unregisterFocusScope");

        await Assert.That(invocation.Arguments.Count).IsEqualTo(2);
        await Assert.That(invocation.Arguments[1]).IsEqualTo(false);
    }

    [Test]
    public async Task Focus_scope_accepts_loop_and_trapped_parameters()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixFocusScope>(0);
            builder.AddAttribute(1, nameof(BradixFocusScope.Loop), true);
            builder.AddAttribute(2, nameof(BradixFocusScope.Trapped), true);
            builder.AddAttribute(3, nameof(BradixFocusScope.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenElement(0, "button");
                contentBuilder.AddContent(1, "Focusable");
                contentBuilder.CloseElement();
            }));
            builder.CloseComponent();
        });

        await Assert.That(cut.Markup).Contains("Focusable");
    }

    private bool _mounted;
    private bool _unmounted;

    private static RenderFragment CreateFocusScope(EventCallback onMountAutoFocus = default, EventCallback onUnmountAutoFocus = default)
    {
        return builder =>
        {
            builder.OpenComponent<BradixFocusScope>(0);
            if (onMountAutoFocus.HasDelegate)
                builder.AddAttribute(1, nameof(BradixFocusScope.OnMountAutoFocus), onMountAutoFocus);
            if (onUnmountAutoFocus.HasDelegate)
                builder.AddAttribute(2, nameof(BradixFocusScope.OnUnmountAutoFocus), onUnmountAutoFocus);
            builder.AddAttribute(3, nameof(BradixFocusScope.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenElement(0, "button");
                contentBuilder.AddContent(1, "Focusable");
                contentBuilder.CloseElement();
            }));
            builder.CloseComponent();
        };
    }
}