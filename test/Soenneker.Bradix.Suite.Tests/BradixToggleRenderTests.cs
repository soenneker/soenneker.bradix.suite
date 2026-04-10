using Bunit;
using Microsoft.AspNetCore.Components;
using Soenneker.Bradix.Suite.Toggle;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixToggleRenderTests : Bunit.BunitContext
{
    [Fact]
    public void Uncontrolled_toggle_updates_pressed_state_metadata()
    {
        var cut = Render(CreateToggle());

        var button = cut.Find("button");

        Assert.Equal("false", button.GetAttribute("aria-pressed"));
        Assert.Equal("off", button.GetAttribute("data-state"));

        button.Click();
        button = cut.Find("button");

        Assert.Equal("true", button.GetAttribute("aria-pressed"));
        Assert.Equal("on", button.GetAttribute("data-state"));
    }

    [Fact]
    public void Controlled_toggle_notifies_parent_without_changing_pressed_markup()
    {
        bool? requestedPressed = null;

        var cut = Render(CreateToggle(
            pressed: true,
            onPressedChange: EventCallback.Factory.Create<bool>(this, pressed => requestedPressed = pressed)));

        var button = cut.Find("button");
        button.Click();
        button = cut.Find("button");

        Assert.Equal(false, requestedPressed);
        Assert.Equal("true", button.GetAttribute("aria-pressed"));
        Assert.Equal("on", button.GetAttribute("data-state"));
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
