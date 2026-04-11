using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixCheckboxRenderTests : BunitContext
{
    public BradixCheckboxRenderTests()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.Setup<bool>("isFormControl", _ => true).SetResult(false);
        module.SetupVoid("registerCheckboxRoot", _ => true);
        module.SetupVoid("unregisterCheckboxRoot", _ => true);
        module.SetupVoid("registerDelegatedInteraction", _ => true);
        module.SetupVoid("unregisterDelegatedInteraction", _ => true);
        module.SetupVoid("syncCheckboxBubbleInputState", _ => true);

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
    public void Checkbox_with_name_renders_hidden_input()
    {
        var cut = Render(CreateCheckbox(defaultChecked: BradixCheckboxCheckedState.Checked, name: "terms"));

        var input = cut.Find("input[type='checkbox']");

        Assert.Equal("terms", input.GetAttribute("name"));
        Assert.True(input.HasAttribute("checked"));
    }

    [Fact]
    public async Task Uncontrolled_checkbox_resets_to_initial_state()
    {
        var cut = Render(CreateCheckbox(defaultChecked: BradixCheckboxCheckedState.Indeterminate));
        var checkbox = cut.FindComponent<BradixCheckbox>();

        cut.Find("button").Click();
        await checkbox.Instance.HandleFormResetAsync();

        var button = cut.Find("button");
        Assert.Equal("mixed", button.GetAttribute("aria-checked"));
    }

    private static RenderFragment CreateCheckbox(BradixCheckboxCheckedState? checkedState = null, BradixCheckboxCheckedState defaultChecked = BradixCheckboxCheckedState.Unchecked, bool forceMountIndicator = false, string? name = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixCheckbox>(0);
            builder.AddAttribute(1, nameof(BradixCheckbox.DefaultChecked), defaultChecked);

            if (checkedState.HasValue)
                builder.AddAttribute(2, nameof(BradixCheckbox.Checked), checkedState.Value);

            if (name is not null)
                builder.AddAttribute(3, nameof(BradixCheckbox.Name), name);

            builder.AddAttribute(4, nameof(BradixCheckbox.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixCheckboxIndicator>(0);
                contentBuilder.AddAttribute(1, nameof(BradixCheckboxIndicator.ForceMount), forceMountIndicator);
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }
}
