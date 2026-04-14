using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit.Rendering;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixToggleRenderTests : BunitContext
{
    public BradixToggleRenderTests()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerDelegatedInteraction", _ => true);
        module.SetupVoid("unregisterDelegatedInteraction", _ => true);

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Uncontrolled_toggle_updates_pressed_state_metadata()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateToggle());

        IElement button = cut.Find("button");

        await Assert.That(button.GetAttribute("aria-pressed")).IsEqualTo("false");
        await Assert.That(button.GetAttribute("data-state")).IsEqualTo("off");

        await button.ClickAsync();
        button = cut.Find("button");

        await Assert.That(button.GetAttribute("aria-pressed")).IsEqualTo("true");
        await Assert.That(button.GetAttribute("data-state")).IsEqualTo("on");
    }

    [Test]
    public async Task Controlled_toggle_notifies_parent_without_changing_pressed_markup()
    {
        bool? requestedPressed = null;

        IRenderedComponent<ContainerFragment> cut = Render(CreateToggle(
            pressed: true,
            onPressedChange: EventCallback.Factory.Create<bool>(this, pressed => requestedPressed = pressed)));

        IElement button = cut.Find("button");
        await button.ClickAsync();
        button = cut.Find("button");

        await Assert.That(requestedPressed).IsEqualTo(false);
        await Assert.That(button.GetAttribute("aria-pressed")).IsEqualTo("true");
        await Assert.That(button.GetAttribute("data-state")).IsEqualTo("on");
    }

    private static RenderFragment CreateToggle(bool? pressed = null, bool defaultPressed = false, EventCallback<bool> onPressedChange = default)
    {
        return builder =>
        {
            builder.OpenComponent<BradixToggle>(0);

            if (pressed.HasValue)
                builder.AddAttribute(1, nameof(BradixToggle.Pressed), pressed.Value);

            builder.AddAttribute(2, nameof(BradixToggle.DefaultPressed), defaultPressed);

            if (onPressedChange.HasDelegate)
                builder.AddAttribute(3, nameof(BradixToggle.OnPressedChange), onPressedChange);

            builder.AddAttribute(4, nameof(BradixToggle.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.AddContent(0, "Toggle");
            }));
            builder.CloseComponent();
        };
    }
}