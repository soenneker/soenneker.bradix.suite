using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

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
        _module.SetupVoid("focusElementPreventScroll", _ => true).SetVoidResult();
        _module.Setup<bool>("isKeyboardInteractionMode", _ => true).SetResult(false);
        _module.Setup<string>("getTextContent", _ => true).SetResult("Share");
        _module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "fade-out", Display = "block" });

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
    }

    [Fact]
    public void Arrow_down_on_trigger_opens_associated_menu()
    {
        var cut = Render(CreateMenubar());
        var triggers = cut.FindAll("button[role='menuitem']");
        string closedControls = Assert.IsType<string>(triggers[0].GetAttribute("aria-controls"));

        triggers[0].KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });

        cut.WaitForAssertion(() =>
        {
            var updatedTriggers = cut.FindAll("button[role='menuitem']");
            var menu = cut.Find("[role='menu']");
            Assert.Equal("true", updatedTriggers[0].GetAttribute("aria-expanded"));
            Assert.Equal(closedControls, updatedTriggers[0].GetAttribute("aria-controls"));
            Assert.Equal(menu.Id, updatedTriggers[0].GetAttribute("aria-controls"));
            Assert.Equal(updatedTriggers[0].Id, menu.GetAttribute("aria-labelledby"));
        });
    }

    [Fact]
    public void Trigger_arrow_right_moves_roving_tab_stop()
    {
        var cut = Render(CreateMenubar());
        var triggers = cut.FindAll("button[role='menuitem']");

        triggers[0].KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        cut.WaitForAssertion(() =>
        {
            var updatedTriggers = cut.FindAll("button[role='menuitem']");
            Assert.Equal("0", updatedTriggers[1].GetAttribute("tabindex"));
            Assert.Equal("-1", updatedTriggers[0].GetAttribute("tabindex"));
        });
    }

    [Fact]
    public void Menubar_root_exposes_horizontal_orientation()
    {
        var cut = Render(CreateMenubar());

        Assert.Equal("horizontal", cut.Find("[role='menubar']").GetAttribute("aria-orientation"));
    }

    [Fact]
    public async Task Content_arrow_right_opens_adjacent_menu()
    {
        var cut = Render(CreateMenubar());
        var triggers = cut.FindAll("button[role='menuitem']");

        triggers[0].KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        var content = FindOpenContent(cut);
        string currentContentId = Assert.IsType<string>(cut.Find("[role='menu']").Id);

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDownAsync(new BradixDelegatedKeyboardEvent
        {
            Key = "ArrowRight",
            ClosestMenubarContentId = currentContentId
        }));

        cut.WaitForAssertion(() =>
        {
            var updatedTriggers = cut.FindAll("button[role='menuitem']");
            Assert.Equal("true", updatedTriggers[1].GetAttribute("aria-expanded"));
            Assert.Equal("false", updatedTriggers[0].GetAttribute("aria-expanded"));
            Assert.Equal("0", updatedTriggers[1].GetAttribute("tabindex"));
            Assert.Equal("-1", updatedTriggers[0].GetAttribute("tabindex"));
        });
    }

    [Fact]
    public async Task Menubar_content_root_character_key_runs_typeahead()
    {
        var cut = Render(CreateMenubar());
        cut.FindAll("button[role='menuitem']")[0].KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        var content = FindOpenContent(cut);
        string currentContentId = Assert.IsType<string>(cut.Find("[role='menu']").Id);

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDownAsync(new BradixDelegatedKeyboardEvent
        {
            Key = "s",
            TargetId = currentContentId
        }));

        cut.WaitForAssertion(() =>
        {
            var items = cut.Find("[role='menu']").QuerySelectorAll("[role='menuitem']");
            Assert.Equal("0", items[1].GetAttribute("tabindex"));
        });
    }

    [Fact]
    public async Task Menubar_interactions_do_not_query_active_element_js()
    {
        var cut = Render(CreateMenubar());
        cut.FindAll("button[role='menuitem']")[0].KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        var content = FindOpenContent(cut);
        string currentContentId = Assert.IsType<string>(cut.Find("[role='menu']").Id);

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDownAsync(new BradixDelegatedKeyboardEvent
        {
            Key = "ArrowRight",
            ClosestMenubarContentId = currentContentId
        }));

        Assert.DoesNotContain(_module.Invocations, invocation => invocation.Identifier == "getActiveElementId");
    }

    [Fact]
    public async Task Menubar_content_arrow_right_ignores_nested_subtrigger_navigation()
    {
        var cut = Render(CreateMenubar());
        cut.FindAll("button[role='menuitem']")[0].KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        var content = FindOpenContent(cut);

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDownAsync(new BradixDelegatedKeyboardEvent
        {
            Key = "ArrowRight",
            IsMenubarSubTrigger = true
        }));

        var updatedTriggers = cut.FindAll("button[role='menuitem']");
        Assert.Equal("true", updatedTriggers[0].GetAttribute("aria-expanded"));
        Assert.Equal("false", updatedTriggers[1].GetAttribute("aria-expanded"));
    }

    [Fact]
    public async Task Menubar_content_focus_outside_resets_typeahead_buffer()
    {
        var cut = Render(CreateMenubar());
        cut.FindAll("button[role='menuitem']")[0].KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        var content = FindOpenContent(cut);
        string currentContentId = Assert.IsType<string>(cut.Find("[role='menu']").Id);
        var items = cut.Find("[role='menu']").QuerySelectorAll("[role='menuitem']");

        items[0].KeyDown(new KeyboardEventArgs { Key = "s" });

        cut.WaitForAssertion(() =>
        {
            items = cut.Find("[role='menu']").QuerySelectorAll("[role='menuitem']");
            Assert.Equal("0", items[1].GetAttribute("tabindex"));
        });

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentFocusOutAsync(new BradixDelegatedFocusEvent
        {
            TargetId = currentContentId,
            RelatedTargetId = "outside-target"
        }));

        items = cut.Find("[role='menu']").QuerySelectorAll("[role='menuitem']");
        items[1].KeyDown(new KeyboardEventArgs { Key = "f" });

        cut.WaitForAssertion(() =>
        {
            items = cut.Find("[role='menu']").QuerySelectorAll("[role='menuitem']");
            Assert.Equal("0", items[0].GetAttribute("tabindex"));
        });
    }

    [Fact]
    public async Task Checkbox_and_radio_wrappers_render_checked_state()
    {
        var cut = Render(CreateMenubar());
        cut.FindAll("button[role='menuitem']")[1].KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("mixed", cut.Find("[role='menuitemcheckbox']").GetAttribute("aria-checked"));
        });

        var content = FindOpenContent(cut);
        string currentContentId = Assert.IsType<string>(cut.Find("[role='menu']").Id);

        await cut.InvokeAsync(() => content.Instance.HandleDelegatedContentKeyDownAsync(new BradixDelegatedKeyboardEvent
        {
            Key = "ArrowRight",
            ClosestMenubarContentId = currentContentId
        }));

        cut.WaitForAssertion(() =>
        {
            var radioItems = cut.FindAll("[role='menuitemradio']");
            Assert.Contains(radioItems, item => item.GetAttribute("aria-checked") == "true");
            Assert.Contains(radioItems, item => item.GetAttribute("aria-checked") == "false");
        });
    }

    [Fact]
    public void Submenu_wrapper_opens_from_sub_trigger()
    {
        var cut = Render(CreateMenubar());
        cut.FindAll("button[role='menuitem']")[0].KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        var subTrigger = cut.Find("[data-radix-menubar-subtrigger]");

        subTrigger.KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("true", cut.Find("[data-radix-menubar-subtrigger]").GetAttribute("aria-expanded"));
            Assert.Contains("Copy link", cut.Markup);
        });
    }

    [Fact]
    public async Task Detailed_escape_keydown_can_prevent_menubar_content_dismiss()
    {
        var cut = Render(builder =>
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

        cut.Find("button[role='menuitem']").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        var layer = cut.FindComponent<BradixDismissableLayer>();

        bool prevented = await cut.InvokeAsync(() => layer.Instance.HandleEscapeKeyDownAsync());

        Assert.False(prevented);
        Assert.Single(cut.FindAll("[role='menu']"));
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
                        menuContent.AddAttribute(11, nameof(BradixMenubarCheckboxItem.DefaultChecked), BradixCheckboxCheckedState.Indeterminate);
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
