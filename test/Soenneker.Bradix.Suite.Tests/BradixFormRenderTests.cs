using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixFormRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixFormRenderTests()
    {
        _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        _module.SetupVoid("registerFormRoot", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterFormRoot", _ => true).SetVoidResult();
        _module.SetupVoid("registerLabelTextSelectionGuard", _ => true).SetVoidResult();
        _module.SetupVoid("unregisterLabelTextSelectionGuard", _ => true).SetVoidResult();
        _module.SetupVoid("setFormControlCustomValidity", _ => true).SetVoidResult();
        _module.SetupVoid("clearFormCustomValidity", _ => true).SetVoidResult();
        _module.Setup<bool>("focusServerInvalidFormControl", _ => true).SetResult(true);
        _module.Setup<BradixFormValiditySnapshot>("getFormControlValidity", _ => true)
            .SetResult(new BradixFormValiditySnapshot());
        _module.Setup<BradixFormControlSnapshot>("getFormControlState", _ => true)
            .SetResult(new BradixFormControlSnapshot());

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Fact]
    public void Field_label_and_control_share_generated_relationship_attributes()
    {
        var cut = RenderForm();

        var label = cut.Find("label");
        var input = cut.Find("input");

        Assert.Equal("email", input.GetAttribute("name"));
        Assert.Equal(input.Id, label.GetAttribute("for"));
    }

    [Fact]
    public async Task Invalid_control_registers_message_id_and_invalid_data_attributes()
    {
        var cut = RenderForm();
        var control = cut.FindComponent<BradixFormControl>();

        await control.Instance.HandleValidityChanged(new BradixFormValiditySnapshot
        {
            Valid = false,
            ValueMissing = true
        });

        cut.WaitForAssertion(() =>
        {
            var field = cut.Find("div[data-invalid]");
            var label = cut.Find("label[data-invalid]");
            var input = cut.Find("input");
            var message = cut.Find("span[id]");

            Assert.NotNull(field);
            Assert.NotNull(label);
            Assert.Equal(message.Id, input.GetAttribute("aria-describedby"));
            Assert.Equal("This value is missing", message.TextContent.Trim());
        });
    }

    [Fact]
    public async Task Built_in_message_uses_default_copy_for_matching_validity_state()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixForm>(0);
            builder.AddAttribute(1, nameof(BradixForm.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixFormField>(0);
                contentBuilder.AddAttribute(1, nameof(BradixFormField.Name), "website");
                contentBuilder.AddAttribute(2, nameof(BradixFormField.ChildContent), (RenderFragment)(fieldBuilder =>
                {
                    fieldBuilder.OpenComponent<BradixFormControl>(0);
                    fieldBuilder.AddAttribute(1, nameof(BradixFormControl.Type), "url");
                    fieldBuilder.CloseComponent();

                    fieldBuilder.OpenComponent<BradixFormMessage>(2);
                    fieldBuilder.AddAttribute(3, nameof(BradixFormMessage.Match), "typeMismatch");
                    fieldBuilder.CloseComponent();
                }));
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        var control = cut.FindComponent<BradixFormControl>();
        await control.Instance.HandleValidityChanged(new BradixFormValiditySnapshot
        {
            Valid = false,
            TypeMismatch = true
        });

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("This value does not match the required type", cut.Find("span[id]").TextContent.Trim());
        });
    }

    [Fact]
    public void Submit_renders_submit_button_type()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixFormSubmit>(0);
            builder.AddAttribute(1, nameof(BradixFormSubmit.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.AddContent(0, "Send");
            }));
            builder.CloseComponent();
        });

        var button = cut.Find("button");
        Assert.Equal("submit", button.GetAttribute("type"));
        Assert.Equal("Send", button.TextContent);
    }

    [Fact]
    public async Task Custom_sync_matcher_uses_form_data_and_registers_message_description()
    {
        var cut = RenderCustomMessageForm(
            (Func<string?, BradixFormDataSnapshot, bool>)((value, formData) =>
                !string.Equals(value, formData.Get("password"), StringComparison.Ordinal)),
            "Passwords must match.");

        var control = cut.FindComponent<BradixFormControl>();

        await control.Instance.HandleControlStateChanged(CreateControlSnapshot("mismatch", new BradixFormValiditySnapshot(), new Dictionary<string, string[]>
        {
            ["password"] = ["secret"],
            ["confirmPassword"] = ["mismatch"]
        }));

        cut.WaitForAssertion(() =>
        {
            var input = cut.Find("input");
            var message = cut.Find("span[id]");

            Assert.Equal("Passwords must match.", message.TextContent.Trim());
            Assert.Equal(message.Id, input.GetAttribute("aria-describedby"));
        });
    }

    [Fact]
    public async Task Custom_async_matcher_displays_default_message_when_matcher_fails()
    {
        var cut = RenderCustomMessageForm((Func<string?, BradixFormDataSnapshot, Task<bool>>)(async (value, _) =>
        {
            await Task.Yield();
            return string.Equals(value, "taken", StringComparison.Ordinal);
        }));

        var control = cut.FindComponent<BradixFormControl>();

        await control.Instance.HandleControlStateChanged(CreateControlSnapshot("taken", new BradixFormValiditySnapshot(), new Dictionary<string, string[]>
        {
            ["confirmPassword"] = ["taken"]
        }));

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("This value is not valid", cut.Find("span[id]").TextContent.Trim());
        });
    }

    [Fact]
    public void Reset_clears_custom_validity_through_root_interop()
    {
        var cut = RenderForm();

        cut.Find("form").TriggerEvent("onreset", EventArgs.Empty);

        cut.WaitForAssertion(() =>
        {
            Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "clearFormCustomValidity");
        });
    }

    [Fact]
    public void Server_invalid_control_requests_focus_bridge_on_render()
    {
        Render<BradixForm>(parameters => parameters
            .Add(form => form.ChildContent, (RenderFragment)(contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixFormField>(0);
                contentBuilder.AddAttribute(1, nameof(BradixFormField.Name), "username");
                contentBuilder.AddAttribute(2, nameof(BradixFormField.ServerInvalid), true);
                contentBuilder.AddAttribute(3, nameof(BradixFormField.ChildContent), (RenderFragment)(fieldBuilder =>
                {
                    fieldBuilder.OpenComponent<BradixFormControl>(0);
                    fieldBuilder.CloseComponent();
                }));
                contentBuilder.CloseComponent();
            })));

        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "focusServerInvalidFormControl");
    }

    [Fact]
    public async Task Input_event_clears_custom_validity_before_commit()
    {
        var cut = RenderForm();
        var control = cut.Find("input");
        var controlComponent = cut.FindComponent<BradixFormControl>();

        await controlComponent.Instance.HandleValidityChanged(new BradixFormValiditySnapshot
        {
            Valid = false,
            ValueMissing = true
        });

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll("span[id]")));

        control.Input("hello@example.com");

        cut.WaitForAssertion(() =>
        {
            Assert.Empty(cut.FindAll("span[id]"));
        });

        Assert.Contains(_module.Invocations, invocation => invocation.Identifier == "setFormControlCustomValidity");
    }

    [Fact]
    public void Force_matched_message_retargets_aria_describedby_when_name_changes()
    {
        var cut = Render<TargetedMessageHost>();
        var inputs = cut.FindAll("input");
        var message = cut.Find("span[id]");

        Assert.Equal(message.Id, inputs[0].GetAttribute("aria-describedby"));
        Assert.Null(inputs[1].GetAttribute("aria-describedby"));

        cut.Find("button").Click();

        cut.WaitForAssertion(() =>
        {
            var updatedInputs = cut.FindAll("input");
            var updatedMessage = cut.Find("span[id]");

            Assert.Null(updatedInputs[0].GetAttribute("aria-describedby"));
            Assert.Equal(updatedMessage.Id, updatedInputs[1].GetAttribute("aria-describedby"));
        });
    }

    private IRenderedComponent<BradixForm> RenderForm()
    {
        return Render<BradixForm>(parameters => parameters
            .Add(form => form.ChildContent, (RenderFragment)(contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixFormField>(0);
                contentBuilder.AddAttribute(1, nameof(BradixFormField.Name), "email");
                contentBuilder.AddAttribute(2, nameof(BradixFormField.ChildContent), (RenderFragment)(fieldBuilder =>
                {
                    fieldBuilder.OpenComponent<BradixFormLabel>(0);
                    fieldBuilder.AddAttribute(1, nameof(BradixFormLabel.ChildContent), (RenderFragment)(labelBuilder =>
                    {
                        labelBuilder.AddContent(0, "Email");
                    }));
                    fieldBuilder.CloseComponent();

                    fieldBuilder.OpenComponent<BradixFormControl>(2);
                    fieldBuilder.AddAttribute(3, nameof(BradixFormControl.Type), "email");
                    fieldBuilder.CloseComponent();

                    fieldBuilder.OpenComponent<BradixFormMessage>(4);
                    fieldBuilder.AddAttribute(5, nameof(BradixFormMessage.Match), "valueMissing");
                    fieldBuilder.CloseComponent();
                }));
                contentBuilder.CloseComponent();
            })));
    }

    private IRenderedComponent<BradixForm> RenderCustomMessageForm(object matcher, string? messageContent = null)
    {
        return Render<BradixForm>(parameters => parameters
            .Add(form => form.ChildContent, (RenderFragment)(contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixFormField>(0);
                contentBuilder.AddAttribute(1, nameof(BradixFormField.Name), "confirmPassword");
                contentBuilder.AddAttribute(2, nameof(BradixFormField.ChildContent), (RenderFragment)(fieldBuilder =>
                {
                    fieldBuilder.OpenComponent<BradixFormControl>(0);
                    fieldBuilder.AddAttribute(1, nameof(BradixFormControl.Type), "password");
                    fieldBuilder.CloseComponent();

                    fieldBuilder.OpenComponent<BradixFormMessage>(2);
                    fieldBuilder.AddAttribute(3, nameof(BradixFormMessage.Match), matcher);
                    if (messageContent is not null)
                    {
                        fieldBuilder.AddAttribute(4, nameof(BradixFormMessage.ChildContent), (RenderFragment)(messageBuilder =>
                        {
                            messageBuilder.AddContent(0, messageContent);
                        }));
                    }

                    fieldBuilder.CloseComponent();
                }));
                contentBuilder.CloseComponent();
            })));
    }

    private static BradixFormControlSnapshot CreateControlSnapshot(string value, BradixFormValiditySnapshot validity, Dictionary<string, string[]> formValues)
    {
        return new BradixFormControlSnapshot
        {
            Value = value,
            Validity = validity,
            FormData = new BradixFormDataSnapshot
            {
                Values = formValues
            }
        };
    }

    private sealed class TargetedMessageHost : ComponentBase
    {
        private string _targetName = "email";

        private void ToggleTarget()
        {
            _targetName = _targetName == "email" ? "username" : "email";
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "button");
            builder.AddAttribute(1, "type", "button");
            builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, ToggleTarget));
            builder.AddContent(3, "Retarget");
            builder.CloseElement();

            builder.OpenComponent<BradixForm>(4);
            builder.AddAttribute(5, nameof(BradixForm.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixFormField>(0);
                contentBuilder.AddAttribute(1, nameof(BradixFormField.Name), "email");
                contentBuilder.AddAttribute(2, nameof(BradixFormField.ChildContent), (RenderFragment)(fieldBuilder =>
                {
                    fieldBuilder.OpenComponent<BradixFormControl>(0);
                    fieldBuilder.CloseComponent();
                }));
                contentBuilder.CloseComponent();

                contentBuilder.OpenComponent<BradixFormField>(10);
                contentBuilder.AddAttribute(11, nameof(BradixFormField.Name), "username");
                contentBuilder.AddAttribute(12, nameof(BradixFormField.ChildContent), (RenderFragment)(fieldBuilder =>
                {
                    fieldBuilder.OpenComponent<BradixFormControl>(0);
                    fieldBuilder.CloseComponent();
                }));
                contentBuilder.CloseComponent();

                contentBuilder.OpenComponent<BradixFormMessage>(20);
                contentBuilder.AddAttribute(21, nameof(BradixFormMessage.Name), _targetName);
                contentBuilder.AddAttribute(22, nameof(BradixFormMessage.ForceMatch), true);
                contentBuilder.AddAttribute(23, nameof(BradixFormMessage.ChildContent), (RenderFragment)(messageBuilder =>
                {
                    messageBuilder.AddContent(0, "Targeted message");
                }));
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        }
    }
}
