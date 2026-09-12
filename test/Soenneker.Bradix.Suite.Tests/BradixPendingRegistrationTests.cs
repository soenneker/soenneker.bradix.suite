using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixPendingRegistrationTests : BunitContext
{
    [Test]
    public async Task Toast_viewport_releases_registration_that_completes_after_disposal()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        var register = module.SetupVoid("registerToastViewport", _ => true);
        var unregister = module.SetupVoid("unregisterToastViewport", _ => true);
        unregister.SetVoidResult();
        Services.AddBradixTestInterops();
        var cut = Render<BradixToastViewport>();
        await Assert.That(register.Invocations.Count).IsEqualTo(1);
        await cut.InvokeAsync(async () => await cut.Instance.DisposeAsync());
        register.SetVoidResult();
        await cut.WaitForAssertionAsync(async () => await Assert.That(unregister.Invocations.Count).IsEqualTo(1));
        await cut.InvokeAsync(async () => await cut.Instance.DisposeAsync());
        await Assert.That(unregister.Invocations.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Slider_releases_registration_that_completes_after_disposal()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        var register = module.SetupVoid("registerSliderPointerBridge", _ => true);
        var unregister = module.SetupVoid("unregisterSliderPointerBridge", _ => true);
        unregister.SetVoidResult();
        Services.AddBradixTestInterops();
        var cut = Render<BradixSlider>();
        await Assert.That(register.Invocations.Count).IsEqualTo(1);
        await cut.InvokeAsync(async () => await cut.Instance.DisposeAsync());
        register.SetVoidResult();
        await cut.WaitForAssertionAsync(async () => await Assert.That(unregister.Invocations.Count).IsEqualTo(1));
        await cut.InvokeAsync(async () => await cut.Instance.DisposeAsync());
        await Assert.That(unregister.Invocations.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Form_releases_registration_that_completes_after_disposal()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        var register = module.SetupVoid("registerFormRoot", _ => true);
        var unregister = module.SetupVoid("unregisterFormRoot", _ => true);
        unregister.SetVoidResult();
        Services.AddBradixTestInterops();
        var cut = Render<BradixForm>();
        await Assert.That(register.Invocations.Count).IsEqualTo(1);
        await cut.InvokeAsync(async () => await cut.Instance.DisposeAsync());
        register.SetVoidResult();
        await cut.WaitForAssertionAsync(async () => await Assert.That(unregister.Invocations.Count).IsEqualTo(1));
    }

    [Test]
    public async Task Checkbox_releases_form_registration_that_completes_after_disposal()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        var register = module.SetupVoid("registerCheckboxRoot", _ => true);
        var unregister = module.SetupVoid("unregisterCheckboxRoot", _ => true);
        unregister.SetVoidResult();
        Services.AddBradixTestInterops();
        var cut = Render<BradixCheckbox>();
        await Assert.That(register.Invocations.Count).IsEqualTo(1);
        await cut.InvokeAsync(async () => await cut.Instance.DisposeAsync());
        register.SetVoidResult();
        await cut.WaitForAssertionAsync(async () => await Assert.That(unregister.Invocations.Count).IsEqualTo(1));
    }

    [Test]
    public async Task Viewport_releases_registration_that_completes_after_disposal()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerScrollAreaRoot", _ => true).SetVoidResult();
        module.SetupVoid("unregisterScrollAreaRoot", _ => true).SetVoidResult();
        var register = module.SetupVoid("registerScrollAreaViewport", _ => true);
        var unregister = module.SetupVoid("unregisterScrollAreaViewport", _ => true);
        unregister.SetVoidResult();
        Services.AddBradixTestInterops();
        var cut = Render<BradixScrollArea>(p => p.Add(c => c.ChildContent, (RenderFragment)(builder =>
        {
            builder.OpenComponent<BradixScrollAreaViewport>(0);
            builder.CloseComponent();
        })));
        await Assert.That(register.Invocations.Count).IsEqualTo(1);
        var viewport = cut.FindComponent<BradixScrollAreaViewport>();
        await viewport.InvokeAsync(async () => await viewport.Instance.DisposeAsync());
        register.SetVoidResult();
        await cut.WaitForAssertionAsync(async () => await Assert.That(unregister.Invocations.Count).IsEqualTo(1));
    }

    [Test]
    public async Task Scrollbar_releases_registration_that_completes_after_disposal()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerScrollAreaRoot", _ => true).SetVoidResult();
        module.SetupVoid("unregisterScrollAreaRoot", _ => true).SetVoidResult();
        module.SetupVoid("registerScrollAreaViewport", _ => true).SetVoidResult();
        module.SetupVoid("unregisterScrollAreaViewport", _ => true).SetVoidResult();
        var register = module.SetupVoid("registerScrollAreaScrollbar", _ => true);
        var unregister = module.SetupVoid("unregisterScrollAreaScrollbar", _ => true);
        unregister.SetVoidResult();
        Services.AddBradixTestInterops();
        var cut = Render<BradixScrollArea>(p => p.Add(c => c.Type, ScrollAreaType.Always)
            .Add(c => c.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<BradixScrollAreaViewport>(0);
                builder.CloseComponent();
                builder.OpenComponent<BradixScrollAreaScrollbar>(1);
                builder.CloseComponent();
            })));
        await Assert.That(register.Invocations.Count).IsEqualTo(1);
        var scrollbar = cut.FindComponent<BradixScrollAreaScrollbar>();
        await scrollbar.InvokeAsync(async () => await scrollbar.Instance.DisposeAsync());
        register.SetVoidResult();
        await cut.WaitForAssertionAsync(async () => await Assert.That(unregister.Invocations.Count).IsEqualTo(1));
    }

    [Test]
    public async Task Portal_unmounts_when_mount_completes_after_disposal()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        var mount = module.SetupVoid("mountPortal", _ => true);
        var unmount = module.SetupVoid("unmountPortal", _ => true);
        unmount.SetVoidResult();
        Services.AddBradixTestInterops();
        var cut = Render<BradixPortal>();
        await cut.InvokeAsync(async () => await cut.Instance.DisposeAsync());
        mount.SetVoidResult();
        await cut.WaitForAssertionAsync(async () => await Assert.That(unmount.Invocations.Count).IsEqualTo(1));
        await cut.InvokeAsync(async () => await cut.Instance.DisposeAsync());
        await Assert.That(unmount.Invocations.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Focus_guards_release_registration_that_completes_after_disposal()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        var register = module.SetupVoid("registerFocusGuards", _ => true);
        var unregister = module.SetupVoid("unregisterFocusGuards", _ => true);
        unregister.SetVoidResult();
        Services.AddBradixTestInterops();
        var cut = Render<BradixFocusGuards>();
        await cut.InvokeAsync(async () => await cut.Instance.DisposeAsync());
        register.SetVoidResult();
        await cut.WaitForAssertionAsync(async () => await Assert.That(unregister.Invocations.Count).IsEqualTo(1));
        await cut.InvokeAsync(async () => await cut.Instance.DisposeAsync());
        await Assert.That(unregister.Invocations.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Dismissable_branch_releases_registration_that_completes_after_disposal()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        var register = module.SetupVoid("registerDismissableLayerBranch", _ => true);
        var unregister = module.SetupVoid("unregisterDismissableLayerBranch", _ => true);
        unregister.SetVoidResult();
        Services.AddBradixTestInterops();
        var cut = Render<BradixDismissableLayerBranch>();
        await cut.InvokeAsync(async () => await cut.Instance.DisposeAsync());
        register.SetVoidResult();
        await cut.WaitForAssertionAsync(async () => await Assert.That(unregister.Invocations.Count).IsEqualTo(1));
        await cut.InvokeAsync(async () => await cut.Instance.DisposeAsync());
        await Assert.That(unregister.Invocations.Count).IsEqualTo(1);
    }
}
