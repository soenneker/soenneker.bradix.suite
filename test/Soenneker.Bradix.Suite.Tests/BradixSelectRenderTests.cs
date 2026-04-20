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

public sealed class BradixSelectRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixSelectRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("updateDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("registerDismissableLayerBranch", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayerBranch", _ => true).SetVoidResult();
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
        _module.SetupVoid("registerHideOthers", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterHideOthers", _ => true).SetVoidResult();
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
        _module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("focusElementPreventScroll", _ => true).SetVoidResult();
        _module.SetupVoid("scrollElementIntoViewNearest", _ => true).SetVoidResult();
        _module.SetupVoid("syncSelectBubbleInputValue", _ => true).SetVoidResult();
        _module.SetupVoid("registerSelectBubbleInput", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterSelectBubbleInput", _ => true).SetVoidResult();
        _module.Setup<bool>("isFormControl", _ => true).SetResult(true);
        _module.Setup<string>("getTextContent", _ => true).SetResult("Fruit");
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Closed_select_defers_content_and_item_interop()
    {
        _ = Render(CreateSelect());

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerSelectViewport")).IsFalse();
        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerSelectItemAlignedPosition")).IsFalse();
        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerSelectWindowDismiss")).IsFalse();
        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "getTextContent")).IsFalse();
    }

    [Test]
    public async Task Arrow_down_opens_content_and_links_ids()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect());
        IElement trigger = cut.Find("button[role='combobox']");

        await trigger.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement content = cut.Find("[role='listbox']");
            await Assert.That(trigger.GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(trigger.GetAttribute("aria-controls")).IsEqualTo(content.Id);
            await Assert.That(content.GetAttribute("data-state")).IsEqualTo("open");
        });

        await Assert.That(trigger.GetAttribute("aria-haspopup")).IsEqualTo("listbox");
    }

    [Test]
    public async Task Primary_mouse_pointer_down_opens_content_from_delegated_trigger_path()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect());
        IRenderedComponent<BradixSelectTrigger> trigger = cut.FindComponent<BradixSelectTrigger>();

        await cut.InvokeAsync(() => trigger.Instance.HandleDelegatedPointerDown(new BradixDelegatedMouseEvent
        {
            Button = 0,
            PageX = 12,
            PageY = 24,
            PointerType = "mouse"
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("button[role='combobox']").GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(cut.Find("[role='listbox']").GetAttribute("data-state")).IsEqualTo("open");
        });
    }

    [Test]
    public async Task Open_select_disables_outside_pointer_events()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect());
        IRenderedComponent<BradixSelectTrigger> trigger = cut.FindComponent<BradixSelectTrigger>();

        await cut.InvokeAsync(() => trigger.Instance.HandleDelegatedPointerDown(new BradixDelegatedMouseEvent
        {
            Button = 0,
            PageX = 12,
            PageY = 24,
            PointerType = "mouse"
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            JSRuntimeInvocation invocation = _module.Invocations.Last(call => call.Identifier == "registerDismissableLayer");
            await Assert.That(invocation.Arguments[2]).IsEqualTo(true);
        });
    }

    [Test]
    public async Task Open_select_hides_outside_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect());
        IRenderedComponent<BradixSelectTrigger> trigger = cut.FindComponent<BradixSelectTrigger>();

        await cut.InvokeAsync(() => trigger.Instance.HandleDelegatedPointerDown(new BradixDelegatedMouseEvent
        {
            Button = 0,
            PageX = 12,
            PageY = 24,
            PointerType = "mouse"
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerHideOthers")).IsTrue();
        });
    }

    [Test]
    public async Task Open_select_registers_focus_guards_and_remove_scroll()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect());
        IRenderedComponent<BradixSelectTrigger> trigger = cut.FindComponent<BradixSelectTrigger>();

        await cut.InvokeAsync(() => trigger.Instance.HandleDelegatedPointerDown(new BradixDelegatedMouseEvent
        {
            Button = 0,
            PageX = 12,
            PageY = 24,
            PointerType = "mouse"
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerFocusGuards")).IsTrue();
            await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "registerRemoveScroll")).IsTrue();
        });
    }

    [Test]
    public async Task Trigger_typeahead_selects_next_match_while_closed()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect(defaultValue: "orange"));
        IElement trigger = cut.Find("button[role='combobox']");

        await trigger.KeyDownAsync(new KeyboardEventArgs { Key = "l" });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(trigger.TextContent).Contains("Lime");
            await Assert.That(cut.Find("option[value='lime']").HasAttribute("selected")).IsTrue();
        });
    }

    [Test]
    public async Task Selecting_item_updates_value_and_closes_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect(defaultOpen: true, defaultValue: "orange"));

        IElement lime = cut.FindAll("[role='option']").First(item => item.TextContent.Contains("Lime"));

        await lime.KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement trigger = cut.Find("button[role='combobox']");
            await Assert.That(trigger.TextContent).Contains("Lime");
            await Assert.That(cut.Markup).DoesNotContain("[role='listbox']");
        });
    }

    [Test]
    public async Task Space_does_not_select_item_while_typeahead_is_active()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect(defaultOpen: true, defaultValue: "orange"));
        IRenderedComponent<BradixSelectContent> content = cut.FindComponent<BradixSelectContent>();

        IReadOnlyList<IElement> items = cut.FindAll("[role='option']");
        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "l"
        }));
        items = cut.FindAll("[role='option']");
        await items[1].KeyDownAsync(new KeyboardEventArgs { Key = " " });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("button[role='combobox']").TextContent).Contains("Orange");
            await Assert.That(cut.FindAll("[role='listbox']")).IsNotEmpty();
        });
    }

    [Test]
    public async Task Page_up_and_page_down_move_focus_to_first_and_last_items()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect(defaultOpen: true, defaultValue: "orange"));
        IRenderedComponent<BradixSelectContent> content = cut.FindComponent<BradixSelectContent>();

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "PageDown"
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> items = cut.FindAll("[role='option']");
            await Assert.That(items[0].GetAttribute("tabindex")).IsEqualTo("-1");
            await Assert.That(items[1].GetAttribute("tabindex")).IsEqualTo("-1");
            await Assert.That(items[2].GetAttribute("tabindex")).IsEqualTo("0");
        });

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "PageUp"
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> items = cut.FindAll("[role='option']");
            await Assert.That(items[0].GetAttribute("tabindex")).IsEqualTo("0");
            await Assert.That(items[1].GetAttribute("tabindex")).IsEqualTo("-1");
            await Assert.That(items[2].GetAttribute("tabindex")).IsEqualTo("-1");
        });

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "scrollElementIntoViewNearest")).IsTrue();
    }

    [Test]
    public async Task Content_typeahead_scrolls_newly_focused_item_into_view()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect(defaultOpen: true, defaultValue: "orange"));
        IRenderedComponent<BradixSelectContent> content = cut.FindComponent<BradixSelectContent>();

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "s"
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> items = cut.FindAll("[role='option']");
            await Assert.That(items[0].GetAttribute("tabindex")).IsEqualTo("-1");
            await Assert.That(items[1].GetAttribute("tabindex")).IsEqualTo("-1");
            await Assert.That(items[2].GetAttribute("tabindex")).IsEqualTo("0");
        });

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "scrollElementIntoViewNearest")).IsTrue();
    }

    [Test]
    public async Task Hidden_native_select_tracks_selected_option()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect(defaultValue: "lime"));

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement hiddenSelect = cut.Find("select[aria-hidden='true']");
            await Assert.That(hiddenSelect.GetAttribute("name")).IsEqualTo("fruit");
            await Assert.That(cut.Find("option[value='lime']").HasAttribute("selected")).IsTrue();
        });
    }

    [Test]
    public async Task Default_content_uses_item_aligned_position_and_group_label_wiring()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect(defaultOpen: true));

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement positionWrapper = cut.Find("[data-radix-select-position='item-aligned']");
            IElement group = cut.Find("[role='group']");
            string? labelId = group.GetAttribute("aria-labelledby");
            await Assert.That(string.IsNullOrWhiteSpace(labelId)).IsFalse();
            IElement label = cut.Find($"#{labelId}");

            await Assert.That(positionWrapper).IsNotNull();
            await Assert.That(group.GetAttribute("aria-labelledby")).IsEqualTo(label.Id);
            await Assert.That(label.TextContent).IsEqualTo("Fruit");
        });
    }

    [Test]
    public async Task Scroll_buttons_follow_viewport_metrics()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect(defaultOpen: true, includeScrollButtons: true));
        BradixSelectContent content = cut.FindComponent<BradixSelectContent>().Instance;

        await content.HandleViewportMetricsChanged(0, 400, 120);

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.FindAll("[data-radix-select-scroll-up-button]")).IsEmpty();
            await Assert.That(cut.FindAll("[data-radix-select-scroll-down-button]")).HasSingleItem();
        });

        await content.HandleViewportMetricsChanged(40, 400, 120);

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.FindAll("[data-radix-select-scroll-up-button]")).HasSingleItem();
            await Assert.That(cut.FindAll("[data-radix-select-scroll-down-button]")).HasSingleItem();
        });
    }

    [Test]
    public async Task Pointer_guard_suppresses_first_mouse_pointerup_selection()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect(defaultOpen: true, defaultValue: "orange"));
        BradixSelectContent content = cut.FindComponent<BradixSelectContent>().Instance;

        await content.HandleTriggerPointerGuardResult(suppressSelection: true, shouldClose: false);

        await cut.FindAll("[role='option']").First(item => item.TextContent.Contains("Lime"))
                 .TriggerEventAsync("onpointerup", new PointerEventArgs { PointerType = "mouse" });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("button[role='combobox']").TextContent).Contains("Orange");
            await Assert.That(cut.FindAll("[role='listbox']")).IsNotEmpty();
        });
    }

    [Test]
    public async Task Pointer_guard_closes_when_pointerup_occurs_outside_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect(defaultOpen: true, defaultValue: "orange"));
        BradixSelectContent content = cut.FindComponent<BradixSelectContent>().Instance;

        await content.HandleTriggerPointerGuardResult(suppressSelection: false, shouldClose: true);

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.FindAll("[role='listbox']")).IsEmpty();
        });
    }

    [Test]
    public async Task Window_dismiss_closes_open_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect(defaultOpen: true, defaultValue: "orange"));
        BradixSelectContent content = cut.FindComponent<BradixSelectContent>().Instance;

        await content.HandleWindowDismiss();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.FindAll("[role='listbox']")).IsEmpty();
        });
    }

    [Test]
    public async Task Select_can_reopen_after_closing()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSelect(defaultValue: "orange"));
        IRenderedComponent<BradixSelectTrigger> trigger = cut.FindComponent<BradixSelectTrigger>();

        await cut.InvokeAsync(() => trigger.Instance.HandleDelegatedPointerDown(new BradixDelegatedMouseEvent
        {
            Button = 0,
            PageX = 12,
            PageY = 24,
            PointerType = "mouse"
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.FindAll("[role='listbox']")).HasSingleItem();
        });

        BradixSelectContent content = cut.FindComponent<BradixSelectContent>().Instance;
        await cut.InvokeAsync(() => content.HandleWindowDismiss());

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.FindAll("[role='listbox']")).IsEmpty();
        });

        trigger = cut.FindComponent<BradixSelectTrigger>();
        await cut.InvokeAsync(() => trigger.Instance.HandleDelegatedPointerDown(new BradixDelegatedMouseEvent
        {
            Button = 0,
            PageX = 48,
            PageY = 72,
            PointerType = "mouse"
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.FindAll("[role='listbox']")).HasSingleItem();
        });
    }

    [Test]
    public async Task Detailed_close_auto_focus_can_prevent_select_trigger_refocus()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixSelect>(0);
            builder.AddAttribute(1, nameof(BradixSelect.DefaultOpen), true);
            builder.AddAttribute(2, nameof(BradixSelect.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixSelectTrigger>(0);
                content.AddAttribute(1, nameof(BradixSelectTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, "Trigger")));
                content.CloseComponent();

                content.OpenComponent<BradixSelectPortal>(2);
                content.AddAttribute(3, nameof(BradixSelectPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixSelectContent>(0);
                    portal.AddAttribute(1, nameof(BradixSelectContent.OnCloseAutoFocusDetailed),
                        EventCallback.Factory.Create<BradixAutoFocusEventArgs>(this, args => args.PreventDefault()));
                    portal.AddAttribute(2, nameof(BradixSelectContent.ChildContent), (RenderFragment)(selectContent =>
                    {
                        selectContent.OpenComponent<BradixSelectViewport>(0);
                        selectContent.AddAttribute(1, nameof(BradixSelectViewport.ChildContent), (RenderFragment)(viewport =>
                        {
                            RenderItem(viewport, 0, "orange", "Orange");
                        }));
                        selectContent.CloseComponent();
                    }));
                    portal.CloseComponent();
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });

        IRenderedComponent<BradixFocusScope> focusScope = cut.FindComponent<BradixFocusScope>();
        bool prevented = await cut.InvokeAsync(() => focusScope.Instance.HandleUnmountAutoFocus());

        await Assert.That(prevented).IsTrue();
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
