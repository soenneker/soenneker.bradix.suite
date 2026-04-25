using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

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

    [Test]
    public async Task Field_label_and_control_share_generated_relationship_attributes()
    {
        IRenderedComponent<BradixForm> cut = RenderForm();

        IElement label = cut.Find("label");
        IElement input = cut.Find("input");

        await Assert.That(input.GetAttribute("name")).IsEqualTo("email");
        await Assert.That(label.GetAttribute("for")).IsEqualTo(input.Id);
    }

    [Test]
    public async Task Form_label_forwards_id_and_mouse_down_to_label_primitive()
    {
        var mouseDownCount = 0;

        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixFormLabel>(0);
            builder.AddAttribute(1, nameof(BradixFormLabel.Id), "email-label");
            builder.AddAttribute(2, nameof(BradixFormLabel.For), "email-input");
            builder.AddAttribute(3, nameof(BradixFormLabel.OnMouseDown), EventCallback.Factory.Create<MouseEventArgs>(this, () => mouseDownCount++));
            builder.AddAttribute(4, nameof(BradixFormLabel.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.AddContent(0, "Email");
            }));
            builder.CloseComponent();
        });

        IElement label = cut.Find("label");
        await Assert.That(label.Id).IsEqualTo("email-label");
        await Assert.That(label.GetAttribute("for")).IsEqualTo("email-input");

        IRenderedComponent<BradixLabel> primitive = cut.FindComponent<BradixLabel>();
        await primitive.Instance.HandleMouseDownFromJs(new BradixDelegatedMouseEvent { Detail = 1 });

        await Assert.That(mouseDownCount).IsEqualTo(1);
    }

    [Test]
    public async Task Invalid_control_registers_message_id_and_invalid_data_attributes()
    {
        IRenderedComponent<BradixForm> cut = RenderForm();
        IRenderedComponent<BradixFormControl> control = cut.FindComponent<BradixFormControl>();

        await control.Instance.HandleValidityChanged(new BradixFormValiditySnapshot
        {
            Valid = false,
            ValueMissing = true
        });

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement field = cut.Find("div[data-invalid]");
            IElement label = cut.Find("label[data-invalid]");
            IElement input = cut.Find("input");
            IElement message = cut.Find("span[id]");

            await Assert.That(field).IsNotNull();
            await Assert.That(label).IsNotNull();
            await Assert.That(input.GetAttribute("aria-describedby")).IsEqualTo(message.Id);
            await Assert.That(message.TextContent.Trim()).IsEqualTo("This value is missing");
        });
    }

    [Test]
    public async Task Built_in_message_uses_default_copy_for_matching_validity_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
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

        IRenderedComponent<BradixFormControl> control = cut.FindComponent<BradixFormControl>();
        await control.Instance.HandleValidityChanged(new BradixFormValiditySnapshot
        {
            Valid = false,
            TypeMismatch = true
        });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("span[id]").TextContent.Trim()).IsEqualTo("This value does not match the required type");
        });
    }

    [Test]
    public async Task Built_in_valid_message_matches_valid_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixForm>(0);
            builder.AddAttribute(1, nameof(BradixForm.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixFormField>(0);
                contentBuilder.AddAttribute(1, nameof(BradixFormField.Name), "email");
                contentBuilder.AddAttribute(2, nameof(BradixFormField.ChildContent), (RenderFragment)(fieldBuilder =>
                {
                    fieldBuilder.OpenComponent<BradixFormControl>(0);
                    fieldBuilder.CloseComponent();

                    fieldBuilder.OpenComponent<BradixFormMessage>(2);
                    fieldBuilder.AddAttribute(3, nameof(BradixFormMessage.Match), "valid");
                    fieldBuilder.AddAttribute(4, nameof(BradixFormMessage.ChildContent), (RenderFragment)(messageBuilder =>
                    {
                        messageBuilder.AddContent(0, "Looks good");
                    }));
                    fieldBuilder.CloseComponent();
                }));
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        IRenderedComponent<BradixFormControl> control = cut.FindComponent<BradixFormControl>();
        await control.Instance.HandleValidityChanged(new BradixFormValiditySnapshot
        {
            Valid = true
        });

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("span[id]").TextContent.Trim()).IsEqualTo("Looks good");
        });
    }

    [Test]
    public async Task Message_without_match_renders_and_registers_description_immediately()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixForm>(0);
            builder.AddAttribute(1, nameof(BradixForm.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixFormField>(0);
                contentBuilder.AddAttribute(1, nameof(BradixFormField.Name), "email");
                contentBuilder.AddAttribute(2, nameof(BradixFormField.ChildContent), (RenderFragment)(fieldBuilder =>
                {
                    fieldBuilder.OpenComponent<BradixFormControl>(0);
                    fieldBuilder.CloseComponent();

                    fieldBuilder.OpenComponent<BradixFormMessage>(2);
                    fieldBuilder.AddAttribute(3, nameof(BradixFormMessage.ChildContent), (RenderFragment)(messageBuilder =>
                    {
                        messageBuilder.AddContent(0, "Generic message");
                    }));
                    fieldBuilder.CloseComponent();
                }));
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement input = cut.Find("input");
            IElement message = cut.Find("span[id]");

            await Assert.That(message.TextContent.Trim()).IsEqualTo("Generic message");
            await Assert.That(input.GetAttribute("aria-describedby")).IsEqualTo(message.Id);
        });
    }

    [Test]
    public async Task Submit_renders_submit_button_type()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixFormSubmit>(0);
            builder.AddAttribute(1, nameof(BradixFormSubmit.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.AddContent(0, "Send");
            }));
            builder.CloseComponent();
        });

        IElement button = cut.Find("button");
        await Assert.That(button.GetAttribute("type")).IsEqualTo("submit");
        await Assert.That(button.TextContent).IsEqualTo("Send");
    }

    [Test]
    public async Task Custom_sync_matcher_uses_form_data_and_registers_message_description()
    {
        IRenderedComponent<BradixForm> cut = RenderCustomMessageForm(
            (Func<string?, BradixFormDataSnapshot, bool>)((value, formData) =>
                !string.Equals(value, formData.Get("password"), StringComparison.Ordinal)),
            "Passwords must match.");

        IRenderedComponent<BradixFormControl> control = cut.FindComponent<BradixFormControl>();

        await control.Instance.HandleControlStateChanged(CreateControlSnapshot("mismatch", new BradixFormValiditySnapshot(), new Dictionary<string, string[]>
        {
            ["password"] = ["secret"],
            ["confirmPassword"] = ["mismatch"]
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            IElement input = cut.Find("input");
            IElement message = cut.Find("span[id]");

            await Assert.That(message.TextContent.Trim()).IsEqualTo("Passwords must match.");
            await Assert.That(input.GetAttribute("aria-describedby")).IsEqualTo(message.Id);
        });
    }

    [Test]
    public async Task Custom_async_matcher_displays_default_message_when_matcher_fails()
    {
        IRenderedComponent<BradixForm> cut = RenderCustomMessageForm((Func<string?, BradixFormDataSnapshot, Task<bool>>)(async (value, _) =>
        {
            await Task.Yield();
            return string.Equals(value, "taken", StringComparison.Ordinal);
        }));

        IRenderedComponent<BradixFormControl> control = cut.FindComponent<BradixFormControl>();

        await control.Instance.HandleControlStateChanged(CreateControlSnapshot("taken", new BradixFormValiditySnapshot(), new Dictionary<string, string[]>
        {
            ["confirmPassword"] = ["taken"]
        }));

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Find("span[id]").TextContent.Trim()).IsEqualTo("This value is not valid");
        });
    }

    [Test]
    public async Task Reset_clears_custom_validity_through_root_interop()
    {
        IRenderedComponent<BradixForm> cut = RenderForm();

        await cut.Find("form").TriggerEventAsync("onreset", EventArgs.Empty);

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "clearFormCustomValidity")).IsTrue();
        });
    }

    [Test]
    public async Task Submit_and_reset_compose_public_callbacks_with_server_error_clearing()
    {
        List<string> events = [];

        IRenderedComponent<BradixForm> cut = Render<BradixForm>(parameters => parameters
            .Add(form => form.OnSubmit, EventCallback.Factory.Create<EventArgs>(this, () => events.Add("submit")))
            .Add(form => form.OnReset, EventCallback.Factory.Create<EventArgs>(this, () => events.Add("reset")))
            .Add(form => form.OnClearServerErrors, EventCallback.Factory.Create(this, () => events.Add("clear")))
            .Add(form => form.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<BradixFormSubmit>(0);
                builder.AddAttribute(1, nameof(BradixFormSubmit.ChildContent), (RenderFragment)(contentBuilder =>
                {
                    contentBuilder.AddContent(0, "Send");
                }));
                builder.CloseComponent();
            })));

        await cut.Find("form").TriggerEventAsync("onsubmit", EventArgs.Empty);
        await cut.Find("form").TriggerEventAsync("onreset", EventArgs.Empty);

        await Assert.That(events).IsEquivalentTo(["submit", "clear", "reset", "clear"]);
    }

    [Test]
    public async Task Invalid_controls_are_reported_through_public_callback_once_per_field()
    {
        IReadOnlyList<string>? invalidFields = null;

        IRenderedComponent<BradixForm> cut = Render<BradixForm>(parameters => parameters
            .Add(form => form.OnInvalid, EventCallback.Factory.Create<IReadOnlyList<string>>(this, fields => invalidFields = fields))
            .Add(form => form.ChildContent, (RenderFragment)(contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixFormField>(0);
                contentBuilder.AddAttribute(1, nameof(BradixFormField.Name), "email");
                contentBuilder.AddAttribute(2, nameof(BradixFormField.ChildContent), (RenderFragment)(fieldBuilder =>
                {
                    fieldBuilder.OpenComponent<BradixFormControl>(0);
                    fieldBuilder.CloseComponent();
                }));
                contentBuilder.CloseComponent();
            })));

        await cut.Instance.HandleInvalidControls(["email", "email"]);

        await Assert.That(invalidFields).IsNotNull();
        await Assert.That(invalidFields!).IsEquivalentTo(["email"]);
    }

    [Test]
    public async Task Server_invalid_control_requests_focus_bridge_on_render()
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

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "focusServerInvalidFormControl")).IsTrue();
    }

    [Test]
    public async Task Input_event_clears_custom_validity_before_commit()
    {
        IRenderedComponent<BradixForm> cut = RenderForm();
        IElement control = cut.Find("input");
        IRenderedComponent<BradixFormControl> controlComponent = cut.FindComponent<BradixFormControl>();

        await controlComponent.Instance.HandleValidityChanged(new BradixFormValiditySnapshot
        {
            Valid = false,
            ValueMissing = true
        });

        await Assert.That(cut.FindAll("span[id]")).HasSingleItem();

        await control.InputAsync("hello@example.com");

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.FindAll("span[id]")).IsEmpty();
        });

        await Assert.That(_module.Invocations.Any(invocation => invocation.Identifier == "setFormControlCustomValidity")).IsTrue();
    }

    [Test]
    public async Task Force_matched_message_retargets_aria_describedby_when_name_changes()
    {
        IRenderedComponent<TargetedMessageHost> cut = Render<TargetedMessageHost>();
        IReadOnlyList<IElement> inputs = cut.FindAll("input");
        IElement message = cut.Find("span[id]");

        await Assert.That(inputs[0].GetAttribute("aria-describedby")).IsEqualTo(message.Id);
        await Assert.That(inputs[1].GetAttribute("aria-describedby")).IsNull();

        await cut.Find("button").ClickAsync();

        await cut.WaitForAssertionAsync(async () =>
        {
            IReadOnlyList<IElement> updatedInputs = cut.FindAll("input");
            IElement updatedMessage = cut.Find("span[id]");

            await Assert.That(updatedInputs[0].GetAttribute("aria-describedby")).IsNull();
            await Assert.That(updatedInputs[1].GetAttribute("aria-describedby")).IsEqualTo(updatedMessage.Id);
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
