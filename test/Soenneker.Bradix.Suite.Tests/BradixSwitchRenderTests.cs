using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixSwitchRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixSwitchRenderTests()
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
    public void Switch_renders_switch_role_and_unchecked_state()
    {
        var cut = Render(CreateSwitch());

        var button = cut.Find("button");

        Assert.Equal("switch", button.GetAttribute("role"));
        Assert.Equal("false", button.GetAttribute("aria-checked"));
        Assert.Equal("unchecked", button.GetAttribute("data-state"));
    }

    [Fact]
    public void Switch_click_toggles_uncontrolled_state()
    {
        var cut = Render(CreateSwitch());

        cut.Find("button").Click();
        var button = cut.Find("button");

        Assert.Equal("true", button.GetAttribute("aria-checked"));
        Assert.Equal("checked", button.GetAttribute("data-state"));
    }

    [Fact]
    public void Controlled_switch_respects_checked_parameter()
    {
        var cut = Render(CreateSwitch(checkedState: true));

        var button = cut.Find("button");
        var thumb = cut.Find("span");

        Assert.Equal("true", button.GetAttribute("aria-checked"));
        Assert.Equal("checked", thumb.GetAttribute("data-state"));
    }

    [Fact]
    public void Switch_with_name_outside_form_does_not_render_hidden_input()
    {
        var cut = Render(CreateSwitch(defaultChecked: true, name: "notifications"));
        Assert.Empty(cut.FindAll("input[type='checkbox']"));
    }

    [Fact]
    public void Switch_with_explicit_form_renders_hidden_input_outside_form()
    {
        var cut = Render(CreateSwitch(defaultChecked: true, name: "notifications", form: "settings-form"));

        var input = cut.Find("input[type='checkbox']");
        Assert.Equal("settings-form", input.GetAttribute("form"));
        Assert.Contains(_module.Invocations, invocation =>
            invocation.Identifier == "registerCheckboxRoot" &&
            invocation.Arguments.Count > 2 &&
            Equals(invocation.Arguments[2], "settings-form"));
    }

    [Fact]
    public async Task Uncontrolled_switch_resets_to_default_state()
    {
        var cut = Render(CreateSwitch(defaultChecked: true));
        var component = cut.FindComponent<BradixSwitch>();

        cut.Find("button").Click();
        await component.Instance.HandleFormReset();

        var button = cut.Find("button");
        Assert.Equal("true", button.GetAttribute("aria-checked"));
    }

    private static RenderFragment CreateSwitch(bool? checkedState = null, bool defaultChecked = false, string? name = null, string? form = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixSwitch>(0);
            builder.AddAttribute(1, nameof(BradixSwitch.DefaultChecked), defaultChecked);

            if (checkedState.HasValue)
                builder.AddAttribute(2, nameof(BradixSwitch.Checked), checkedState.Value);

            if (name is not null)
                builder.AddAttribute(3, nameof(BradixSwitch.Name), name);

            if (form is not null)
                builder.AddAttribute(4, nameof(BradixSwitch.Form), form);

            builder.AddAttribute(5, nameof(BradixSwitch.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixSwitchThumb>(0);
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }
}
