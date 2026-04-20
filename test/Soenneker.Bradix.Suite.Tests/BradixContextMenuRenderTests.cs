using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixContextMenuRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixContextMenuRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("updateDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("registerDismissableLayerBranch", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayerBranch", _ => true).SetVoidResult();
        _module.SetupVoid("registerPopperContent", _ => true).SetVoidResult();
        _module.SetupVoid("updatePopperContent", _ => true).SetVoidResult();
        _module.SetupVoid("registerVirtualPopperContent", _ => true).SetVoidResult();
        _module.SetupVoid("updateVirtualPopperContent", _ => true).SetVoidResult();
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
    public async Task Right_click_opens_context_menu_and_registers_virtual_anchor()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateContextMenu());
        IElement trigger = cut.Find("[data-state='closed']");
        string? closedControlsAttr = trigger.GetAttribute("aria-controls");
        await Assert.That(closedControlsAttr).IsNotNull();
        string closedControls = closedControlsAttr!;
        await Assert.That(trigger.GetAttribute("aria-haspopup")).IsEqualTo("menu");
        await Assert.That(trigger.GetAttribute("aria-expanded")).IsEqualTo("false");

        await trigger.TriggerEventAsync("oncontextmenu", new MouseEventArgs { ClientX = 120, ClientY = 40, Button = 2 });

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement updatedTrigger = cut.Find("[aria-haspopup='menu']");
            await Assert.That(cut.Find("[role='menu']").GetAttribute("data-state")).IsEqualTo("open");
            await Assert.That(updatedTrigger.GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(updatedTrigger.GetAttribute("aria-controls")).IsEqualTo(closedControls);
        });

        await Assert.That(_module.Invocations.Any(invocation =>
            invocation.Identifier == "registerVirtualPopperContent" &&
            invocation.Arguments.Count >= 5 &&
            invocation.Arguments[3] is double x &&
            invocation.Arguments[4] is double y &&
            x == 120 &&
            y == 40)).IsTrue();
    }

    [Test]
    public async Task Checkbox_and_radio_wrappers_render_checked_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateContextMenu());
        await cut.Find("[data-state='closed']").TriggerEventAsync("oncontextmenu", new MouseEventArgs { ClientX = 120, ClientY = 40, Button = 2 });

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
        IRenderedComponent<ContainerFragment> cut = Render(CreateContextMenu());
        await cut.Find("[data-state='closed']").TriggerEventAsync("oncontextmenu", new MouseEventArgs { ClientX = 120, ClientY = 40, Button = 2 });
        IElement subTrigger = cut.Find("[aria-haspopup='menu'][role='menuitem']");

        await subTrigger.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("[aria-haspopup='menu'][role='menuitem']").GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(cut.Markup).Contains("Copy link");
        });
    }

    [Test]
    public async Task Selecting_item_closes_context_menu_root()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateContextMenu());
        await cut.Find("[data-state='closed']").TriggerEventAsync("oncontextmenu", new MouseEventArgs { ClientX = 120, ClientY = 40, Button = 2 });

        await cut.FindAll("[role='menuitem']").First().ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("[role='menu']").GetAttribute("data-state")).IsEqualTo("closed");
        });
    }

    [Test]
    public async Task Non_modal_outside_interaction_prevents_close_auto_focus()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateContextMenu(modal: false));
        await cut.Find("[data-state='closed']").TriggerEventAsync("oncontextmenu", new MouseEventArgs { ClientX = 120, ClientY = 40, Button = 2 });

        IRenderedComponent<BradixDismissableLayer> dismissableLayer = cut.FindComponent<BradixDismissableLayer>();
        IRenderedComponent<BradixFocusScope> focusScope = cut.FindComponent<BradixFocusScope>();

        await cut.InvokeAsync(() => dismissableLayer.Instance.HandlePointerDownOutside());
        bool prevented = await cut.InvokeAsync(() => focusScope.Instance.HandleUnmountAutoFocus());

        await Assert.That(prevented).IsTrue();
    }

    [Test]
    public async Task Detailed_close_auto_focus_can_prevent_context_menu_refocus()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixContextMenu>(0);
            builder.AddAttribute(1, nameof(BradixContextMenu.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixContextMenuTrigger>(0);
                content.AddAttribute(1, nameof(BradixContextMenuTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, "Area")));
                content.CloseComponent();

                content.OpenComponent<BradixContextMenuPortal>(2);
                content.AddAttribute(3, nameof(BradixContextMenuPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixContextMenuContent>(0);
                    portal.AddAttribute(1, nameof(BradixContextMenuContent.OnCloseAutoFocusDetailed),
                        EventCallback.Factory.Create<BradixAutoFocusEventArgs>(this, args => args.PreventDefault()));
                    portal.AddAttribute(2, nameof(BradixContextMenuContent.ChildContent), (RenderFragment)(menuContent =>
                    {
                        menuContent.OpenComponent<BradixContextMenuItem>(0);
                        menuContent.AddAttribute(1, nameof(BradixContextMenuItem.TextValue), "Open");
                        menuContent.AddAttribute(2, nameof(BradixContextMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Open")));
                        menuContent.CloseComponent();
                    }));
                    portal.CloseComponent();
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });

        await cut.Find("[data-state='closed']").TriggerEventAsync("oncontextmenu", new MouseEventArgs { ClientX = 120, ClientY = 40, Button = 2 });

        IRenderedComponent<BradixFocusScope> focusScope = cut.FindComponent<BradixFocusScope>();
        bool prevented = await cut.InvokeAsync(() => focusScope.Instance.HandleUnmountAutoFocus());

        await Assert.That(prevented).IsTrue();
    }

    private static RenderFragment CreateContextMenu(bool modal = true)
    {
        return builder =>
        {
            builder.OpenComponent<BradixContextMenu>(0);
            builder.AddAttribute(1, nameof(BradixContextMenu.Modal), modal);
            builder.AddAttribute(2, nameof(BradixContextMenu.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixContextMenuTrigger>(0);
                content.AddAttribute(1, nameof(BradixContextMenuTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, "Area")));
                content.CloseComponent();

                content.OpenComponent<BradixContextMenuPortal>(2);
                content.AddAttribute(3, nameof(BradixContextMenuPortal.ChildContent), (RenderFragment)(portal =>
                {
                    portal.OpenComponent<BradixContextMenuContent>(0);
                    portal.AddAttribute(1, nameof(BradixContextMenuContent.ChildContent), (RenderFragment)(menuContent =>
                    {
                        menuContent.OpenComponent<BradixContextMenuItem>(0);
                        menuContent.AddAttribute(1, nameof(BradixContextMenuItem.TextValue), "Open");
                        menuContent.AddAttribute(2, nameof(BradixContextMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Open")));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixContextMenuCheckboxItem>(3);
                        menuContent.AddAttribute(4, nameof(BradixContextMenuCheckboxItem.DefaultChecked), (object) BradixCheckboxCheckedState.Indeterminate);
                        menuContent.AddAttribute(5, nameof(BradixContextMenuCheckboxItem.CloseOnSelect), false);
                        menuContent.AddAttribute(6, nameof(BradixContextMenuCheckboxItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Show bookmarks")));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixContextMenuRadioGroup>(7);
                        menuContent.AddAttribute(8, nameof(BradixContextMenuRadioGroup.DefaultValue), "name");
                        menuContent.AddAttribute(9, nameof(BradixContextMenuRadioGroup.ChildContent), (RenderFragment)(group =>
                        {
                            group.OpenComponent<BradixContextMenuRadioItem>(0);
                            group.AddAttribute(1, nameof(BradixContextMenuRadioItem.Value), "name");
                            group.AddAttribute(2, nameof(BradixContextMenuRadioItem.CloseOnSelect), false);
                            group.AddAttribute(3, nameof(BradixContextMenuRadioItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Name")));
                            group.CloseComponent();

                            group.OpenComponent<BradixContextMenuRadioItem>(4);
                            group.AddAttribute(5, nameof(BradixContextMenuRadioItem.Value), "date");
                            group.AddAttribute(6, nameof(BradixContextMenuRadioItem.CloseOnSelect), false);
                            group.AddAttribute(7, nameof(BradixContextMenuRadioItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Date modified")));
                            group.CloseComponent();
                        }));
                        menuContent.CloseComponent();

                        menuContent.OpenComponent<BradixContextMenuSub>(10);
                        menuContent.AddAttribute(11, nameof(BradixContextMenuSub.ChildContent), (RenderFragment)(sub =>
                        {
                            sub.OpenComponent<BradixContextMenuSubTrigger>(0);
                            sub.AddAttribute(1, nameof(BradixContextMenuSubTrigger.TextValue), "Share");
                            sub.AddAttribute(2, nameof(BradixContextMenuSubTrigger.ChildContent), (RenderFragment)(item => item.AddContent(0, "Share")));
                            sub.CloseComponent();

                            sub.OpenComponent<BradixContextMenuPortal>(3);
                            sub.AddAttribute(4, nameof(BradixContextMenuPortal.ChildContent), (RenderFragment)(subPortal =>
                            {
                                subPortal.OpenComponent<BradixContextMenuSubContent>(0);
                                subPortal.AddAttribute(1, nameof(BradixContextMenuSubContent.ChildContent), (RenderFragment)(submenu =>
                                {
                                    submenu.OpenComponent<BradixContextMenuItem>(0);
                                    submenu.AddAttribute(1, nameof(BradixContextMenuItem.TextValue), "Copy link");
                                    submenu.AddAttribute(2, nameof(BradixContextMenuItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Copy link")));
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
