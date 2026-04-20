using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixOneTimePasswordFieldRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixOneTimePasswordFieldRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerOneTimePasswordInput", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterOneTimePasswordInput", _ => true).SetVoidResult();
        _module.SetupVoid("registerAssociatedFormReset", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterAssociatedFormReset", _ => true).SetVoidResult();
        _module.SetupVoid("requestFormSubmit", _ => true).SetVoidResult();
        _module.SetupVoid("selectInputText", _ => true).SetVoidResult();
        _module.SetupVoid("syncInputValue", _ => true).SetVoidResult();
        _module.SetupVoid("focusElementById", _ => true).SetVoidResult();

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Sequential_input_updates_hidden_value()
    {
        IRenderedComponent<ContainerFragment> cut = RenderOtpField();

        await cut.Find("input[data-index='0']").InputAsync("1");
        await cut.Find("input[data-index='1']").InputAsync("2");

        await Assert.That(cut.Find("input[type='hidden']").GetAttribute("value")).IsEqualTo("12");
    }

    [Test]
    public async Task Paste_distributes_value_across_inputs()
    {
        IRenderedComponent<ContainerFragment> cut = RenderOtpField();
        IRenderedComponent<BradixOneTimePasswordFieldInput> input = cut.FindComponents<BradixOneTimePasswordFieldInput>().First();

        await cut.InvokeAsync(() => input.Instance.HandlePaste("1234"));

        await Assert.That(cut.Find("input[type='hidden']").GetAttribute("value")).IsEqualTo("1234");
        await Assert.That(cut.Find("input[data-index='3']").GetAttribute("value")).IsEqualTo("4");
    }

    [Test]
    public async Task Backspace_shifts_remaining_characters_left()
    {
        IRenderedComponent<ContainerFragment> cut = RenderOtpField();
        IRenderedComponent<BradixOneTimePasswordFieldInput> input = cut.FindComponents<BradixOneTimePasswordFieldInput>().First();
        await cut.InvokeAsync(() => input.Instance.HandlePaste("1234"));

        await cut.Find("input[data-index='1']").KeyDownAsync("Backspace");

        await Assert.That(cut.Find("input[type='hidden']").GetAttribute("value")).IsEqualTo("134");
        await Assert.That(cut.Find("input[data-index='1']").GetAttribute("value")).IsEqualTo("3");
        await Assert.That(cut.Find("input[data-index='2']").GetAttribute("value")).IsEqualTo("4");
    }

    [Test]
    public async Task Invalid_numeric_character_is_rejected()
    {
        string? invalid = null;
        IRenderedComponent<ContainerFragment> cut = RenderOtpField(onInvalidChange: value => invalid = value);

        await cut.Find("input[data-index='0']").InputAsync("A");

        await Assert.That(invalid).IsEqualTo("A");
        await Assert.That(cut.Find("input[type='hidden']").GetAttribute("value")).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task Completion_triggers_auto_submit_callback_and_form_request()
    {
        string submitted = string.Empty;
        IRenderedComponent<ContainerFragment> cut = RenderOtpField(autoSubmit: true, onAutoSubmit: value => submitted = value);
        IRenderedComponent<BradixOneTimePasswordFieldInput> input = cut.FindComponents<BradixOneTimePasswordFieldInput>().First();

        await cut.InvokeAsync(() => input.Instance.HandlePaste("1234"));

        await Assert.That(submitted).IsEqualTo("1234");
        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "requestFormSubmit")).IsTrue();
    }

    [Test]
    public async Task Form_reset_clears_uncontrolled_value()
    {
        IRenderedComponent<ContainerFragment> cut = RenderOtpField();
        IRenderedComponent<BradixOneTimePasswordFieldInput> input = cut.FindComponents<BradixOneTimePasswordFieldInput>().First();
        IRenderedComponent<BradixOneTimePasswordField> root = cut.FindComponent<BradixOneTimePasswordField>();

        await cut.InvokeAsync(() => input.Instance.HandlePaste("1234"));
        await cut.InvokeAsync(() => root.Instance.HandleFormReset());

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("input[type='hidden']").GetAttribute("value")).IsEqualTo(string.Empty);
            await Assert.That(cut.Find("input[data-index='0']").GetAttribute("value")).IsEqualTo(string.Empty);
        });
    }

    [Test]
    public async Task Inputs_render_radix_accessibility_and_password_manager_attributes()
    {
        IRenderedComponent<ContainerFragment> cut = RenderOtpField();

        IElement first = cut.Find("input[data-index='0']");
        IElement second = cut.Find("input[data-index='1']");
        IElement hidden = cut.Find("input[type='hidden']");

        await Assert.That(first.GetAttribute("aria-label")).IsEqualTo("Character 1 of 4");
        await Assert.That(first.GetAttribute("autocomplete")).IsEqualTo("one-time-code");
        await Assert.That(first.GetAttribute("maxlength")).IsEqualTo("4");
        await Assert.That(first.HasAttribute("data-radix-otp-input")).IsTrue();
        await Assert.That(first.GetAttribute("data-radix-index")).IsEqualTo("0");

        await Assert.That(second.GetAttribute("autocomplete")).IsEqualTo("off");
        await Assert.That(second.GetAttribute("data-1p-ignore")).IsEqualTo("true");
        await Assert.That(second.GetAttribute("data-lpignore")).IsEqualTo("true");
        await Assert.That(second.GetAttribute("data-protonpass-ignore")).IsEqualTo("true");
        await Assert.That(second.GetAttribute("data-bwignore")).IsEqualTo("true");
        await Assert.That(second.GetAttribute("maxlength")).IsEqualTo("1");

        await Assert.That(hidden.GetAttribute("autocomplete")).IsEqualTo("off");
        await Assert.That(hidden.GetAttribute("autocapitalize")).IsEqualTo("off");
        await Assert.That(hidden.GetAttribute("autocorrect")).IsEqualTo("off");
        await Assert.That(hidden.GetAttribute("autosave")).IsEqualTo("off");
        await Assert.That(hidden.GetAttribute("spellcheck")).IsEqualTo("false");
    }

    [Test]
    public async Task Inputs_use_single_roving_tab_stop()
    {
        IRenderedComponent<ContainerFragment> cut = RenderOtpField();
        IRenderedComponent<BradixOneTimePasswordFieldInput> input = cut.FindComponents<BradixOneTimePasswordFieldInput>().First();

        await Assert.That(cut.Find("input[data-index='0']").GetAttribute("tabindex")).IsEqualTo("0");
        await Assert.That(cut.Find("input[data-index='1']").GetAttribute("tabindex")).IsEqualTo("-1");

        await cut.InvokeAsync(() => input.Instance.HandlePaste("12"));

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("input[data-index='0']").GetAttribute("tabindex")).IsEqualTo("-1");
            await Assert.That(cut.Find("input[data-index='1']").GetAttribute("tabindex")).IsEqualTo("0");
            await Assert.That(cut.Find("input[data-index='2']").GetAttribute("tabindex")).IsEqualTo("-1");
        });
    }

    [Test]
    public async Task Standalone_input_does_not_emit_invalid_character_count_label()
    {
        IRenderedComponent<BradixOneTimePasswordFieldInput> cut = Render<BradixOneTimePasswordFieldInput>();

        await Assert.That(cut.Find("input").GetAttribute("aria-label")).IsNull();
    }

    private IRenderedComponent<ContainerFragment> RenderOtpField(Action<string>? onInvalidChange = null, bool autoSubmit = false, Action<string>? onAutoSubmit = null)
    {
        return Render(builder =>
        {
            builder.OpenComponent<BradixOneTimePasswordField>(0);
            builder.AddAttribute(1, nameof(BradixOneTimePasswordField.Name), "otp");
            builder.AddAttribute(2, nameof(BradixOneTimePasswordField.Form), "otp-form");
            builder.AddAttribute(3, nameof(BradixOneTimePasswordField.AutoSubmit), autoSubmit);
            builder.AddAttribute(4, nameof(BradixOneTimePasswordField.OnAutoSubmit), EventCallback.Factory.Create<string>(this, value => onAutoSubmit?.Invoke(value)));
            builder.AddAttribute(5, nameof(BradixOneTimePasswordField.ChildContent), (RenderFragment)(content =>
            {
                for (int i = 0; i < 4; i++)
                {
                    content.OpenComponent<BradixOneTimePasswordFieldInput>(i);
                    content.AddAttribute(i + 10, nameof(BradixOneTimePasswordFieldInput.Index), i);
                    content.AddAttribute(i + 20, nameof(BradixOneTimePasswordFieldInput.OnInvalidChange), EventCallback.Factory.Create<string>(this, value => onInvalidChange?.Invoke(value)));
                    content.CloseComponent();
                }

                content.OpenComponent<BradixOneTimePasswordFieldHiddenInput>(100);
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });
    }
}
