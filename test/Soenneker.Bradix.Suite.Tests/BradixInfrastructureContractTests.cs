using System;
using System.IO;
using System.Threading.Tasks;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Soenneker.Blazor.Interops.Floating.Abstract;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Bradix.Configuration;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixInfrastructureContractTests : BunitContext
{
    [Test]
    public async Task Shared_enum_tokens_match_radix_string_contracts()
    {
        await Assert.That(Alignment.Start.Value).IsEqualTo("start");
        await Assert.That(Alignment.Center.Value).IsEqualTo("center");
        await Assert.That(Alignment.End.Value).IsEqualTo("end");
        await Assert.That(Orientation.Horizontal.Value).IsEqualTo("horizontal");
        await Assert.That(Orientation.Vertical.Value).IsEqualTo("vertical");
        await Assert.That(Soenneker.Bradix.Side.Top.Value).IsEqualTo("top");
        await Assert.That(Soenneker.Bradix.Side.Right.Value).IsEqualTo("right");
        await Assert.That(Soenneker.Bradix.Side.Bottom.Value).IsEqualTo("bottom");
        await Assert.That(Soenneker.Bradix.Side.Left.Value).IsEqualTo("left");
        await Assert.That(SelectPosition.ItemAligned.Value).IsEqualTo("item-aligned");
        await Assert.That(SelectPosition.Popper.Value).IsEqualTo("popper");
        await Assert.That(SelectionMode.Single.Value).IsEqualTo("single");
        await Assert.That(SelectionMode.Multiple.Value).IsEqualTo("multiple");
        await Assert.That(ScrollAreaType.Hover.Value).IsEqualTo("hover");
        await Assert.That(ScrollAreaType.Scroll.Value).IsEqualTo("scroll");
        await Assert.That(ScrollAreaType.Auto.Value).IsEqualTo("auto");
        await Assert.That(ScrollAreaType.Always.Value).IsEqualTo("always");
        await Assert.That(SwipeDirection.Up.Value).IsEqualTo("up");
        await Assert.That(SwipeDirection.Down.Value).IsEqualTo("down");
        await Assert.That(SwipeDirection.Left.Value).IsEqualTo("left");
        await Assert.That(SwipeDirection.Right.Value).IsEqualTo("right");
        await Assert.That(TabsActivationMode.Automatic.Value).IsEqualTo("automatic");
        await Assert.That(TabsActivationMode.Manual.Value).IsEqualTo("manual");
        await Assert.That(ToastType.Foreground.Value).IsEqualTo("foreground");
        await Assert.That(ToastType.Background.Value).IsEqualTo("background");
    }

    [Test]
    public async Task Registrar_adds_resource_loader_and_bradix_interop_once()
    {
        Services.AddBradixSuiteAsScoped();
        Services.AddBradixSuiteAsScoped();

        await Assert.That(Services.GetRequiredService<IResourceLoader>()).IsNotNull();
        await Assert.That(Services.GetRequiredService<IFloatingUiInterop>()).IsNotNull();
        await Assert.That(Services.GetRequiredService<IBradixSuiteInterop>()).IsNotNull();
        await Assert.That(Services.GetServices<IBradixSuiteInterop>()).Count().IsEqualTo(1);
        await Assert.That(Services.GetServices<IFloatingUiInterop>()).Count().IsEqualTo(1);
        await Assert.That(Services.GetRequiredService<IOptions<BradixSuiteOptions>>().Value.UseCdn).IsFalse();
    }

    [Test]
    public async Task Registrar_can_configure_bradix_suite_options()
    {
        Services.AddBradixSuiteAsScoped(options => options.UseCdn = true);

        await Assert.That(Services.GetRequiredService<IOptions<BradixSuiteOptions>>().Value.UseCdn).IsTrue();
    }

    [Test]
    public async Task Static_web_assets_include_required_bradix_modules()
    {
        string root = Path.Combine(FindRepositoryRoot(), "src", "Soenneker.Bradix.Suite", "wwwroot");

        string[] requiredAssets =
        [
            "js/bradix.js",
            "js/bradix/popper.js",
            "js/bradix/portal.js",
            "js/bradix/dismissableLayer.js",
            "js/bradix/focusScope.js",
            "js/bradix/rovingFocus.js",
            "js/bradix/menu.js",
            "js/bradix/select.js",
            "js/bradix/tooltip.js"
        ];

        foreach (string asset in requiredAssets)
        {
            string path = Path.Combine(root, asset.Replace('/', Path.DirectorySeparatorChar));
            await Assert.That(File.Exists(path)).IsTrue();
            await Assert.That(new FileInfo(path).Length).IsGreaterThan(0);
        }
    }

    [Test]
    public async Task Form_invalid_capture_coalesces_dispatches_without_swallowing_callback_failures()
    {
        string path = Path.Combine(FindRepositoryRoot(), "src", "Soenneker.Bradix.Suite", "wwwroot", "js", "bradix", "forms.js");
        string source = await File.ReadAllTextAsync(path);

        await Assert.That(source).Contains("invalidDispatchQueued");
        await Assert.That(source).Contains("queueMicrotask(dispatchInvalidControls)");
        await Assert.That(source).Contains("seenControlNames");
        await Assert.That(source).Contains("isDisposedDotNetReferenceError");
        await Assert.That(source).Contains("throw error;");
        await Assert.That(source).DoesNotContain(".catch(() => {})");
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Soenneker.Bradix.Suite.slnx")))
                return directory.FullName;

            directory = directory.Parent;
        }

        return Directory.GetCurrentDirectory();
    }

}
