using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixOneTimePasswordFieldRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixOneTimePasswordFieldRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerOneTimePasswordInput", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterOneTimePasswordInput", _ => true).SetVoidResult();
        _module.SetupVoid("requestFormSubmit", _ => true).SetVoidResult();

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
    }

    [Fact]
    public void Sequential_input_updates_hidden_value()
    {
        var cut = RenderOtpField();

        cut.Find("input[data-index='0']").Input("1");
        cut.Find("input[data-index='1']").Input("2");

        Assert.Equal("12", cut.Find("input[type='hidden']").GetAttribute("value"));
    }

    [Fact]
    public async Task Paste_distributes_value_across_inputs()
    {
        var cut = RenderOtpField();
        var input = cut.FindComponents<BradixOneTimePasswordFieldInput>().First();

        await cut.InvokeAsync(() => input.Instance.HandlePasteAsync("1234"));

        Assert.Equal("1234", cut.Find("input[type='hidden']").GetAttribute("value"));
        Assert.Equal("4", cut.Find("input[data-index='3']").GetAttribute("value"));
    }

    [Fact]
    public async Task Backspace_shifts_remaining_characters_left()
    {
        var cut = RenderOtpField();
        var input = cut.FindComponents<BradixOneTimePasswordFieldInput>().First();
        await cut.InvokeAsync(() => input.Instance.HandlePasteAsync("1234"));

        cut.Find("input[data-index='1']").KeyDown("Backspace");

        Assert.Equal("134", cut.Find("input[type='hidden']").GetAttribute("value"));
        Assert.Equal("3", cut.Find("input[data-index='1']").GetAttribute("value"));
        Assert.Equal("4", cut.Find("input[data-index='2']").GetAttribute("value"));
    }

    [Fact]
    public void Invalid_numeric_character_is_rejected()
    {
        string? invalid = null;
        var cut = RenderOtpField(onInvalidChange: value => invalid = value);

        cut.Find("input[data-index='0']").Input("A");

        Assert.Equal("A", invalid);
        Assert.Equal(string.Empty, cut.Find("input[type='hidden']").GetAttribute("value"));
    }

    [Fact]
    public async Task Completion_triggers_auto_submit_callback_and_form_request()
    {
        string submitted = string.Empty;
        var cut = RenderOtpField(autoSubmit: true, onAutoSubmit: value => submitted = value);
        var input = cut.FindComponents<BradixOneTimePasswordFieldInput>().First();

        await cut.InvokeAsync(() => input.Instance.HandlePasteAsync("1234"));

        Assert.Equal("1234", submitted);
        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "requestFormSubmit");
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
