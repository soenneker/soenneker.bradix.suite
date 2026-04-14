using System.Collections.Generic;
using System.Linq;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit.Rendering;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixRadioGroupRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixRadioGroupRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.Setup<bool>("isFormControl", invocation =>
                invocation.Arguments.Count > 1 && invocation.Arguments[1] is string formId && !string.IsNullOrWhiteSpace(formId))
            .SetResult(true);
        _module.Setup<bool>("isFormControl", _ => true).SetResult(false);
        _module.SetupVoid("registerRovingFocusNavigationKeys", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterRovingFocusNavigationKeys", _ => true).SetVoidResult();
        _module.SetupVoid("registerRadioGroupItemKeys", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterRadioGroupItemKeys", _ => true).SetVoidResult();
        _module.SetupVoid("syncCheckboxBubbleInputState", _ => true).SetVoidResult();
        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Radio_group_renders_checked_state_without_hidden_input_outside_form()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateRadioGroup(defaultValue: "one", name: "plan", required: true));

        IElement group = cut.Find("[role='radiogroup']");
        IReadOnlyList<IElement> buttons = cut.FindAll("button");

        await Assert.That(group.GetAttribute("aria-required")).IsEqualTo("true");
        await Assert.That(buttons[0].GetAttribute("aria-checked")).IsEqualTo("true");
        await Assert.That(buttons[0].GetAttribute("data-state")).IsEqualTo("checked");
        await Assert.That(cut.FindAll("input[type='radio']")).IsEmpty();
    }

    [Test]
    public async Task Radio_item_with_explicit_form_renders_hidden_input_outside_form()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateRadioGroup(defaultValue: "one", name: "plan", itemForm: "settings-form"));

        IReadOnlyList<IElement> inputs = cut.FindAll("input[type='radio']");
        await Assert.That(inputs.Count).IsEqualTo(3);
        await Assert.That(inputs[0].GetAttribute("form")).IsEqualTo("settings-form");
        await Assert.That(_module.Invocations.Any(invocation =>
            invocation.Identifier == "isFormControl" &&
            invocation.Arguments.Count > 1 &&
            Equals(invocation.Arguments[1], "settings-form"))).IsTrue();
    }

    [Test]
    public async Task Clicking_item_updates_uncontrolled_selection()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateRadioGroup(defaultValue: "one"));

        IReadOnlyList<IElement> buttons = cut.FindAll("button");
        await buttons[2].ClickAsync();
        buttons = cut.FindAll("button");

        await Assert.That(buttons[0].GetAttribute("aria-checked")).IsEqualTo("false");
        await Assert.That(buttons[2].GetAttribute("aria-checked")).IsEqualTo("true");
    }

    [Test]
    public async Task Arrow_navigation_moves_focus_and_selects_target()
    {
        string? requestedValue = null;

        IRenderedComponent<ContainerFragment> cut = Render(CreateRadioGroup(
            defaultValue: "one",
            onValueChange: EventCallback.Factory.Create<string?>(this, value => requestedValue = value)));

        IReadOnlyList<IElement> buttons = cut.FindAll("button");
        await buttons[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });
        buttons = cut.FindAll("button");
        await buttons[2].FocusAsync();
        buttons = cut.FindAll("button");

        await Assert.That(requestedValue).IsEqualTo("three");
        await Assert.That(buttons[0].GetAttribute("tabindex")).IsEqualTo("-1");
        await Assert.That(buttons[2].GetAttribute("tabindex")).IsEqualTo("0");
        await Assert.That(buttons[2].GetAttribute("aria-checked")).IsEqualTo("true");
    }

    [Test]
    public async Task Enter_does_not_change_selection()
    {
        string? requestedValue = null;

        IRenderedComponent<ContainerFragment> cut = Render(CreateRadioGroup(
            defaultValue: "one",
            onValueChange: EventCallback.Factory.Create<string?>(this, value => requestedValue = value)));

        IReadOnlyList<IElement> buttons = cut.FindAll("button");
        await buttons[2].KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        await Assert.That(requestedValue).IsNull();
        await Assert.That(cut.Markup).Contains("one");
    }

    [Test]
    public async Task Force_mount_renders_all_indicators()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateRadioGroup(forceMountIndicator: true));

        await Assert.That(cut.FindAll("span").Count).IsEqualTo(3);
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
                CreateRadioGroup(defaultValue: "one", includeDisabledMiddle: false)(contentBuilder);
            }));
            builder.CloseComponent();
        });

        IReadOnlyList<IElement> buttons = cut.FindAll("button");
        await buttons[0].KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });
        buttons = cut.FindAll("button");
        await buttons[2].FocusAsync();
        buttons = cut.FindAll("button");

        await Assert.That(buttons[2].GetAttribute("aria-checked")).IsEqualTo("true");
    }

    private static RenderFragment CreateRadioGroup(string? defaultValue = null, string? name = null, bool required = false, EventCallback<string?> onValueChange = default, bool forceMountIndicator = false, bool includeDisabledMiddle = true, string? itemForm = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixRadioGroup>(0);

            if (defaultValue is not null)
                builder.AddAttribute(1, nameof(BradixRadioGroup.DefaultValue), defaultValue);

            if (name is not null)
                builder.AddAttribute(2, nameof(BradixRadioGroup.Name), name);

            builder.AddAttribute(3, nameof(BradixRadioGroup.Required), required);

            if (onValueChange.HasDelegate)
                builder.AddAttribute(4, nameof(BradixRadioGroup.OnValueChange), onValueChange);

            builder.AddAttribute(5, nameof(BradixRadioGroup.ChildContent), (RenderFragment) (contentBuilder =>
            {
                RenderItem(contentBuilder, 0, "one", forceMountIndicator, form: itemForm);
                RenderItem(contentBuilder, 10, "two", forceMountIndicator, includeDisabledMiddle, itemForm);
                RenderItem(contentBuilder, 20, "three", forceMountIndicator, form: itemForm);
            }));
            builder.CloseComponent();
        };
    }

    private static void RenderItem(RenderTreeBuilder builder, int sequence, string value, bool forceMountIndicator, bool disabled = false, string? form = null)
    {
        builder.OpenComponent<BradixRadioGroupItem>(sequence);
        builder.AddAttribute(sequence + 1, nameof(BradixRadioGroupItem.Value), value);
        builder.AddAttribute(sequence + 2, nameof(BradixRadioGroupItem.InputValue), value);
        builder.AddAttribute(sequence + 3, nameof(BradixRadioGroupItem.Disabled), disabled);
        if (form is not null)
            builder.AddAttribute(sequence + 4, nameof(BradixRadioGroupItem.Form), form);

        builder.AddAttribute(sequence + 5, nameof(BradixRadioGroupItem.ChildContent), (RenderFragment) (contentBuilder =>
        {
            contentBuilder.OpenComponent<BradixRadioGroupIndicator>(0);
            contentBuilder.AddAttribute(1, nameof(BradixRadioGroupIndicator.ForceMount), forceMountIndicator);
            contentBuilder.CloseComponent();
        }));
        builder.CloseComponent();
    }
}