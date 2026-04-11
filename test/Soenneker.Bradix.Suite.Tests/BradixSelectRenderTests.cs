using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixSelectRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixSelectRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("updateDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("registerPopperContent", _ => true).SetVoidResult();
        _module.SetupVoid("updatePopperContent", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterPopperContent", _ => true).SetVoidResult();
        _module.SetupVoid("registerSelectItemAlignedPosition", _ => true).SetVoidResult();
        _module.SetupVoid("updateSelectItemAlignedPosition", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterSelectItemAlignedPosition", _ => true).SetVoidResult();
        _module.SetupVoid("registerSelectViewport", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterSelectViewport", _ => true).SetVoidResult();
        _module.SetupVoid("scrollSelectViewportByItem", _ => true).SetVoidResult();
        _module.SetupVoid("registerSelectContentPointerTracker", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterSelectContentPointerTracker", _ => true).SetVoidResult();
        _module.SetupVoid("registerSelectWindowDismiss", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterSelectWindowDismiss", _ => true).SetVoidResult();
        _module.SetupVoid("registerPresence", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterPresence", _ => true).SetVoidResult();
        _module.SetupVoid("mountPortal", _ => true).SetVoidResult();
        _module.SetupVoid("unmountPortal", _ => true).SetVoidResult();
        _module.SetupVoid("registerFocusGuards", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusGuards", _ => true).SetVoidResult();
        _module.SetupVoid("registerFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("updateFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("registerRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("focusElementPreventScroll", _ => true).SetVoidResult();
        _module.Setup<string>("getTextContent", _ => true).SetResult("Fruit");
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
    }

    [Fact]
    public void Arrow_down_opens_content_and_links_ids()
    {
        var cut = Render(CreateSelect());
        var trigger = cut.Find("button[role='combobox']");

        trigger.KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });

        cut.WaitForAssertion(() =>
        {
            var content = cut.Find("[role='listbox']");
            Assert.Equal("true", trigger.GetAttribute("aria-expanded"));
            Assert.Equal(content.Id, trigger.GetAttribute("aria-controls"));
            Assert.Equal("open", content.GetAttribute("data-state"));
        });
    }

    [Fact]
    public void Trigger_typeahead_selects_next_match_while_closed()
    {
        var cut = Render(CreateSelect(defaultValue: "orange"));
        var trigger = cut.Find("button[role='combobox']");

        trigger.KeyDown(new KeyboardEventArgs { Key = "l" });

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Lime", trigger.TextContent);
            Assert.True(cut.Find("option[value='lime']").HasAttribute("selected"));
        });
    }

    [Fact]
    public void Selecting_item_updates_value_and_closes_content()
    {
        var cut = Render(CreateSelect(defaultOpen: true, defaultValue: "orange"));

        var lime = cut.FindAll("[role='option']").First(item => item.TextContent.Contains("Lime"));

        lime.KeyDown(new KeyboardEventArgs { Key = "Enter" });

        cut.WaitForAssertion(() =>
        {
            var trigger = cut.Find("button[role='combobox']");
            Assert.Contains("Lime", trigger.TextContent);
            Assert.DoesNotContain("[role='listbox']", cut.Markup);
        });
    }

    [Fact]
    public void Space_does_not_select_item_while_typeahead_is_active()
    {
        var cut = Render(CreateSelect(defaultOpen: true, defaultValue: "orange"));

        var items = cut.FindAll("[role='option']");
        items[0].KeyDown(new KeyboardEventArgs { Key = "l" });
        items = cut.FindAll("[role='option']");
        items[1].KeyDown(new KeyboardEventArgs { Key = " " });

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Orange", cut.Find("button[role='combobox']").TextContent);
            Assert.NotEmpty(cut.FindAll("[role='listbox']"));
        });
    }

    [Fact]
    public void Hidden_native_select_tracks_selected_option()
    {
        var cut = Render(CreateSelect(defaultValue: "lime"));

        cut.WaitForAssertion(() =>
        {
            var hiddenSelect = cut.Find("select[aria-hidden='true']");
            Assert.Equal("fruit", hiddenSelect.GetAttribute("name"));
            Assert.True(cut.Find("option[value='lime']").HasAttribute("selected"));
        });
    }

    [Fact]
    public void Default_content_uses_item_aligned_position_and_group_label_wiring()
    {
        var cut = Render(CreateSelect(defaultOpen: true));

        cut.WaitForAssertion(() =>
        {
            var positionWrapper = cut.Find("[data-radix-select-position='item-aligned']");
            var group = cut.Find("[role='group']");
            var label = cut.Find("div[id^='bradix-select-group-label']");

            Assert.NotNull(positionWrapper);
            Assert.Equal(label.Id, group.GetAttribute("aria-labelledby"));
            Assert.Equal("Fruit", label.TextContent);
        });
    }

    [Fact]
    public async Task Scroll_buttons_follow_viewport_metrics()
    {
        var cut = Render(CreateSelect(defaultOpen: true, includeScrollButtons: true));
        var content = cut.FindComponent<BradixSelectContent>().Instance;

        await content.HandleViewportMetricsChangedAsync(0, 400, 120);

        cut.WaitForAssertion(() =>
        {
            Assert.Empty(cut.FindAll("[data-radix-select-scroll-up-button]"));
            Assert.Single(cut.FindAll("[data-radix-select-scroll-down-button]"));
        });

        await content.HandleViewportMetricsChangedAsync(40, 400, 120);

        cut.WaitForAssertion(() =>
        {
            Assert.Single(cut.FindAll("[data-radix-select-scroll-up-button]"));
            Assert.Single(cut.FindAll("[data-radix-select-scroll-down-button]"));
        });
    }

    [Fact]
    public async Task Pointer_guard_suppresses_first_mouse_pointerup_selection()
    {
        var cut = Render(CreateSelect(defaultOpen: true, defaultValue: "orange"));
        var content = cut.FindComponent<BradixSelectContent>().Instance;

        await content.HandleTriggerPointerGuardResultAsync(suppressSelection: true, shouldClose: false);

        cut.FindAll("[role='option']").First(item => item.TextContent.Contains("Lime"))
            .TriggerEvent("onpointerup", new PointerEventArgs { PointerType = "mouse" });

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Orange", cut.Find("button[role='combobox']").TextContent);
            Assert.NotEmpty(cut.FindAll("[role='listbox']"));
        });
    }

    [Fact]
    public async Task Pointer_guard_closes_when_pointerup_occurs_outside_content()
    {
        var cut = Render(CreateSelect(defaultOpen: true, defaultValue: "orange"));
        var content = cut.FindComponent<BradixSelectContent>().Instance;

        await content.HandleTriggerPointerGuardResultAsync(suppressSelection: false, shouldClose: true);

        cut.WaitForAssertion(() =>
        {
            Assert.Empty(cut.FindAll("[role='listbox']"));
        });
    }

    [Fact]
    public async Task Window_dismiss_closes_open_content()
    {
        var cut = Render(CreateSelect(defaultOpen: true, defaultValue: "orange"));
        var content = cut.FindComponent<BradixSelectContent>().Instance;

        await content.HandleWindowDismissAsync();

        cut.WaitForAssertion(() =>
        {
            Assert.Empty(cut.FindAll("[role='listbox']"));
        });
    }

    private static RenderFragment CreateSelect(bool defaultOpen = false, string? defaultValue = null, string? position = null, bool includeScrollButtons = false)
    {
        return builder =>
        {
            builder.OpenComponent<BradixSelect>(0);
            builder.AddAttribute(1, nameof(BradixSelect.DefaultOpen), defaultOpen);
            builder.AddAttribute(2, nameof(BradixSelect.DefaultValue), defaultValue);
            builder.AddAttribute(3, nameof(BradixSelect.Name), "fruit");
            builder.AddAttribute(4, nameof(BradixSelect.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixSelectTrigger>(0);
                content.AddAttribute(1, nameof(BradixSelectTrigger.ChildContent), (RenderFragment)(trigger =>
                {
                    trigger.OpenComponent<BradixSelectValue>(0);
                    trigger.AddAttribute(1, nameof(BradixSelectValue.Placeholder), (RenderFragment)(placeholder => placeholder.AddContent(0, "Choose fruit")));
                    trigger.CloseComponent();
                    trigger.OpenComponent<BradixSelectIcon>(2);
                    trigger.CloseComponent();
                }));
                content.CloseComponent();

                content.OpenComponent<BradixSelectPortal>(5);
                content.AddAttribute(6, nameof(BradixSelectPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixSelectContent>(0);
                    if (!string.IsNullOrWhiteSpace(position))
                        portal.AddAttribute(1, nameof(BradixSelectContent.Position), position);

                    portal.AddAttribute(1, nameof(BradixSelectContent.ChildContent), (RenderFragment)(selectContent =>
                    {
                        if (includeScrollButtons)
                        {
                            selectContent.OpenComponent<BradixSelectScrollUpButton>(0);
                            selectContent.AddAttribute(1, nameof(BradixSelectScrollUpButton.ChildContent), (RenderFragment)(button => button.AddContent(0, "Up")));
                            selectContent.CloseComponent();
                        }

                        selectContent.OpenComponent<BradixSelectViewport>(0);
                        selectContent.AddAttribute(1, nameof(BradixSelectViewport.ChildContent), (RenderFragment)(viewport =>
                        {
                            viewport.OpenComponent<BradixSelectGroup>(0);
                            viewport.AddAttribute(1, nameof(BradixSelectGroup.ChildContent), (RenderFragment)(group =>
                            {
                                group.OpenComponent<BradixSelectLabel>(0);
                                group.AddAttribute(1, nameof(BradixSelectLabel.ChildContent), (RenderFragment)(label => label.AddContent(0, "Fruit")));
                                group.CloseComponent();

                                RenderItem(group, 2, "orange", "Orange");
                                RenderItem(group, 5, "lime", "Lime");
                                RenderItem(group, 8, "strawberry", "Strawberry");
                            }));
                            viewport.CloseComponent();
                        }));
                        selectContent.CloseComponent();

                        if (includeScrollButtons)
                        {
                            selectContent.OpenComponent<BradixSelectScrollDownButton>(2);
                            selectContent.AddAttribute(3, nameof(BradixSelectScrollDownButton.ChildContent), (RenderFragment)(button => button.AddContent(0, "Down")));
                            selectContent.CloseComponent();
                        }
                    }));
                    portal.CloseComponent();
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }

    private static void RenderItem(RenderTreeBuilder builder, int sequence, string value, string text)
    {
        builder.OpenComponent<BradixSelectItem>(sequence);
        builder.AddAttribute(sequence + 1, nameof(BradixSelectItem.Value), value);
        builder.AddAttribute(sequence + 2, nameof(BradixSelectItem.TextValue), text);
        builder.AddAttribute(sequence + 3, nameof(BradixSelectItem.ChildContent), (RenderFragment)(item =>
        {
            item.OpenComponent<BradixSelectItemText>(0);
            item.AddAttribute(1, nameof(BradixSelectItemText.ChildContent), (RenderFragment)(textContent => textContent.AddContent(0, text)));
            item.CloseComponent();
            item.OpenComponent<BradixSelectItemIndicator>(2);
            item.AddAttribute(3, nameof(BradixSelectItemIndicator.ChildContent), (RenderFragment)(indicator => indicator.AddContent(0, "[x]")));
            item.CloseComponent();
        }));
        builder.CloseComponent();
    }
}
