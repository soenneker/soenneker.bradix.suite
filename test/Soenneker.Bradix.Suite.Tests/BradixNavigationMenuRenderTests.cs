using Bunit;
using Bunit.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Bradix.Suite.Id;
using Soenneker.Bradix.Suite.Interop;
using Soenneker.Bradix.Suite.NavigationMenu;
using Soenneker.Bradix.Suite.Presence;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixNavigationMenuRenderTests : Bunit.BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixNavigationMenuRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("updateDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("registerPresence", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterPresence", _ => true).SetVoidResult();
        _module.SetupVoid("registerNavigationMenuIndicator", _ => true).SetVoidResult();
        _module.SetupVoid("updateNavigationMenuIndicator", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterNavigationMenuIndicator", _ => true).SetVoidResult();
        _module.SetupVoid("registerNavigationMenuViewport", _ => true).SetVoidResult();
        _module.SetupVoid("updateNavigationMenuViewport", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterNavigationMenuViewport", _ => true).SetVoidResult();
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
    }

    [Fact]
    public void Trigger_click_toggles_associated_content_and_links_ids()
    {
        var cut = Render(CreateNavigationMenu());
        cut.FindAll("button").First(button => button.TextContent.Contains("Products")).Click();

        cut.WaitForAssertion(() =>
        {
            var updatedTrigger = cut.FindAll("button").First(button => button.TextContent.Contains("Products"));
            var content = cut.Find("[aria-labelledby]");
            Assert.Equal("true", updatedTrigger.GetAttribute("aria-expanded"));
            Assert.Equal(content.Id, updatedTrigger.GetAttribute("aria-controls"));
            Assert.Equal(updatedTrigger.Id, content.GetAttribute("aria-labelledby"));
        });
    }

    [Fact]
    public void Arrow_right_moves_roving_tab_stop()
    {
        var cut = Render(CreateNavigationMenu());
        var triggers = cut.FindAll("button");

        triggers[0].KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        cut.WaitForAssertion(() =>
        {
            var updatedTriggers = cut.FindAll("button");
            Assert.Equal("-1", updatedTriggers[0].GetAttribute("tabindex"));
            Assert.Equal("0", updatedTriggers[1].GetAttribute("tabindex"));
        });
    }

    [Fact]
    public void Pointer_move_opens_delayed_content_when_delay_is_zero()
    {
        var cut = Render(CreateNavigationMenu());
        var trigger = cut.FindAll("button").First(button => button.TextContent.Contains("Docs"));

        trigger.TriggerEvent("onpointermove", new PointerEventArgs { PointerType = "mouse" });

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Getting started", cut.Markup);
        });
    }

    [Fact]
    public void Link_click_closes_open_content_and_sets_active_state()
    {
        var cut = Render(CreateNavigationMenu());
        var trigger = cut.FindAll("button").First(button => button.TextContent.Contains("Products"));
        trigger.Click();

        cut.WaitForAssertion(() => Assert.Contains("Buttons", cut.Markup));

        var link = cut.FindAll("a").First(anchor => anchor.TextContent.Contains("Buttons"));
        link.Click();

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Buttons", cut.Markup);
            Assert.Equal("page", link.GetAttribute("aria-current"));
        });
    }

    [Fact]
    public async Task Indicator_updates_position_from_js_callback()
    {
        var cut = Render(CreateNavigationMenu(includeIndicator: true));
        cut.FindAll("button").First(button => button.TextContent.Contains("Products")).Click();
        var indicator = cut.FindComponent<BradixNavigationMenuIndicator>().Instance;

        await indicator.HandleIndicatorPositionChangedAsync(80, 24);

        cut.WaitForAssertion(() =>
        {
            var node = cut.Find("[aria-hidden='true'][data-state='visible']");
            Assert.Contains("width:80", node.GetAttribute("style"));
            Assert.Contains("translateX(24", node.GetAttribute("style"));
        });
    }

    [Fact]
    public async Task Viewport_renders_active_content_and_updates_size_vars()
    {
        var cut = Render(CreateNavigationMenu(includeViewport: true));
        cut.FindAll("button").First(button => button.TextContent.Contains("Products")).Click();
        var viewport = cut.FindComponent<BradixNavigationMenuViewport>().Instance;

        await viewport.HandleViewportSizeChangedAsync(320, 180);

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Buttons", cut.Markup);
            var viewportNode = cut.FindAll("[data-orientation='horizontal']").First(node =>
                (node.GetAttribute("style") ?? string.Empty).Contains("--radix-navigation-menu-viewport-width", System.StringComparison.Ordinal));
            string style = viewportNode.GetAttribute("style") ?? string.Empty;
            Assert.Contains("--radix-navigation-menu-viewport-width:320px", style);
            Assert.Contains("--radix-navigation-menu-viewport-height:180px", style);
        });
    }

    private static RenderFragment CreateNavigationMenu(bool includeIndicator = false, bool includeViewport = false)
    {
        return builder =>
        {
            builder.OpenComponent<BradixNavigationMenu>(0);
            builder.AddAttribute(1, nameof(BradixNavigationMenu.DelayDuration), 0);
            builder.AddAttribute(2, nameof(BradixNavigationMenu.Style), "position:relative;");
            builder.AddAttribute(3, nameof(BradixNavigationMenu.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixNavigationMenuList>(0);
                content.AddAttribute(1, nameof(BradixNavigationMenuList.ChildContent), (RenderFragment)(list =>
                {
                    BuildItem(list, 0, "products", "Products", ("Buttons", "buttons"), ("Inputs", "inputs"));
                    BuildItem(list, 100, "docs", "Docs", ("Getting started", "getting-started"), ("Components", "components"));
                    BuildItem(list, 200, "about", "About", ("About Bradix", "about"));
                }));
                content.CloseComponent();

                if (includeIndicator)
                {
                    content.OpenComponent<BradixNavigationMenuIndicator>(10);
                    content.CloseComponent();
                }

                if (includeViewport)
                {
                    content.OpenComponent<BradixNavigationMenuViewport>(20);
                    content.CloseComponent();
                }
            }));
            builder.CloseComponent();
        };
    }

    private static void BuildItem(RenderTreeBuilder builder, int sequence, string value, string label, params (string Text, string Key)[] links)
    {
        builder.OpenComponent<BradixNavigationMenuItem>(sequence);
        builder.AddAttribute(sequence + 1, nameof(BradixNavigationMenuItem.Value), value);
        builder.AddAttribute(sequence + 2, nameof(BradixNavigationMenuItem.ChildContent), (RenderFragment)(item =>
        {
            item.OpenComponent<BradixNavigationMenuTrigger>(0);
            item.AddAttribute(1, nameof(BradixNavigationMenuTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, label)));
            item.CloseComponent();

            item.OpenComponent<BradixNavigationMenuContent>(2);
            item.AddAttribute(3, nameof(BradixNavigationMenuContent.ChildContent), (RenderFragment)(menuContent =>
            {
                for (var index = 0; index < links.Length; index++)
                {
                    (string text, string key) = links[index];
                    menuContent.OpenComponent<BradixNavigationMenuLink>(index * 2);
                    menuContent.AddAttribute(index * 2 + 1, "href", $"#{key}");
                    menuContent.AddAttribute(index * 2 + 2, nameof(BradixNavigationMenuLink.Active), index == 0);
                    menuContent.AddAttribute(index * 2 + 3, nameof(BradixNavigationMenuLink.ChildContent), (RenderFragment)(link => link.AddContent(0, text)));
                    menuContent.CloseComponent();
                }
            }));
            item.CloseComponent();
        }));
        builder.CloseComponent();
    }
}
