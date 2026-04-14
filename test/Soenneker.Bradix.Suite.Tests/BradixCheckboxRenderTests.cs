using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixCheckboxRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixCheckboxRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.Setup<bool>("isFormControl", invocation =>
                invocation.Arguments.Count > 1 && invocation.Arguments[1] is string formId && !string.IsNullOrWhiteSpace(formId))
            .SetResult(true);
        _module.Setup<bool>("isFormControl", _ => true).SetResult(false);
        _module.SetupVoid("registerCheckboxRoot", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterCheckboxRoot", _ => true).SetVoidResult();
        _module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterDelegatedInteraction", _ => true).SetVoidResult();
        _module.SetupVoid("syncCheckboxBubbleInputState", _ => true).SetVoidResult();

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Checkbox_renders_unchecked_state_by_default()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateCheckbox());

        IElement button = cut.Find("button");

        await Assert.That(button.GetAttribute("aria-checked")).IsEqualTo("false");
        await Assert.That(button.GetAttribute("data-state")).IsEqualTo("unchecked");
    }

    [Test]
    public async Task Checkbox_click_cycles_indeterminate_to_checked()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateCheckbox(defaultChecked: BradixCheckboxCheckedState.Indeterminate));

        IElement button = cut.Find("button");
        await button.ClickAsync();
        button = cut.Find("button");

        await Assert.That(button.GetAttribute("aria-checked")).IsEqualTo("true");
        await Assert.That(button.GetAttribute("data-state")).IsEqualTo("checked");
    }

    [Test]
    public async Task Checkbox_can_render_mixed_state_when_controlled()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateCheckbox(checkedState: BradixCheckboxCheckedState.Indeterminate));

        IElement button = cut.Find("button");

        await Assert.That(button.GetAttribute("aria-checked")).IsEqualTo("mixed");
        await Assert.That(button.GetAttribute("data-state")).IsEqualTo("indeterminate");
    }

    [Test]
    public async Task Checkbox_indicator_force_mount_renders_when_unchecked()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateCheckbox(forceMountIndicator: true));

        IElement indicator = cut.Find("span");
        await Assert.That(indicator.GetAttribute("style")).Contains("pointer-events: none");
    }

    [Test]
    public async Task Checkbox_with_name_outside_form_does_not_render_hidden_input()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateCheckbox(defaultChecked: BradixCheckboxCheckedState.Checked, name: "terms"));
        await Assert.That(cut.FindAll("input[type='checkbox']")).IsEmpty();
    }

    [Test]
    public async Task Checkbox_with_explicit_form_renders_hidden_input_outside_form()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateCheckbox(defaultChecked: BradixCheckboxCheckedState.Checked, name: "terms", form: "settings-form"));

        IElement input = cut.Find("input[type='checkbox']");
        await Assert.That(input.GetAttribute("form")).IsEqualTo("settings-form");
        await Assert.That(_module.Invocations.Any(invocation =>
            invocation.Identifier == "registerCheckboxRoot" &&
            invocation.Arguments.Count > 2 &&
            Equals(invocation.Arguments[2], "settings-form"))).IsTrue();
    }

    [Test]
    public async Task Uncontrolled_checkbox_resets_to_initial_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateCheckbox(defaultChecked: BradixCheckboxCheckedState.Indeterminate));
        IRenderedComponent<BradixCheckbox> checkbox = cut.FindComponent<BradixCheckbox>();

        await cut.Find("button").ClickAsync();
        await checkbox.Instance.HandleFormReset();

        IElement button = cut.Find("button");
        await Assert.That(button.GetAttribute("aria-checked")).IsEqualTo("mixed");
    }

    private static RenderFragment CreateCheckbox(BradixCheckboxCheckedState? checkedState = null, BradixCheckboxCheckedState? defaultChecked = null, bool forceMountIndicator = false, string? name = null, string? form = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixCheckbox>(0);
            builder.AddAttribute(1, nameof(BradixCheckbox.DefaultChecked), (object) (defaultChecked ?? BradixCheckboxCheckedState.Unchecked));

            if (checkedState is not null)
                builder.AddAttribute(2, nameof(BradixCheckbox.Checked), (object) checkedState);

            if (name is not null)
                builder.AddAttribute(3, nameof(BradixCheckbox.Name), name);

            if (form is not null)
                builder.AddAttribute(4, nameof(BradixCheckbox.Form), form);

            builder.AddAttribute(5, nameof(BradixCheckbox.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixCheckboxIndicator>(0);
                contentBuilder.AddAttribute(1, nameof(BradixCheckboxIndicator.ForceMount), forceMountIndicator);
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }
}