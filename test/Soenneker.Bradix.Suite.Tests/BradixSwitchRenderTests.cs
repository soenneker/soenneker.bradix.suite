using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixSwitchRenderTests : BunitContext
{
    public BradixSwitchRenderTests()
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
    public void Switch_with_name_renders_hidden_input()
    {
        var cut = Render(CreateSwitch(defaultChecked: true, name: "notifications"));

        var input = cut.Find("input[type='checkbox']");

        Assert.Equal("notifications", input.GetAttribute("name"));
        Assert.True(input.HasAttribute("checked"));
    }

    [Fact]
    public async Task Uncontrolled_switch_resets_to_default_state()
    {
        var cut = Render(CreateSwitch(defaultChecked: true));
        var component = cut.FindComponent<BradixSwitch>();

        cut.Find("button").Click();
        await component.Instance.HandleFormResetAsync();

        var button = cut.Find("button");
        Assert.Equal("true", button.GetAttribute("aria-checked"));
    }

    private static RenderFragment CreateSwitch(bool? checkedState = null, bool defaultChecked = false, string? name = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixSwitch>(0);
            builder.AddAttribute(1, nameof(BradixSwitch.DefaultChecked), defaultChecked);

            if (checkedState.HasValue)
                builder.AddAttribute(2, nameof(BradixSwitch.Checked), checkedState.Value);

            if (name is not null)
                builder.AddAttribute(3, nameof(BradixSwitch.Name), name);

            builder.AddAttribute(4, nameof(BradixSwitch.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixSwitchThumb>(0);
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }
}
