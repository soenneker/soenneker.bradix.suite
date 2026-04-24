using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

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

    [Test]
    public async Task Switch_renders_switch_role_and_unchecked_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSwitch());

        IElement button = cut.Find("button");

        await Assert.That(button.GetAttribute("role")).IsEqualTo("switch");
        await Assert.That(button.GetAttribute("aria-checked")).IsEqualTo("false");
        await Assert.That(button.GetAttribute("data-state")).IsEqualTo("unchecked");
    }

    [Test]
    public async Task Switch_click_toggles_uncontrolled_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSwitch());

        await cut.Find("button").ClickAsync();
        IElement button = cut.Find("button");

        await Assert.That(button.GetAttribute("aria-checked")).IsEqualTo("true");
        await Assert.That(button.GetAttribute("data-state")).IsEqualTo("checked");
    }

    [Test]
    public async Task Controlled_switch_respects_checked_parameter()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSwitch(checkedState: true));

        IElement button = cut.Find("button");
        IElement thumb = cut.Find("span");

        await Assert.That(button.GetAttribute("aria-checked")).IsEqualTo("true");
        await Assert.That(thumb.GetAttribute("data-state")).IsEqualTo("checked");
    }

    [Test]
    public async Task Switch_with_name_outside_form_does_not_render_hidden_input()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSwitch(defaultChecked: true, name: "notifications"));
        await Assert.That(cut.FindAll("input[type='checkbox']")).IsEmpty();
    }

    [Test]
    public async Task Switch_with_explicit_form_renders_hidden_input_outside_form()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSwitch(defaultChecked: true, name: "notifications", form: "settings-form"));

        IElement input = cut.Find("input[type='checkbox']");
        await Assert.That(input.GetAttribute("form")).IsEqualTo("settings-form");
        await Assert.That(_module.Invocations.Any(invocation =>
            invocation.Identifier == "registerCheckboxRoot" &&
            invocation.Arguments.Count > 2 &&
            Equals(invocation.Arguments[2], "settings-form"))).IsTrue();
    }

    [Test]
    public async Task Switch_registers_enter_key_activation()
    {
        _ = Render(CreateSwitch());

        object? options = _module.Invocations.First(invocation => invocation.Identifier == "registerDelegatedInteraction").Arguments[2];
        object? keydown = options?.GetType().GetProperty("keydown")?.GetValue(options);
        object? keys = keydown?.GetType().GetProperty("keys")?.GetValue(keydown);
        object? method = keydown?.GetType().GetProperty("method")?.GetValue(keydown);
        object? preventDefault = keydown?.GetType().GetProperty("preventDefault")?.GetValue(keydown);

        await Assert.That(keys).IsAssignableTo<string[]>();
        await Assert.That(((string[])keys!).Contains("Enter")).IsTrue();
        await Assert.That(method).IsEqualTo(nameof(BradixSwitch.HandleDelegatedEnterKeyDown));
        await Assert.That(preventDefault is true).IsTrue();
    }

    [Test]
    public async Task Uncontrolled_switch_resets_to_default_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSwitch(defaultChecked: true));
        IRenderedComponent<BradixSwitch> component = cut.FindComponent<BradixSwitch>();

        await cut.Find("button").ClickAsync();
        await component.Instance.HandleFormReset();

        IElement button = cut.Find("button");
        await Assert.That(button.GetAttribute("aria-checked")).IsEqualTo("true");
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
