using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

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

    [Fact]
    public void Toolbar_renders_role_and_separator_orientation()
    {
        var cut = Render(CreateToolbar());

        var toolbar = cut.Find("[role='toolbar']");
        var separator = cut.Find("[data-orientation='vertical']");

        Assert.Equal("horizontal", toolbar.GetAttribute("aria-orientation"));
        Assert.Equal("separator", separator.GetAttribute("role"));
        Assert.Equal("vertical", separator.GetAttribute("aria-orientation"));
    }

    [Fact]
    public void Toolbar_roving_focus_skips_disabled_items()
    {
        var cut = Render(CreateToolbar());

        var buttons = cut.FindAll("button");
        buttons[0].KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        var link = cut.Find("a");
        Assert.Equal("0", link.GetAttribute("tabindex"));
    }

    [Fact]
    public void Toolbar_toggle_group_updates_pressed_state()
    {
        var cut = Render(CreateToolbar());

        var radios = cut.FindAll("[role='radio']");
        Assert.Equal("horizontal", cut.Find("[role='radiogroup']").GetAttribute("aria-orientation"));
        radios[2].Click();
        radios = cut.FindAll("[role='radio']");

        Assert.Equal("false", radios[0].GetAttribute("aria-checked"));
        Assert.Equal("true", radios[2].GetAttribute("aria-checked"));
    }

    [Fact]
    public void Inherited_direction_flips_horizontal_navigation()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, nameof(BradixDirectionProvider.Dir), "rtl");
            builder.AddAttribute(2, nameof(BradixDirectionProvider.ChildContent), (RenderFragment) (contentBuilder =>
            {
                CreateSimpleToolbar()(contentBuilder);
            }));
            builder.CloseComponent();
        });

        var buttons = cut.FindAll("button");
        buttons[0].KeyDown(new KeyboardEventArgs { Key = "ArrowLeft" });
        buttons = cut.FindAll("button");

        Assert.Equal("0", buttons[1].GetAttribute("tabindex"));
    }

    [Fact]
    public void Toolbar_link_space_key_invokes_click_interop()
    {
        var cut = Render(CreateToolbar());

        var link = cut.Find("a");
        link.KeyDown(new KeyboardEventArgs { Key = " " });

        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "clickElement");
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
