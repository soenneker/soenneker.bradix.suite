using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Soenneker.Bradix.Suite.Tests;

internal sealed class TargetedMessageHost : ComponentBase
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
