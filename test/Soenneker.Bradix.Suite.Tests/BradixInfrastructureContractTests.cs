using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixInfrastructureContractTests : BunitContext
{
    [Test]
    public async Task Base_component_merges_common_attributes_with_consumer_precedence()
    {
        var component = new BradixExposedComponent();
        component.Configure(
            id: "owned-id",
            @class: "owned-class",
            style: "color: red",
            additionalAttributes: new Dictionary<string, object>
            {
                ["id"] = "consumer-id",
                ["class"] = "consumer-class",
                ["style"] = "background: blue",
                ["data-owned"] = "consumer"
            });

        Dictionary<string, object> attributes = component.ExposeBuildAttributes(("data-owned", "owned"), ("role", "presentation"));

        await Assert.That(attributes["id"]).IsEqualTo("consumer-id");
        await Assert.That(attributes["class"].ToString()).IsEqualTo("owned-class consumer-class");
        await Assert.That(attributes["style"].ToString()).IsEqualTo("color: red; background: blue");
        await Assert.That(attributes["data-owned"]).IsEqualTo("consumer");
        await Assert.That(attributes["role"]).IsEqualTo("presentation");
    }

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
        await Assert.That(Services.GetRequiredService<IBradixSuiteInterop>()).IsNotNull();
        await Assert.That(Services.GetServices<IBradixSuiteInterop>()).Count().IsEqualTo(1);
    }

    [Test]
    public async Task Static_web_assets_include_required_bradix_and_floating_ui_modules()
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
            "js/bradix/tooltip.js",
            "js/vendor/floating-ui.core.umd.min.js",
            "js/vendor/floating-ui.dom.umd.min.js"
        ];

        foreach (string asset in requiredAssets)
        {
            string path = Path.Combine(root, asset.Replace('/', Path.DirectorySeparatorChar));
            await Assert.That(File.Exists(path)).IsTrue();
            await Assert.That(new FileInfo(path).Length).IsGreaterThan(0);
        }
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
