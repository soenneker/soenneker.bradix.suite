using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixIssueRegressionTests : BunitContext
{
    public BradixIssueRegressionTests()
    {
        JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js").Mode = JSRuntimeMode.Loose;
        Services.AddBradixTestInterops();
    }

    [Test]
    [Arguments("touch")]
    [Arguments("pen")]
    public async Task Dropdown_waits_for_non_mouse_click(string pointerType)
    {
        var cut = Render<BradixDropdownMenu>(p => p.AddChildContent<BradixDropdownMenuTrigger>());
        await cut.Find("button").PointerDownAsync(new PointerEventArgs { PointerType = pointerType, Button = 0 });
        await Assert.That(cut.Find("button").GetAttribute("aria-expanded")).IsEqualTo("false");
        await cut.Find("button").ClickAsync(new MouseEventArgs { Detail = 1 });
        await Assert.That(cut.Find("button").GetAttribute("aria-expanded")).IsEqualTo("true");
        await cut.Find("button").PointerDownAsync(new PointerEventArgs { PointerType = pointerType, Button = 0 });
        await cut.Find("button").ClickAsync(new MouseEventArgs { Detail = 1 });
        await Assert.That(cut.Find("button").GetAttribute("aria-expanded")).IsEqualTo("false");
    }

    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task Popover_keyboard_loop_follows_modality(bool modal)
    {
        var cut = Render<BradixPopover>(p => p.Add(c => c.DefaultOpen, true)
            .Add(c => c.Modal, modal).AddChildContent<BradixPopoverContent>());
        var scope = cut.FindComponent<BradixFocusScope>().Instance;
        await Assert.That(scope.Loop).IsEqualTo(modal);
        await Assert.That(scope.Trapped).IsEqualTo(modal);
    }

    [Test]
    public async Task Radio_group_reset_uses_current_default_without_changing_value_before_reset()
    {
        var cut = Render<BradixRadioGroup>(p => p.Add(c => c.DefaultValue, "a").AddChildContent(b =>
        {
            b.OpenComponent<BradixRadioGroupItem>(0); b.AddAttribute(1, "Value", "a"); b.AddAttribute(2, "Id", "a"); b.CloseComponent();
            b.OpenComponent<BradixRadioGroupItem>(3); b.AddAttribute(4, "Value", "b"); b.AddAttribute(5, "Id", "b"); b.CloseComponent();
        }));
        cut.Render(p => p.Add(c => c.DefaultValue, "b"));
        await Assert.That(cut.Find("[role=radio][aria-checked=true]").Id).IsEqualTo("a");
        await cut.InvokeAsync(cut.Instance.HandleFormReset);
        await Assert.That(cut.Find("[role=radio][aria-checked=true]").Id).IsEqualTo("b");
        cut.Render(p => p.Add(c => c.DefaultValue, (string?)null));
        await cut.InvokeAsync(cut.Instance.HandleFormReset);
        await Assert.That(cut.FindAll("[role=radio][aria-checked=true]").Count).IsEqualTo(0);
    }
}
