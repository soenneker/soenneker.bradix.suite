using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

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

    [Fact]
    public void Focus_scope_renders_with_negative_tabindex()
    {
        var cut = Render(CreateFocusScope());

        var scope = cut.Find("[tabindex='-1']");
        Assert.NotNull(scope);
    }

    [Fact]
    public async Task Focus_scope_mount_callback_can_be_invoked()
    {
        var cut = Render(CreateFocusScope(EventCallback.Factory.Create(this, () => _mounted = true)));
        var scope = cut.FindComponent<BradixFocusScope>();

        await scope.Instance.HandleMountAutoFocus();

        Assert.True(_mounted);
    }

    [Fact]
    public async Task Focus_scope_unmount_callback_can_be_invoked()
    {
        var cut = Render(CreateFocusScope(default, EventCallback.Factory.Create(this, () => _unmounted = true)));
        var scope = cut.FindComponent<BradixFocusScope>();

        await scope.Instance.HandleUnmountAutoFocus();

        Assert.True(_unmounted);
    }

    [Fact]
    public async Task Focus_scope_disposal_invokes_unmount_callback_before_unregistering()
    {
        var cut = Render(CreateFocusScope(default, EventCallback.Factory.Create(this, () => _unmounted = true)));
        var scope = cut.FindComponent<BradixFocusScope>();

        await scope.Instance.DisposeAsync();

        Assert.True(_unmounted);

        var invocation = _module.Invocations.Last(call => call.Identifier == "unregisterFocusScope");

        Assert.Equal(2, invocation.Arguments.Count);
        Assert.Equal(false, invocation.Arguments[1]);
    }

    [Fact]
    public void Focus_scope_accepts_loop_and_trapped_parameters()
    {
        var cut = Render(builder =>
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

        Assert.Contains("Focusable", cut.Markup);
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
