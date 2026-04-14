using System.Collections.Generic;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit.Rendering;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixToggleGroupRenderTests : BunitContext
{
    public BradixToggleGroupRenderTests()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerRovingFocusNavigationKeys", _ => true).SetVoidResult();
        module.SetupVoid("unregisterRovingFocusNavigationKeys", _ => true).SetVoidResult();
        module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        module.SetupVoid("unregisterDelegatedInteraction", _ => true).SetVoidResult();
        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Single_group_toggles_between_values_and_radio_semantics()
    {
        string? requestedValue = null;

        IRenderedComponent<ContainerFragment> cut = Render(CreateSingleGroup(EventCallback.Factory.Create<string?>(this, value => requestedValue = value)));
        IElement group = cut.Find("[role='radiogroup']");

        IReadOnlyList<IElement> buttons = cut.FindAll("button");

        await Assert.That(group.GetAttribute("aria-orientation")).IsEqualTo("horizontal");
        await Assert.That(buttons[0].GetAttribute("role")).IsEqualTo("radio");
        await Assert.That(buttons[0].GetAttribute("aria-checked")).IsEqualTo("false");

        await buttons[0].ClickAsync();
        buttons = cut.FindAll("button");

        await Assert.That(requestedValue).IsEqualTo("one");
        await Assert.That(buttons[0].GetAttribute("aria-checked")).IsEqualTo("true");
        await Assert.That(buttons[0].GetAttribute("data-state")).IsEqualTo("on");

        await buttons[0].ClickAsync();

        await Assert.That(requestedValue).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task Multiple_group_accumulates_values_and_removes_them_independently()
    {
        IReadOnlyCollection<string>? requestedValues = null;

        IRenderedComponent<ContainerFragment> cut = Render(CreateMultipleGroup(EventCallback.Factory.Create<IReadOnlyCollection<string>>(this, values => requestedValues = values)));
        await Assert.That(cut.Find("[role='group']")).IsNotNull();

        IReadOnlyList<IElement> buttons = cut.FindAll("button");

        await buttons[0].ClickAsync();
        await buttons[1].ClickAsync();

        await Assert.That(string.Join(",", requestedValues!)).IsEqualTo("one,two");

        buttons = cut.FindAll("button");
        await buttons[0].ClickAsync();

        await Assert.That(string.Join(",", requestedValues!)).IsEqualTo("two");
    }

    [Test]
    public async Task Roving_focus_moves_current_tab_stop_with_arrow_keys()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSingleGroup());

        IReadOnlyList<IElement> buttons = cut.FindAll("button");
        await Assert.That(buttons[0].GetAttribute("tabindex")).IsEqualTo("0");
        await Assert.That(buttons[1].GetAttribute("tabindex")).IsEqualTo("-1");

        await buttons[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });
        buttons = cut.FindAll("button");

        await Assert.That(buttons[0].GetAttribute("tabindex")).IsEqualTo("-1");
        await Assert.That(buttons[1].GetAttribute("tabindex")).IsEqualTo("0");
    }

    [Test]
    public async Task Direction_provider_flips_horizontal_navigation()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, nameof(BradixDirectionProvider.Dir), "rtl");
            builder.AddAttribute(2, nameof(BradixDirectionProvider.ChildContent), (RenderFragment) (contentBuilder =>
            {
                CreateSingleGroup()(contentBuilder);
            }));
            builder.CloseComponent();
        });

        IReadOnlyList<IElement> buttons = cut.FindAll("button");
        await buttons[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });
        buttons = cut.FindAll("button");

        await Assert.That(buttons[0].GetAttribute("tabindex")).IsEqualTo("-1");
        await Assert.That(buttons[1].GetAttribute("tabindex")).IsEqualTo("0");
    }

    private static RenderFragment CreateSingleGroup(EventCallback<string?> onValueChange = default)
    {
        return builder =>
        {
            builder.OpenComponent<BradixToggleGroup>(0);
            builder.AddAttribute(1, nameof(BradixToggleGroup.Type), (object) BradixSelectionMode.Single);
            builder.AddAttribute(2, nameof(BradixToggleGroup.Orientation), (object) BradixOrientation.Horizontal);

            if (onValueChange.HasDelegate)
                builder.AddAttribute(3, nameof(BradixToggleGroup.OnValueChange), onValueChange);

            builder.AddAttribute(4, nameof(BradixToggleGroup.ChildContent), (RenderFragment) (contentBuilder =>
            {
                RenderItem(contentBuilder, 0, "one");
                RenderItem(contentBuilder, 10, "two");
                RenderItem(contentBuilder, 20, "three");
            }));
            builder.CloseComponent();
        };
    }

    private static RenderFragment CreateMultipleGroup(EventCallback<IReadOnlyCollection<string>> onValuesChange = default)
    {
        return builder =>
        {
            builder.OpenComponent<BradixToggleGroup>(0);
            builder.AddAttribute(1, nameof(BradixToggleGroup.Type), (object) BradixSelectionMode.Multiple);

            if (onValuesChange.HasDelegate)
                builder.AddAttribute(2, nameof(BradixToggleGroup.OnValuesChange), onValuesChange);

            builder.AddAttribute(3, nameof(BradixToggleGroup.ChildContent), (RenderFragment) (contentBuilder =>
            {
                RenderItem(contentBuilder, 0, "one");
                RenderItem(contentBuilder, 10, "two");
                RenderItem(contentBuilder, 20, "three");
            }));
            builder.CloseComponent();
        };
    }

    private static void RenderItem(RenderTreeBuilder builder, int sequence, string value)
    {
        builder.OpenComponent<BradixToggleGroupItem>(sequence);
        builder.AddAttribute(sequence + 1, nameof(BradixToggleGroupItem.Value), value);
        builder.AddAttribute(sequence + 2, nameof(BradixToggleGroupItem.ChildContent), (RenderFragment) (contentBuilder =>
        {
            contentBuilder.AddContent(0, value);
        }));
        builder.CloseComponent();
    }
}