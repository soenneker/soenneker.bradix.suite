using System.Collections.Generic;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixMenubarRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixMenubarRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("updateDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayer", _ => true).SetVoidResult();
        _module.SetupVoid("registerDismissableLayerBranch", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDismissableLayerBranch", _ => true).SetVoidResult();
        _module.SetupVoid("registerMenubarDocumentDismiss", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterMenubarDocumentDismiss", _ => true).SetVoidResult();
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
        _module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("registerRovingFocusNavigationKeys", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterRovingFocusNavigationKeys", _ => true).SetVoidResult();
        _module.SetupVoid("focusElementPreventScroll", _ => true).SetVoidResult();
        _module.Setup<bool>("isKeyboardInteractionMode", _ => true).SetResult(false);
        _module.Setup<string>("getTextContent", _ => true).SetResult("Share");
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Arrow_down_on_trigger_opens_associated_menu()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenubar());
        IReadOnlyList<IElement> triggers = cut.FindAll("button[role='menuitem']");
        string closedControls = await Assert.That(triggers[0].GetAttribute("aria-controls")).IsTypeOf<string>();

        await triggers[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedTriggers = cut.FindAll("button[role='menuitem']");
            IElement menu = cut.Find("[role='menu']");
            await Assert.That(updatedTriggers[0].GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(updatedTriggers[0].GetAttribute("aria-controls")).IsEqualTo(closedControls);
            await Assert.That(updatedTriggers[0].GetAttribute("aria-controls")).IsEqualTo(menu.Id);
            await Assert.That(menu.GetAttribute("aria-labelledby")).IsEqualTo(updatedTriggers[0].Id);
        });
    }

    [Test]
    public async Task Pointer_down_on_trigger_opens_associated_menu()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenubar());
        IReadOnlyList<IElement> triggers = cut.FindAll("button[role='menuitem']");
        string closedControls = await Assert.That(triggers[0].GetAttribute("aria-controls")).IsTypeOf<string>();

        await triggers[0].TriggerEventAsync("onpointerdown", new PointerEventArgs { Button = 0 });

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedTriggers = cut.FindAll("button[role='menuitem']");
            IElement menu = cut.Find("[role='menu']");
            await Assert.That(updatedTriggers[0].GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(updatedTriggers[0].GetAttribute("aria-controls")).IsEqualTo(closedControls);
            await Assert.That(updatedTriggers[0].GetAttribute("aria-controls")).IsEqualTo(menu.Id);
            await Assert.That(menu.GetAttribute("aria-labelledby")).IsEqualTo(updatedTriggers[0].Id);
        });
    }

    [Test]
    public async Task Trigger_arrow_right_moves_roving_tab_stop()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenubar());
        IReadOnlyList<IElement> triggers = cut.FindAll("button[role='menuitem']");

        await triggers[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedTriggers = cut.FindAll("button[role='menuitem']");
            await Assert.That(updatedTriggers[1].GetAttribute("tabindex")).IsEqualTo("0");
            await Assert.That(updatedTriggers[0].GetAttribute("tabindex")).IsEqualTo("-1");
        });
    }

    [Test]
    public async Task Menubar_root_exposes_horizontal_orientation()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenubar());

        await Assert.That(cut.Find("[role='menubar']").GetAttribute("aria-orientation")).IsEqualTo("horizontal");
    }

    [Test]
    public async Task Content_arrow_right_opens_adjacent_menu()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenubar());
        IReadOnlyList<IElement> triggers = cut.FindAll("button[role='menuitem']");

        await triggers[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
        IRenderedComponent<BradixMenubarContent> content = FindOpenContent(cut);
        string currentContentId = await Assert.That(cut.Find("[role='menu']").Id).IsTypeOf<string>();

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "ArrowRight",
            ClosestMenubarContentId = currentContentId
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedTriggers = cut.FindAll("button[role='menuitem']");
            await Assert.That(updatedTriggers[1].GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(updatedTriggers[0].GetAttribute("aria-expanded")).IsEqualTo("false");
            await Assert.That(updatedTriggers[1].GetAttribute("tabindex")).IsEqualTo("0");
            await Assert.That(updatedTriggers[0].GetAttribute("tabindex")).IsEqualTo("-1");
        });
    }

    [Test]
    public async Task Menubar_content_root_character_key_runs_typeahead()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenubar());
        await cut.FindAll("button[role='menuitem']")[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
        IRenderedComponent<BradixMenubarContent> content = FindOpenContent(cut);
        string currentContentId = await Assert.That(cut.Find("[role='menu']").Id).IsTypeOf<string>();

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "s",
            TargetId = currentContentId
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            IHtmlCollection<IElement> items = cut.Find("[role='menu']").QuerySelectorAll("[role='menuitem']");
            await Assert.That(items[1].GetAttribute("tabindex")).IsEqualTo("0");
        });
    }

    [Test]
    public async Task Menubar_interactions_do_not_query_active_element_js()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenubar());
        await cut.FindAll("button[role='menuitem']")[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
        IRenderedComponent<BradixMenubarContent> content = FindOpenContent(cut);
        string currentContentId = await Assert.That(cut.Find("[role='menu']").Id).IsTypeOf<string>();

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "ArrowRight",
            ClosestMenubarContentId = currentContentId
        }));

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "getActiveElementId")).IsFalse();
    }

    [Test]
    public async Task Menubar_content_arrow_right_ignores_nested_subtrigger_navigation()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenubar());
        await cut.FindAll("button[role='menuitem']")[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
        IRenderedComponent<BradixMenubarContent> content = FindOpenContent(cut);

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "ArrowRight",
            IsMenubarSubTrigger = true
        }));

        IReadOnlyList<IElement> updatedTriggers = cut.FindAll("button[role='menuitem']");
        await Assert.That(updatedTriggers[0].GetAttribute("aria-expanded")).IsEqualTo("true");
        await Assert.That(updatedTriggers[1].GetAttribute("aria-expanded")).IsEqualTo("false");
    }

    [Test]
    public async Task Menubar_content_focus_outside_resets_typeahead_buffer()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenubar());
        await cut.FindAll("button[role='menuitem']")[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
        IRenderedComponent<BradixMenubarContent> content = FindOpenContent(cut);
        string currentContentId = await Assert.That(cut.Find("[role='menu']").Id).IsTypeOf<string>();
        IHtmlCollection<IElement> items = cut.Find("[role='menu']").QuerySelectorAll("[role='menuitem']");

        await items[0].KeyDownAsync(new KeyboardEventArgs { Key = "s" });

        await cut.WaitForAssertionAsync(async () =>
        {
            items = cut.Find("[role='menu']").QuerySelectorAll("[role='menuitem']");
            await Assert.That(items[1].GetAttribute("tabindex")).IsEqualTo("0");
        });

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentFocusOut(new BradixDelegatedFocusEvent
        {
            TargetId = currentContentId,
            RelatedTargetId = "outside-target"
        }));

        items = cut.Find("[role='menu']").QuerySelectorAll("[role='menuitem']");
        await items[1].KeyDownAsync(new KeyboardEventArgs { Key = "f" });

        await cut.WaitForAssertionAsync(async () =>
        {
            items = cut.Find("[role='menu']").QuerySelectorAll("[role='menuitem']");
            await Assert.That(items[0].GetAttribute("tabindex")).IsEqualTo("0");
        });
    }

    [Test]
    public async Task Checkbox_and_radio_wrappers_render_checked_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenubar());
        await cut.FindAll("button[role='menuitem']")[1].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("[role='menuitemcheckbox']").GetAttribute("aria-checked")).IsEqualTo("mixed");
        });

        IRenderedComponent<BradixMenubarContent> content = FindOpenContent(cut);
        string currentContentId = await Assert.That(cut.Find("[role='menu']").Id).IsTypeOf<string>();

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDown(new BradixDelegatedKeyboardEvent
        {
            Key = "ArrowRight",
            ClosestMenubarContentId = currentContentId
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> radioItems = cut.FindAll("[role='menuitemradio']");
            await Assert.That(radioItems.Any(item => item.GetAttribute("aria-checked") == "true")).IsTrue();
            await Assert.That(radioItems.Any(item => item.GetAttribute("aria-checked") == "false")).IsTrue();
        });
    }

    [Test]
    public async Task Submenu_wrapper_opens_from_sub_trigger()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMenubar());
        await cut.FindAll("button[role='menuitem']")[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
        IElement subTrigger = cut.Find("[data-radix-menubar-subtrigger]");

        await subTrigger.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("[data-radix-menubar-subtrigger]").GetAttribute("aria-expanded")).IsEqualTo("true");
            await Assert.That(cut.Markup).Contains("Copy link");
        });
    }

    [Test]
    public async Task Detailed_escape_keydown_can_prevent_menubar_content_dismiss()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixMenubar>(0);
            builder.AddAttribute(1, nameof(BradixMenubar.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixMenubarMenu>(0);
                content.AddAttribute(1, nameof(BradixMenubarMenu.Value), "file");
                content.AddAttribute(2, nameof(BradixMenubarMenu.ChildContent), (RenderFragment)(menu =>
                {
                    menu.OpenComponent<BradixMenubarTrigger>(0);
                    menu.AddAttribute(1, nameof(BradixMenubarTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, "File")));
                    menu.CloseComponent();

                    menu.OpenComponent<BradixMenubarPortal>(2);
                    menu.AddAttribute(3, nameof(BradixMenubarPortal.ChildContent), (RenderFragment)(portal =>
                    {
                        portal.OpenComponent<BradixMenubarContent>(0);
                        portal.AddAttribute(1, nameof(BradixMenubarContent.OnEscapeKeyDownDetailed),
                            EventCallback.Factory.Create<BradixEscapeKeyDownEventArgs>(this, args => args.PreventDefault()));
                        portal.AddAttribute(2, nameof(BradixMenubarContent.ChildContent), (RenderFragment)(menuContent =>
                        {
                            menuContent.OpenComponent<BradixMenubarItem>(0);
                            menuContent.AddAttribute(1, nameof(BradixMenubarItem.TextValue), "Open");
                            menuContent.AddAttribute(2, nameof(BradixMenubarItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Open")));
                            menuContent.CloseComponent();
                        }));
                        portal.CloseComponent();
                    }));
                    menu.CloseComponent();
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });

        await cut.Find("button[role='menuitem']").KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
        IRenderedComponent<BradixDismissableLayer> layer = cut.FindComponent<BradixDismissableLayer>();

        bool prevented = await cut.InvokeAsync(() => layer.Instance.HandleEscapeKeyDown());

        await Assert.That(prevented).IsFalse();
        await Assert.That(cut.FindAll("[role='menu']")).HasSingleItem();
    }

    private static RenderFragment CreateMenubar()
    {
        return builder =>
        {
            builder.OpenComponent<BradixMenubar>(0);
            builder.AddAttribute(1, nameof(BradixMenubar.ChildContent), (RenderFragment)(content =>
            {
                BuildMenu(content, 0, "file", "File", submenu: true, includeCheckbox: false, includeRadio: false);
                BuildMenu(content, 100, "edit", "Edit", submenu: false, includeCheckbox: true, includeRadio: false);
                BuildMenu(content, 200, "view", "View", submenu: false, includeCheckbox: false, includeRadio: true);
            }));
            builder.CloseComponent();
        };
    }

    private static void BuildMenu(RenderTreeBuilder builder, int sequence, string value, string label, bool submenu, bool includeCheckbox, bool includeRadio)
    {
        builder.OpenComponent<BradixMenubarMenu>(sequence);
        builder.AddAttribute(sequence + 1, nameof(BradixMenubarMenu.Value), value);
        builder.AddAttribute(sequence + 2, nameof(BradixMenubarMenu.ChildContent), (RenderFragment)(menu =>
        {
            menu.OpenComponent<BradixMenubarTrigger>(0);
            menu.AddAttribute(1, nameof(BradixMenubarTrigger.ChildContent), (RenderFragment)(trigger => trigger.AddContent(0, label)));
            menu.CloseComponent();

            menu.OpenComponent<BradixMenubarPortal>(2);
            menu.AddAttribute(3, nameof(BradixMenubarPortal.ChildContent), (RenderFragment)(portal =>
            {
                portal.OpenComponent<BradixMenubarContent>(0);
                portal.AddAttribute(1, nameof(BradixMenubarContent.ChildContent), (RenderFragment)(menuContent =>
                {
                    menuContent.OpenComponent<BradixMenubarItem>(0);
                    menuContent.AddAttribute(1, nameof(BradixMenubarItem.TextValue), $"{label} action");
                    menuContent.AddAttribute(2, nameof(BradixMenubarItem.ChildContent), (RenderFragment)(item => item.AddContent(0, $"{label} action")));
                    menuContent.CloseComponent();

                    if (submenu)
                    {
                        menuContent.OpenComponent<BradixMenubarSub>(3);
                        menuContent.AddAttribute(4, nameof(BradixMenubarSub.ChildContent), (RenderFragment)(sub =>
                        {
                            sub.OpenComponent<BradixMenubarSubTrigger>(0);
                            sub.AddAttribute(1, nameof(BradixMenubarSubTrigger.TextValue), "Share");
                            sub.AddAttribute(2, nameof(BradixMenubarSubTrigger.ChildContent), (RenderFragment)(item => item.AddContent(0, "Share")));
                            sub.CloseComponent();

                            sub.OpenComponent<BradixMenubarPortal>(3);
                            sub.AddAttribute(4, nameof(BradixMenubarPortal.ChildContent), (RenderFragment)(subPortal =>
                            {
                                subPortal.OpenComponent<BradixMenubarSubContent>(0);
                                subPortal.AddAttribute(1, nameof(BradixMenubarSubContent.ChildContent), (RenderFragment)(submenuContent =>
                                {
                                    submenuContent.OpenComponent<BradixMenubarItem>(0);
                                    submenuContent.AddAttribute(1, nameof(BradixMenubarItem.TextValue), "Copy link");
                                    submenuContent.AddAttribute(2, nameof(BradixMenubarItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Copy link")));
                                    submenuContent.CloseComponent();
                                }));
                                subPortal.CloseComponent();
                            }));
                            sub.CloseComponent();
                        }));
                        menuContent.CloseComponent();
                    }

                    if (includeCheckbox)
                    {
                        menuContent.OpenComponent<BradixMenubarCheckboxItem>(10);
                        menuContent.AddAttribute(11, nameof(BradixMenubarCheckboxItem.DefaultChecked), (object) BradixCheckboxCheckedState.Indeterminate);
                        menuContent.AddAttribute(12, nameof(BradixMenubarCheckboxItem.CloseOnSelect), false);
                        menuContent.AddAttribute(13, nameof(BradixMenubarCheckboxItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Show bookmarks")));
                        menuContent.CloseComponent();
                    }

                    if (includeRadio)
                    {
                        menuContent.OpenComponent<BradixMenubarRadioGroup>(20);
                        menuContent.AddAttribute(21, nameof(BradixMenubarRadioGroup.DefaultValue), "name");
                        menuContent.AddAttribute(22, nameof(BradixMenubarRadioGroup.ChildContent), (RenderFragment)(group =>
                        {
                            group.OpenComponent<BradixMenubarRadioItem>(0);
                            group.AddAttribute(1, nameof(BradixMenubarRadioItem.Value), "name");
                            group.AddAttribute(2, nameof(BradixMenubarRadioItem.CloseOnSelect), false);
                            group.AddAttribute(3, nameof(BradixMenubarRadioItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Name")));
                            group.CloseComponent();

                            group.OpenComponent<BradixMenubarRadioItem>(4);
                            group.AddAttribute(5, nameof(BradixMenubarRadioItem.Value), "date");
                            group.AddAttribute(6, nameof(BradixMenubarRadioItem.CloseOnSelect), false);
                            group.AddAttribute(7, nameof(BradixMenubarRadioItem.ChildContent), (RenderFragment)(item => item.AddContent(0, "Date modified")));
                            group.CloseComponent();
                        }));
                        menuContent.CloseComponent();
                    }
                }));
                portal.CloseComponent();
            }));
            menu.CloseComponent();
        }));
        builder.CloseComponent();
    }

    private static IRenderedComponent<BradixMenubarContent> FindOpenContent(IRenderedComponent<ContainerFragment> cut)
    {
        return cut.FindComponents<BradixMenubarContent>()
            .First(component => component.FindAll("[role='menu']").Count > 0);
    }
}
