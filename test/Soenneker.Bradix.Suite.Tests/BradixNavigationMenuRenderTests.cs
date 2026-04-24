using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixNavigationMenuRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixNavigationMenuRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("updateDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("registerDismissableLayerBranch", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayerBranch", _ => true).SetVoidResult();
        _module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("registerRovingFocusNavigationKeys", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterRovingFocusNavigationKeys", _ => true).SetVoidResult();
        _module.SetupVoid("registerPresence", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterPresence", _ => true).SetVoidResult();
        _module.SetupVoid("mountPortal", _ => true).SetVoidResult();
        _module.SetupVoid("unmountPortal", _ => true).SetVoidResult();
        _module.SetupVoid("registerNavigationMenuIndicator", _ => true).SetVoidResult();
        _module.SetupVoid("updateNavigationMenuIndicator", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterNavigationMenuIndicator", _ => true).SetVoidResult();
        _module.SetupVoid("registerNavigationMenuTriggerInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterNavigationMenuTriggerInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("registerNavigationMenuContentFocusBridge", _ => true).SetVoidResult();
        _module.SetupVoid("updateNavigationMenuContentFocusBridge", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterNavigationMenuContentFocusBridge", _ => true).SetVoidResult();
        _module.SetupVoid("registerNavigationMenuViewport", _ => true).SetVoidResult();
        _module.SetupVoid("updateNavigationMenuViewport", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterNavigationMenuViewport", _ => true).SetVoidResult();
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Trigger_click_toggles_associated_content_and_links_ids()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu());
        await cut.FindAll("button").First(button => button.TextContent.Contains("Products")).ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement updatedTrigger = cut.FindAll("button").First(button => button.TextContent.Contains("Products"));
            IElement content = cut.Find("[aria-labelledby]");
            await Assert.That(updatedTrigger.GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(updatedTrigger.GetAttribute("aria-controls")).IsEqualTo(content.Id);
            await Assert.That(content.GetAttribute("aria-labelledby")).IsEqualTo(updatedTrigger.Id);
        });
    }

    [Test]
    public async Task Root_navigation_menu_sets_default_label_and_positioning()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixNavigationMenu>(0);
            builder.AddAttribute(1, nameof(BradixNavigationMenu.DelayDuration), 0);
            builder.AddAttribute(2, nameof(BradixNavigationMenu.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixNavigationMenuList>(0);
                content.AddAttribute(1, nameof(BradixNavigationMenuList.ChildContent), (RenderFragment)(list =>
                {
                    BuildItem(list, 0, "products", "Products", ("Buttons", "buttons"));
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });

        IElement root = cut.Find("nav");
        string style = root.GetAttribute("style") ?? string.Empty;

        await Assert.That(root.GetAttribute("aria-label")).IsEqualTo("Main");
        await Assert.That(style.Replace(" ", string.Empty)).Contains("position:relative");
    }

    [Test]
    public async Task Root_navigation_menu_list_wraps_items_in_relative_indicator_track()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu(includeIndicator: true));

        IElement track = cut.FindAll("nav div")
                            .First(element => (element.GetAttribute("style") ?? string.Empty).Replace(" ", string.Empty).Contains("position:relative"));
        IElement? list = track.QuerySelector("ul");

        await Assert.That(list).IsNotNull();
        await Assert.That((track.GetAttribute("style") ?? string.Empty).Replace(" ", string.Empty)).Contains("position:relative");
    }

    [Test]
    public async Task Arrow_right_moves_roving_tab_stop()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu());
        IReadOnlyList<IElement> triggers = cut.FindAll("button");

        await triggers[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedTriggers = cut.FindAll("button");
            await Assert.That(updatedTriggers[0].GetAttribute("tabindex")).IsEqualTo("-1");
            await Assert.That(updatedTriggers[1].GetAttribute("tabindex")).IsEqualTo("0");
        });
    }

    [Test]
    public async Task Triggers_register_roving_key_handlers()
    {
        Render(CreateNavigationMenu());

        await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "registerRovingFocusNavigationKeys") >= 3).IsTrue();
    }

    [Test]
    public async Task Pointer_move_opens_delayed_content_when_delay_is_zero()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu());
        IElement trigger = cut.FindAll("button").First(button => button.TextContent.Contains("Docs"));

        await trigger.TriggerEventAsync("onpointermove", new PointerEventArgs { PointerType = "mouse" });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).Contains("Getting started");
        });
    }

    [Test]
    public async Task Pointer_move_does_not_reopen_immediately_after_click_close()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu());
        IElement trigger = cut.FindAll("button").First(button => button.TextContent.Contains("Docs"));

        await trigger.ClickAsync();
        await Assert.That(cut.Markup).Contains("Getting started");

        trigger = cut.FindAll("button").First(button => button.TextContent.Contains("Docs"));
        await trigger.ClickAsync();
        await Assert.That(cut.Markup).DoesNotContain("Getting started");

        trigger = cut.FindAll("button").First(button => button.TextContent.Contains("Docs"));
        await trigger.TriggerEventAsync("onpointermove", new PointerEventArgs { PointerType = "mouse" });

        await Assert.That(cut.Markup).DoesNotContain("Getting started");
    }

    [Test]
    public async Task Pointer_opened_root_item_stays_open_on_first_click_then_closes_on_repeat_click()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu());
        IElement productsTrigger = cut.FindAll("button").First(button => button.TextContent.Contains("Products"));
        IElement docsTrigger = cut.FindAll("button").First(button => button.TextContent.Contains("Docs"));

        await productsTrigger.ClickAsync();
        await Assert.That(cut.Markup).Contains("Buttons");

        docsTrigger = cut.FindAll("button").First(button => button.TextContent.Contains("Docs"));
        await docsTrigger.TriggerEventAsync("onpointermove", new PointerEventArgs { PointerType = "mouse" });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).Contains("Getting started");
            await Assert.That(cut.Markup).DoesNotContain("Buttons");
        });

        docsTrigger = cut.FindAll("button").First(button => button.TextContent.Contains("Docs"));
        await docsTrigger.ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement updatedDocsTrigger = cut.FindAll("button").First(button => button.TextContent.Contains("Docs"));
            await Assert.That(updatedDocsTrigger.GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(cut.Markup).Contains("Getting started");
        });

        docsTrigger = cut.FindAll("button").First(button => button.TextContent.Contains("Docs"));
        await docsTrigger.ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement updatedDocsTrigger = cut.FindAll("button").First(button => button.TextContent.Contains("Docs"));
            await Assert.That(updatedDocsTrigger.GetAttribute("aria-expanded")).IsEqualTo("false");
            await Assert.That(cut.Markup).DoesNotContain("Getting started");
        });
    }

    [Test]
    public async Task Controlled_root_menu_switches_between_triggers_on_click()
    {
        string? value = null;

        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixNavigationMenu>(0);
            builder.AddAttribute(1, nameof(BradixNavigationMenu.DelayDuration), 0);
            builder.AddAttribute(2, nameof(BradixNavigationMenu.Value), value);
            builder.AddAttribute(3, nameof(BradixNavigationMenu.ValueChanged),
                EventCallback.Factory.Create<string?>(this, next => value = string.IsNullOrWhiteSpace(next) ? null : next));
            builder.AddAttribute(4, nameof(BradixNavigationMenu.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixNavigationMenuList>(0);
                content.AddAttribute(1, nameof(BradixNavigationMenuList.ChildContent), (RenderFragment)(list =>
                {
                    BuildItem(list, 0, "products", "Products", ("Buttons", "buttons"));
                    BuildItem(list, 100, "docs", "Docs", ("Getting started", "getting-started"));
                }));
                content.CloseComponent();

                content.OpenComponent<BradixNavigationMenuViewport>(10);
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });

        await cut.FindAll("button").First(button => button.TextContent.Contains("Products")).ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement productsTrigger = cut.FindAll("button").First(button => button.TextContent.Contains("Products"));
            await Assert.That(productsTrigger.GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(value).IsEqualTo("products");
        });

        await cut.FindAll("button").First(button => button.TextContent.Contains("Docs")).ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement productsTrigger = cut.FindAll("button").First(button => button.TextContent.Contains("Products"));
            IElement docsTrigger = cut.FindAll("button").First(button => button.TextContent.Contains("Docs"));
            await Assert.That(productsTrigger.GetAttribute("aria-expanded")).IsEqualTo("false");
            await Assert.That(docsTrigger.GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(value).IsEqualTo("docs");
            await Assert.That(cut.Markup).Contains("Getting started");
        });
    }

    [Test]
    public async Task Pointer_move_does_not_reopen_immediately_after_escape_close()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu());
        IElement trigger = cut.FindAll("button").First(button => button.TextContent.Contains("Docs"));

        await trigger.ClickAsync();
        await Assert.That(cut.Markup).Contains("Getting started");

        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();
        await cut.InvokeAsync(() => layer.Instance.HandleEscapeKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "Escape"
        }));

        await Assert.That(cut.Markup).DoesNotContain("Getting started");

        trigger = cut.FindAll("button").First(button => button.TextContent.Contains("Docs"));
        await trigger.TriggerEventAsync("onpointermove", new PointerEventArgs { PointerType = "mouse" });

        await Assert.That(cut.Markup).DoesNotContain("Getting started");
    }

    [Test]
    public async Task Link_click_closes_open_content_and_sets_active_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu());
        IElement trigger = cut.FindAll("button").First(button => button.TextContent.Contains("Products"));
        await trigger.ClickAsync();

        await Assert.That(cut.Markup).Contains("Buttons");

        IElement link = cut.FindAll("a").First(anchor => anchor.TextContent.Contains("Buttons"));
        await link.ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).DoesNotContain("Buttons");
            await Assert.That(link.GetAttribute("aria-current")).IsEqualTo("page");
        });
    }

    [Test]
    public async Task Indicator_updates_position_from_js_callback()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu(includeIndicator: true));
        await cut.FindAll("button").First(button => button.TextContent.Contains("Products")).ClickAsync();
        BradixNavigationMenuIndicator indicator = cut.FindComponent<BradixNavigationMenuIndicator>().Instance;

        await indicator.HandleIndicatorPositionChanged(80, 24);

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement node = cut.Find("[aria-hidden='true'][data-state='visible']");
            await Assert.That(node.GetAttribute("style")).Contains("width:80");
            await Assert.That(node.GetAttribute("style")).Contains("translateX(24");
        });
    }

    [Test]
    public async Task Viewport_renders_active_content_and_updates_size_vars()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu(includeViewport: true));
        await cut.FindAll("button").First(button => button.TextContent.Contains("Products")).ClickAsync();
        BradixNavigationMenuViewport viewport = cut.FindComponent<BradixNavigationMenuViewport>().Instance;

        await viewport.HandleViewportSizeChanged(320, 180);

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).Contains("Buttons");
            IElement viewportNode = cut.FindAll("[data-orientation='horizontal']").First(node =>
                (node.GetAttribute("style") ?? string.Empty).Contains("--radix-navigation-menu-viewport-width", System.StringComparison.Ordinal));
            string style = viewportNode.GetAttribute("style") ?? string.Empty;
            await Assert.That(style).Contains("--radix-navigation-menu-viewport-width:320px");
            await Assert.That(style).Contains("--radix-navigation-menu-viewport-height:180px");
        });
    }

    [Test]
    public async Task Open_root_item_renders_focus_proxies_and_registers_focus_bridge()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu());
        await cut.FindAll("button").First(button => button.TextContent.Contains("Products")).ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerNavigationMenuContentFocusBridge")).IsTrue();
        });
    }

    [Test]
    public async Task Open_content_registers_link_roving_handlers()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu());
        await cut.FindAll("button").First(button => button.TextContent.Contains("Products")).ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).Contains("Buttons");
            await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "registerRovingFocusNavigationKeys") >= 2).IsTrue();
        });
    }

    [Test]
    public async Task Inline_content_does_not_emit_viewport_ownership_shim()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu());
        await cut.FindAll("button").First(button => button.TextContent.Contains("Products")).ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.FindAll("[aria-owns]")).IsEmpty();
        });
    }

    [Test]
    public async Task Viewport_content_registers_focus_bridge_against_active_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu(includeViewport: true));
        await cut.FindAll("button").First(button => button.TextContent.Contains("Products")).ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).Contains("Buttons");
            await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerNavigationMenuContentFocusBridge")).IsTrue();
        });
    }

    [Test]
    public async Task Viewport_content_registers_link_roving_handlers()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu(includeViewport: true));
        await cut.FindAll("button").First(button => button.TextContent.Contains("Products")).ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).Contains("Buttons");
            await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "registerRovingFocusNavigationKeys") >= 2).IsTrue();
        });
    }

    [Test]
    public async Task Viewport_content_emits_trigger_ownership_shim()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu(includeViewport: true));
        await cut.FindAll("button").First(button => button.TextContent.Contains("Products")).ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement trigger = cut.FindAll("button").First(button => button.TextContent.Contains("Products"));
            string? ownedId = cut.Find("[aria-owns]").GetAttribute("aria-owns");
            await Assert.That(ownedId).IsEqualTo(trigger.GetAttribute("aria-controls"));
        });
    }

    [Test]
    public async Task Viewport_pointer_down_outside_does_not_dismiss_when_target_is_root_viewport()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu(includeViewport: true));
        await cut.FindAll("button").First(button => button.TextContent.Contains("Products")).ClickAsync();

        await Assert.That(cut.Markup).Contains("Buttons");

        IElement viewport = cut.Find("[id$='-viewport']");
        await Assert.That(string.IsNullOrWhiteSpace(viewport.Id)).IsFalse();
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutside(new BradixDelegatedMouseEvent
        {
            AncestorIds = [viewport.Id]
        }));

        await Assert.That(cut.Markup).Contains("Buttons");
    }

    [Test]
    public async Task Viewport_focus_outside_does_not_dismiss_when_target_stays_inside_root_menu()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu(includeViewport: true));
        await cut.FindAll("button").First(button => button.TextContent.Contains("Products")).ClickAsync();

        await Assert.That(cut.Markup).Contains("Buttons");

        IElement root = cut.Find("nav");
        await Assert.That(string.IsNullOrWhiteSpace(root.Id)).IsFalse();
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();

        await cut.InvokeAsync(() => layer.Instance.HandleFocusOutside(new BradixDelegatedFocusEvent
        {
            AncestorIds = [root.Id]
        }));

        await Assert.That(cut.Markup).Contains("Buttons");
    }

    [Test]
    public async Task Viewport_switch_keeps_previous_content_mounted_for_exit_motion()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenu(includeViewport: true));
        await cut.FindAll("button").First(button => button.TextContent.Contains("Products")).ClickAsync();

        await Assert.That(cut.Markup).Contains("Buttons");

        await cut.FindAll("button").First(button => button.TextContent.Contains("Docs")).ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).Contains("Buttons");
            await Assert.That(cut.Markup).Contains("Getting started");
            await Assert.That(cut.FindAll("[data-motion]").Count >= 2).IsTrue();
        });
    }

    [Test]
    public async Task Sub_uses_default_active_item_and_does_not_toggle_closed_on_repeat_click()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenuWithSub());
        await cut.FindAll("button").First(button => button.TextContent.Contains("Guides")).ClickAsync();

        await Assert.That(cut.Markup).Contains("Overview content stays mounted.");

        await cut.FindAll("button").First(button => button.TextContent.Contains("Overview")).ClickAsync();

        await Assert.That(cut.Markup).Contains("Overview content stays mounted.");
    }

    [Test]
    public async Task Sub_link_click_does_not_dismiss_nested_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenuWithSub());
        await cut.FindAll("button").First(button => button.TextContent.Contains("Guides")).ClickAsync();

        await Assert.That(cut.Markup).Contains("Overview content stays mounted.");

        await cut.FindAll("a").First(anchor => anchor.TextContent.Contains("Overview content stays mounted.")).ClickAsync();

        await Assert.That(cut.Markup).Contains("Overview content stays mounted.");
    }

    [Test]
    public async Task Sub_pointer_move_switches_active_content_immediately()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenuWithSub());
        await cut.FindAll("button").First(button => button.TextContent.Contains("Guides")).ClickAsync();

        await Assert.That(cut.Markup).Contains("Overview content stays mounted.");

        await cut.FindAll("button").First(button => button.TextContent.Contains("API"))
                 .TriggerEventAsync("onpointermove", new PointerEventArgs { PointerType = "mouse" });

        await Assert.That(cut.Markup).Contains("API content swaps immediately.");
    }

    [Test]
    public async Task Sub_inherits_parent_direction_for_rtl_navigation()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateNavigationMenuWithSub("rtl"));
        await cut.FindAll("button").First(button => button.TextContent.Contains("Guides")).ClickAsync();

        IElement subRoot = cut.FindAll("div").First(element => string.Equals(element.GetAttribute("dir"), "rtl", System.StringComparison.Ordinal));
        await Assert.That(subRoot.GetAttribute("data-orientation")).IsEqualTo("horizontal");

        IElement overviewTrigger = cut.FindAll("button").First(button => button.TextContent.Contains("Overview"));
        await overviewTrigger.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement updatedOverview = cut.FindAll("button").First(button => button.TextContent.Contains("Overview"));
            IElement updatedApi = cut.FindAll("button").First(button => button.TextContent.Contains("API"));
            await Assert.That(updatedOverview.GetAttribute("tabindex")).IsEqualTo("-1");
            await Assert.That(updatedApi.GetAttribute("tabindex")).IsEqualTo("0");
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

    private static RenderFragment CreateNavigationMenuWithSub(string? dir = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixNavigationMenu>(0);
            builder.AddAttribute(1, nameof(BradixNavigationMenu.DelayDuration), 0);
            builder.AddAttribute(2, nameof(BradixNavigationMenu.Dir), dir);
            builder.AddAttribute(3, nameof(BradixNavigationMenu.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixNavigationMenuList>(0);
                content.AddAttribute(1, nameof(BradixNavigationMenuList.ChildContent), (RenderFragment)(list =>
                {
                    list.OpenComponent<BradixNavigationMenuItem>(0);
                    list.AddAttribute(1, nameof(BradixNavigationMenuItem.Value), "guides");
                    list.AddAttribute(2, nameof(BradixNavigationMenuItem.ChildContent), (RenderFragment)(item =>
                    {
                        item.OpenComponent<BradixNavigationMenuTrigger>(0);
                        item.AddAttribute(1, nameof(BradixNavigationMenuTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, "Guides")));
                        item.CloseComponent();

                        item.OpenComponent<BradixNavigationMenuContent>(2);
                        item.AddAttribute(3, nameof(BradixNavigationMenuContent.ChildContent), (RenderFragment)(menuContent =>
                        {
                            menuContent.OpenComponent<BradixNavigationMenuSub>(0);
                            menuContent.AddAttribute(1, nameof(BradixNavigationMenuSub.DefaultValue), "overview");
                            menuContent.AddAttribute(2, nameof(BradixNavigationMenuSub.ChildContent), (RenderFragment)(sub =>
                            {
                                sub.OpenComponent<BradixNavigationMenuList>(0);
                                sub.AddAttribute(1, nameof(BradixNavigationMenuList.ChildContent), (RenderFragment)(subList =>
                                {
                                    BuildItem(subList, 0, "overview", "Overview", ("Overview content stays mounted.", "overview"));
                                    BuildItem(subList, 100, "api", "API", ("API content swaps immediately.", "api"));
                                }));
                                sub.CloseComponent();
                            }));
                            menuContent.CloseComponent();
                        }));
                        item.CloseComponent();
                    }));
                    list.CloseComponent();
                }));
                content.CloseComponent();
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
