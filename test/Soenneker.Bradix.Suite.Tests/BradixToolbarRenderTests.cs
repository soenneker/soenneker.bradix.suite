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

public sealed class BradixToolbarRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixToolbarRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerRovingFocusNavigationKeys", _ => true);
        _module.SetupVoid("unregisterRovingFocusNavigationKeys", _ => true);
        _module.SetupVoid("clickElement", _ => true);
        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Toolbar_renders_role_and_separator_orientation()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateToolbar());

        IElement toolbar = cut.Find("[role='toolbar']");
        IElement separator = cut.Find("[data-orientation='vertical']");

        await Assert.That(toolbar.GetAttribute("aria-orientation")).IsEqualTo("horizontal");
        await Assert.That(separator.GetAttribute("role")).IsEqualTo("separator");
        await Assert.That(separator.GetAttribute("aria-orientation")).IsEqualTo("vertical");
    }

    [Test]
    public async Task Toolbar_roving_focus_skips_disabled_items()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateToolbar());

        IReadOnlyList<IElement> buttons = cut.FindAll("button");
        await buttons[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

        IElement link = cut.Find("a");
        await Assert.That(link.GetAttribute("tabindex")).IsEqualTo("0");
    }

    [Test]
    public async Task Toolbar_toggle_group_updates_pressed_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateToolbar());

        IReadOnlyList<IElement> radios = cut.FindAll("[role='radio']");
        await Assert.That(cut.Find(".toolbar-single-group").GetAttribute("role")).IsEqualTo("group");
        await Assert.That(cut.Find(".toolbar-single-group").GetAttribute("aria-orientation")).IsEqualTo("horizontal");
        await radios[2].ClickAsync();
        radios = cut.FindAll("[role='radio']");

        await Assert.That(radios[0].GetAttribute("aria-checked")).IsEqualTo("false");
        await Assert.That(radios[2].GetAttribute("aria-checked")).IsEqualTo("true");
    }

    [Test]
    public async Task Toolbar_items_register_radix_roving_mousedown_guards_and_click_callbacks()
    {
        var buttonClicks = 0;
        var toggleClicks = 0;
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixToolbar>(0);
            builder.AddAttribute(1, nameof(BradixToolbar.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixToolbarButton>(0);
                contentBuilder.AddAttribute(1, nameof(BradixToolbarButton.OnClick), EventCallback.Factory.Create<MouseEventArgs>(this, () => buttonClicks++));
                contentBuilder.AddAttribute(2, nameof(BradixToolbarButton.ChildContent), (RenderFragment) (b => b.AddContent(0, "Button")));
                contentBuilder.CloseComponent();

                contentBuilder.OpenComponent<BradixToolbarToggleGroup>(10);
                contentBuilder.AddAttribute(11, nameof(BradixToolbarToggleGroup.ChildContent), (RenderFragment) (groupBuilder =>
                {
                    groupBuilder.OpenComponent<BradixToolbarToggleItem>(0);
                    groupBuilder.AddAttribute(1, nameof(BradixToolbarToggleItem.Value), "bold");
                    groupBuilder.AddAttribute(2, nameof(BradixToolbarToggleItem.OnClick), EventCallback.Factory.Create<MouseEventArgs>(this, () => toggleClicks++));
                    groupBuilder.AddAttribute(3, nameof(BradixToolbarToggleItem.ChildContent), (RenderFragment) (b => b.AddContent(0, "Bold")));
                    groupBuilder.CloseComponent();
                }));
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        IReadOnlyList<IElement> buttons = cut.FindAll("button");

        await Assert.That(buttons[0].GetAttribute("data-bradix-prevent-nonprimary-mousedown")).IsEqualTo("true");
        await Assert.That(buttons[0].GetAttribute("data-bradix-prevent-mousedown-when-disabled")).IsEqualTo("true");
        await Assert.That(buttons[1].GetAttribute("data-bradix-prevent-nonprimary-mousedown")).IsEqualTo("true");
        await Assert.That(buttons[1].GetAttribute("data-bradix-prevent-mousedown-when-disabled")).IsEqualTo("true");

        await buttons[0].ClickAsync();
        await buttons[1].ClickAsync();

        await Assert.That(buttonClicks).IsEqualTo(1);
        await Assert.That(toggleClicks).IsEqualTo(1);
    }

    [Test]
    public async Task Controlled_toolbar_toggle_group_with_null_value_does_not_mutate_internal_state()
    {
        string? reported = null;
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixToolbar>(0);
            builder.AddAttribute(1, nameof(BradixToolbar.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixToolbarToggleGroup>(0);
                contentBuilder.AddAttribute(1, nameof(BradixToolbarToggleGroup.Value), (string?) null);
                contentBuilder.AddAttribute(2, nameof(BradixToolbarToggleGroup.ValueChanged),
                    EventCallback.Factory.Create<string?>(this, value => reported = value));
                contentBuilder.AddAttribute(3, nameof(BradixToolbarToggleGroup.ChildContent), (RenderFragment) (groupBuilder =>
                {
                    RenderToggleItem(groupBuilder, 0, "left");
                    RenderToggleItem(groupBuilder, 10, "center");
                }));
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        IElement left = cut.FindAll("[role='radio']")[0];
        await left.ClickAsync();

        await Assert.That(reported).IsEqualTo("left");
        await Assert.That(cut.FindAll("[role='radio']")[0].GetAttribute("aria-checked")).IsEqualTo("false");
    }

    [Test]
    public async Task Inherited_direction_flips_horizontal_navigation()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, nameof(BradixDirectionProvider.Dir), "rtl");
            builder.AddAttribute(2, nameof(BradixDirectionProvider.ChildContent), (RenderFragment) (contentBuilder =>
            {
                CreateSimpleToolbar()(contentBuilder);
            }));
            builder.CloseComponent();
        });

        IReadOnlyList<IElement> buttons = cut.FindAll("button");
        await buttons[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });
        buttons = cut.FindAll("button");

        await Assert.That(buttons[1].GetAttribute("tabindex")).IsEqualTo("0");
    }

    [Test]
    public async Task Toolbar_link_space_key_invokes_click_interop()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateToolbar());

        IElement link = cut.Find("a");
        await link.KeyDownAsync(new KeyboardEventArgs { Key = " " });

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "clickElement")).IsTrue();
    }

    private static RenderFragment CreateToolbar()
    {
        return builder =>
        {
            builder.OpenComponent<BradixToolbar>(0);
            builder.AddAttribute(1, nameof(BradixToolbar.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixToolbarButton>(0);
                contentBuilder.AddAttribute(1, nameof(BradixToolbarButton.ChildContent), (RenderFragment) (b => b.AddContent(0, "Button")));
                contentBuilder.CloseComponent();

                contentBuilder.OpenComponent<BradixToolbarButton>(10);
                contentBuilder.AddAttribute(11, nameof(BradixToolbarButton.Disabled), true);
                contentBuilder.AddAttribute(12, nameof(BradixToolbarButton.ChildContent), (RenderFragment) (b => b.AddContent(0, "Disabled")));
                contentBuilder.CloseComponent();

                contentBuilder.OpenComponent<BradixToolbarSeparator>(20);
                contentBuilder.CloseComponent();

                contentBuilder.OpenComponent<BradixToolbarLink>(30);
                contentBuilder.AddAttribute(31, nameof(BradixToolbarLink.Href), "https://example.com");
                contentBuilder.AddAttribute(32, nameof(BradixToolbarLink.ChildContent), (RenderFragment) (b => b.AddContent(0, "Link")));
                contentBuilder.CloseComponent();

                contentBuilder.OpenComponent<BradixToolbarToggleGroup>(40);
                contentBuilder.AddAttribute(44, nameof(BradixToolbarToggleGroup.Class), "toolbar-single-group");
                contentBuilder.AddAttribute(41, nameof(BradixToolbarToggleGroup.Type), (object) BradixSelectionMode.Single);
                contentBuilder.AddAttribute(42, nameof(BradixToolbarToggleGroup.DefaultValue), "center");
                contentBuilder.AddAttribute(43, nameof(BradixToolbarToggleGroup.ChildContent), (RenderFragment) (groupBuilder =>
                {
                    RenderToggleItem(groupBuilder, 0, "left");
                    RenderToggleItem(groupBuilder, 10, "center");
                    RenderToggleItem(groupBuilder, 20, "right");
                }));
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }

    private static RenderFragment CreateSimpleToolbar()
    {
        return builder =>
        {
            builder.OpenComponent<BradixToolbar>(0);
            builder.AddAttribute(1, nameof(BradixToolbar.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixToolbarButton>(0);
                contentBuilder.AddAttribute(1, nameof(BradixToolbarButton.ChildContent), (RenderFragment) (b => b.AddContent(0, "One")));
                contentBuilder.CloseComponent();

                contentBuilder.OpenComponent<BradixToolbarButton>(10);
                contentBuilder.AddAttribute(11, nameof(BradixToolbarButton.ChildContent), (RenderFragment) (b => b.AddContent(0, "Two")));
                contentBuilder.CloseComponent();

                contentBuilder.OpenComponent<BradixToolbarButton>(20);
                contentBuilder.AddAttribute(21, nameof(BradixToolbarButton.ChildContent), (RenderFragment) (b => b.AddContent(0, "Three")));
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }

    private static void RenderToggleItem(RenderTreeBuilder builder, int sequence, string value)
    {
        builder.OpenComponent<BradixToolbarToggleItem>(sequence);
        builder.AddAttribute(sequence + 1, nameof(BradixToolbarToggleItem.Value), value);
        builder.AddAttribute(sequence + 2, nameof(BradixToolbarToggleItem.ChildContent), (RenderFragment) (contentBuilder =>
        {
            contentBuilder.AddContent(0, value);
        }));
        builder.CloseComponent();
    }
}
