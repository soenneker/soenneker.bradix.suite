using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixDropdownMenuRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixDropdownMenuRenderTests()
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
        _module.SetupVoid("registerPresence", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterPresence", _ => true).SetVoidResult();
        _module.SetupVoid("mountPortal", _ => true).SetVoidResult();
        _module.SetupVoid("unmountPortal", _ => true).SetVoidResult();
        _module.SetupVoid("registerFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("updateFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusScope", _ => true).SetVoidResult();
        _module.SetupVoid("registerFocusGuards", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFocusGuards", _ => true).SetVoidResult();
        _module.SetupVoid("registerHideOthers", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterHideOthers", _ => true).SetVoidResult();
        _module.SetupVoid("registerRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterRemoveScroll", _ => true).SetVoidResult();
        _module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("registerRovingFocusNavigationKeys", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterRovingFocusNavigationKeys", _ => true).SetVoidResult();
        _module.Setup<bool>("isKeyboardInteractionMode", _ => true).SetResult(false);
        _module.Setup<string>("getTextContent", _ => true).SetResult("Share");
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Trigger_arrow_down_opens_content_and_links_ids()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateDropdownMenu());
        IElement trigger = cut.Find("button");
        string closedControls = await Assert.That(trigger.GetAttribute("aria-controls")).IsTypeOf<string>();

        await trigger.KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowDown" });

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement menu = cut.Find("[role='menu']");
            await Assert.That(trigger.GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(trigger.GetAttribute("aria-controls")).IsEqualTo(closedControls);
            await Assert.That(trigger.GetAttribute("aria-controls")).IsEqualTo(menu.Id);
            await Assert.That(menu.GetAttribute("aria-labelledby")).IsEqualTo(trigger.Id);
        });
    }

    [Test]
    public async Task Trigger_pointer_down_opens_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateDropdownMenu());
        IElement trigger = cut.Find("button");

        await trigger.TriggerEventAsync("onpointerdown", new Microsoft.AspNetCore.Components.Web.PointerEventArgs
        {
            Button = 0,
            CtrlKey = false
        });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(trigger.GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(cut.Find("[role='menu']").GetAttribute("data-state")).IsEqualTo("open");
        });
    }

    [Test]
    public async Task Opening_dropdown_registers_focus_scope_with_manual_autofocus_flags()
    {
        _ = Render(CreateDropdownMenu(defaultOpen: true));

        JSRuntimeInvocation invocation = _module.Invocations.Single(call => call.Identifier == "registerFocusScope");

        await Assert.That(invocation.Arguments[4]).IsEqualTo(true);
        await Assert.That(invocation.Arguments[5]).IsEqualTo(true);
    }

    [Test]
    public async Task Detailed_close_auto_focus_can_prevent_dropdown_trigger_refocus()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateDropdownMenu(
            defaultOpen: true,
            onCloseAutoFocusDetailed: EventCallback.Factory.Create<BradixAutoFocusEventArgs>(this, args => args.PreventDefault())));

        int focusCountBefore = _module.Invocations.Count(invocation => invocation.Identifier == "focusElementPreventScroll");
        IRenderedComponent<BradixFocusScope> focusScope = cut.FindComponent<BradixFocusScope>();

        bool prevented = await cut.InvokeAsync(() => focusScope.Instance.HandleUnmountAutoFocus());

        await Assert.That(prevented).IsTrue();
        await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "focusElementPreventScroll")).IsEqualTo(focusCountBefore);
    }

    [Test]
    public async Task Checkbox_and_radio_wrappers_render_checked_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateDropdownMenu(defaultOpen: true));

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> radioItems = cut.FindAll("[role='menuitemradio']");

            await Assert.That(cut.Find("[role='menuitemcheckbox']").GetAttribute("aria-checked")).IsEqualTo("mixed");
            await Assert.That(radioItems.Any(item => item.GetAttribute("aria-checked") == "true")).IsTrue();
            await Assert.That(radioItems.Any(item => item.GetAttribute("aria-checked") == "false")).IsTrue();
        });
    }

    [Test]
    public async Task Submenu_wrapper_opens_from_sub_trigger()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateDropdownMenu(defaultOpen: true));
        IElement subTrigger = cut.Find("[aria-haspopup='menu'][role='menuitem']");

        await subTrigger.KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowRight" });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("[aria-haspopup='menu'][role='menuitem']").GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(cut.Markup).Contains("Copy link");
        });
    }

    [Test]
    public async Task Selecting_item_closes_dropdown_root()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateDropdownMenu(defaultOpen: true));

        await cut.FindAll("[role='menuitem']").First().ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("[role='menu']").GetAttribute("data-state")).IsEqualTo("closed");
        });
    }

    [Test]
    public async Task Detailed_escape_keydown_can_prevent_dropdown_submenu_close()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateDropdownMenu(
            defaultOpen: true,
            onSubEscapeKeyDownDetailed: EventCallback.Factory.Create<BradixEscapeKeyDownEventArgs>(this, args => args.PreventDefault())));

        await cut.Find("[aria-haspopup='menu'][role='menuitem']").KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowRight" });
        await Assert.That(cut.FindAll("[role='menu']").Count).IsEqualTo(2);

        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponents<BradixDismissableLayer>().Last();
        bool prevented = await cut.InvokeAsync(() => layer.Instance.HandleEscapeKeyDown());

        await Assert.That(prevented).IsFalse();
        await Assert.That(cut.FindAll("[role='menu']").Count).IsEqualTo(2);
    }

    [Test]
    public async Task Submenu_escape_closes_dropdown_root()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateDropdownMenu(defaultOpen: true));

        await cut.Find("[aria-haspopup='menu'][role='menuitem']").KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowRight" });
        await Assert.That(cut.FindAll("[role='menu']").Count).IsEqualTo(2);

        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponents<BradixDismissableLayer>().Last();
        await cut.InvokeAsync(() => layer.Instance.HandleEscapeKeyDown());

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("[role='menu']").GetAttribute("data-state")).IsEqualTo("closed");
        });
    }

    [Test]
    public async Task Modal_right_click_outside_does_not_refocus_dropdown_trigger_on_close()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateDropdownMenu(defaultOpen: true, modal: true));
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();
        IRenderedComponent<BradixFocusScope> focusScope = cut.FindComponent<BradixFocusScope>();

        int focusCountBefore = _module.Invocations.Count(invocation => invocation.Identifier == "focusElementPreventScroll");

        await cut.InvokeAsync(() => layer.Instance.HandlePointerDownOutside(new BradixDelegatedMouseEvent
        {
            Button = 2
        }));

        bool prevented = await cut.InvokeAsync(() => focusScope.Instance.HandleUnmountAutoFocus());

        await Assert.That(prevented).IsTrue();
        await Assert.That(_module.Invocations.Count(invocation => invocation.Identifier == "focusElementPreventScroll")).IsEqualTo(focusCountBefore);
    }

    private static RenderFragment CreateDropdownMenu(
        bool defaultOpen = false,
        bool modal = true,
        EventCallback<BradixAutoFocusEventArgs> onCloseAutoFocusDetailed = default,
        EventCallback<BradixEscapeKeyDownEventArgs> onSubEscapeKeyDownDetailed = default)
    {
        return builder =>
        {
            builder.OpenComponent<BradixDropdownMenu>(0);
            builder.AddAttribute(1, nameof(BradixDropdownMenu.DefaultOpen), defaultOpen);
            builder.AddAttribute(2, nameof(BradixDropdownMenu.Modal), modal);
            builder.AddAttribute(3, nameof(BradixDropdownMenu.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixDropdownMenuTrigger>(0);
                content.AddAttribute(1, nameof(BradixDropdownMenuTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, "Open")));
                content.CloseComponent();

                content.OpenComponent<BradixDropdownMenuPortal>(2);
                content.AddAttribute(3, nameof(BradixDropdownMenuPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixDropdownMenuContent>(0);
                    if (onCloseAutoFocusDetailed.HasDelegate)
                        portal.AddAttribute(1, nameof(BradixDropdownMenuContent.OnCloseAutoFocusDetailed), onCloseAutoFocusDetailed);
                    portal.AddAttribute(2, nameof(BradixDropdownMenuContent.ChildContent), (RenderFragment)(menuContent =>
                    {
                        menuContent.OpenComponent<BradixDropdownMenuItem>(0);
                        menuContent.AddAttribute(1, nameof(BradixDropdownMenuItem.TextValue), "Edit");
                        menuContent.AddAttribute(2, nameof(BradixDropdownMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Edit")));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixDropdownMenuCheckboxItem>(3);
                        menuContent.AddAttribute(4, nameof(BradixDropdownMenuCheckboxItem.DefaultChecked), (object) BradixCheckboxCheckedState.Indeterminate);
                        menuContent.AddAttribute(5, nameof(BradixDropdownMenuCheckboxItem.CloseOnSelect), false);
                        menuContent.AddAttribute(6, nameof(BradixDropdownMenuCheckboxItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Show bookmarks")));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixDropdownMenuRadioGroup>(7);
                        menuContent.AddAttribute(8, nameof(BradixDropdownMenuRadioGroup.DefaultValue), "name");
                        menuContent.AddAttribute(9, nameof(BradixDropdownMenuRadioGroup.ChildContent), (RenderFragment)(group =>
                        {
                            group.OpenComponent<BradixDropdownMenuRadioItem>(0);
                            group.AddAttribute(1, nameof(BradixDropdownMenuRadioItem.Value), "name");
                            group.AddAttribute(2, nameof(BradixDropdownMenuRadioItem.CloseOnSelect), false);
                            group.AddAttribute(3, nameof(BradixDropdownMenuRadioItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Name")));
                            group.CloseComponent();

                            group.OpenComponent<BradixDropdownMenuRadioItem>(4);
                            group.AddAttribute(5, nameof(BradixDropdownMenuRadioItem.Value), "date");
                            group.AddAttribute(6, nameof(BradixDropdownMenuRadioItem.CloseOnSelect), false);
                            group.AddAttribute(7, nameof(BradixDropdownMenuRadioItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Date modified")));
                            group.CloseComponent();
                        }));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixDropdownMenuSub>(10);
                        menuContent.AddAttribute(11, nameof(BradixDropdownMenuSub.ChildContent), (RenderFragment)(sub =>
                        {
                            sub.OpenComponent<BradixDropdownMenuSubTrigger>(0);
                            sub.AddAttribute(1, nameof(BradixDropdownMenuSubTrigger.TextValue), "Share");
                            sub.AddAttribute(2, nameof(BradixDropdownMenuSubTrigger.ChildContent), (RenderFragment)(item => item.AddContent(0, "Share")));
                            sub.CloseComponent();

                            sub.OpenComponent<BradixDropdownMenuPortal>(3);
                            sub.AddAttribute(4, nameof(BradixDropdownMenuPortal.ChildContent), (RenderFragment)(subPortal =>
                            {
                                subPortal.OpenComponent<BradixDropdownMenuSubContent>(0);
                                if (onSubEscapeKeyDownDetailed.HasDelegate)
                                    subPortal.AddAttribute(1, nameof(BradixDropdownMenuSubContent.OnEscapeKeyDownDetailed), onSubEscapeKeyDownDetailed);
                                subPortal.AddAttribute(2, nameof(BradixDropdownMenuSubContent.ChildContent), (RenderFragment)(submenu =>
                                {
                                    submenu.OpenComponent<BradixDropdownMenuItem>(0);
                                    submenu.AddAttribute(1, nameof(BradixDropdownMenuItem.TextValue), "Copy link");
                                    submenu.AddAttribute(2, nameof(BradixDropdownMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Copy link")));
                                    submenu.CloseComponent();
                                }));
                                subPortal.CloseComponent();
                            }));
                            sub.CloseComponent();
                        }));
                        menuContent.CloseComponent();
                    }));
                    portal.CloseComponent();
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }
}
