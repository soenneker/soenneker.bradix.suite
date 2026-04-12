using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

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

    [Fact]
    public void Checkbox_renders_unchecked_state_by_default()
    {
        var cut = Render(CreateCheckbox());

        var button = cut.Find("button");

        Assert.Equal("false", button.GetAttribute("aria-checked"));
        Assert.Equal("unchecked", button.GetAttribute("data-state"));
    }

    [Fact]
    public void Checkbox_click_cycles_indeterminate_to_checked()
    {
        var cut = Render(CreateCheckbox(defaultChecked: BradixCheckboxCheckedState.Indeterminate));

        var button = cut.Find("button");
        button.Click();
        button = cut.Find("button");

        Assert.Equal("true", button.GetAttribute("aria-checked"));
        Assert.Equal("checked", button.GetAttribute("data-state"));
    }

    [Fact]
    public void Checkbox_can_render_mixed_state_when_controlled()
    {
        var cut = Render(CreateCheckbox(checkedState: BradixCheckboxCheckedState.Indeterminate));

        var button = cut.Find("button");

        Assert.Equal("mixed", button.GetAttribute("aria-checked"));
        Assert.Equal("indeterminate", button.GetAttribute("data-state"));
    }

    [Fact]
    public void Checkbox_indicator_force_mount_renders_when_unchecked()
    {
        var cut = Render(CreateCheckbox(forceMountIndicator: true));

        var indicator = cut.Find("span");
        Assert.Contains("pointer-events: none", indicator.GetAttribute("style"));
    }

    [Fact]
    public void Checkbox_with_name_outside_form_does_not_render_hidden_input()
    {
        var cut = Render(CreateCheckbox(defaultChecked: BradixCheckboxCheckedState.Checked, name: "terms"));
        Assert.Empty(cut.FindAll("input[type='checkbox']"));
    }

    [Fact]
    public void Checkbox_with_explicit_form_renders_hidden_input_outside_form()
    {
        var cut = Render(CreateCheckbox(defaultChecked: BradixCheckboxCheckedState.Checked, name: "terms", form: "settings-form"));

        var input = cut.Find("input[type='checkbox']");
        Assert.Equal("settings-form", input.GetAttribute("form"));
        Assert.Contains(_module.Invocations, invocation =>
            invocation.Identifier == "registerCheckboxRoot" &&
            invocation.Arguments.Count > 2 &&
            Equals(invocation.Arguments[2], "settings-form"));
    }

    [Fact]
    public async Task Uncontrolled_checkbox_resets_to_initial_state()
    {
        var cut = Render(CreateCheckbox(defaultChecked: BradixCheckboxCheckedState.Indeterminate));
        var checkbox = cut.FindComponent<BradixCheckbox>();

        cut.Find("button").Click();
        await checkbox.Instance.HandleFormReset();

        var button = cut.Find("button");
        Assert.Equal("mixed", button.GetAttribute("aria-checked"));
    }

    private static RenderFragment CreateCheckbox(BradixCheckboxCheckedState? checkedState = null, BradixCheckboxCheckedState defaultChecked = BradixCheckboxCheckedState.Unchecked, bool forceMountIndicator = false, string? name = null, string? form = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixCheckbox>(0);
            builder.AddAttribute(1, nameof(BradixCheckbox.DefaultChecked), defaultChecked);

            if (checkedState.HasValue)
                builder.AddAttribute(2, nameof(BradixCheckbox.Checked), checkedState.Value);

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
