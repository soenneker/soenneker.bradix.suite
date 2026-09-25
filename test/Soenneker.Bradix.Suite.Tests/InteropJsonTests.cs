using System.Text.Json;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class InteropJsonTests : BunitContext
{
    [Test]
    public async Task Delegated_options_preserve_explicit_false_and_omit_unconfigured_events()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerDelegatedInteraction", _ => true).SetVoidResult();
        await using var interop = new DelegatedInteractionInterop(new BradixTestModuleImportUtil(JSInterop.JSRuntime));
        await interop.RegisterDelegatedInteraction(default, new object(), new BradixDelegatedInteractionOptions
        {
            Keydown = new BradixDelegatedEventOptions { CurrentTargetOnly = false, Keys = ["Enter", " "], PreventDefault = true }
        });

        var payload = (JsonElement)module.Invocations["registerDelegatedInteraction"][0].Arguments[2]!;
        await Assert.That(payload.TryGetProperty("click", out _)).IsFalse();
        var keydown = payload.GetProperty("keydown");
        await Assert.That(keydown.GetProperty("currentTargetOnly").GetBoolean()).IsFalse();
        await Assert.That(keydown.TryGetProperty("checkForDefaultPrevented", out _)).IsFalse();
        await Assert.That(keydown.GetProperty("keys")[1].GetString()).IsEqualTo(" ");
    }

    [Test]
    public async Task Form_results_deserialize_nested_browser_payloads()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        using var json = JsonDocument.Parse("""{"value":"a","validity":{"valid":false,"valueMissing":true},"formData":{"values":{"roles":["admin","editor"]}}}""");
        module.Setup<JsonElement?>("getFormControlState", _ => true).SetResult(json.RootElement);
        await using var interop = new FormInterop(new BradixTestModuleImportUtil(JSInterop.JSRuntime));
        var result = await interop.GetFormControlState(default);
        await Assert.That(result.Validity.Valid).IsFalse();
        await Assert.That(result.Validity.ValueMissing).IsTrue();
        await Assert.That(result.FormData.GetAll("roles").Count).IsEqualTo(2);
        await Assert.That(result.FormData.GetAll("roles")[1]).IsEqualTo("editor");
    }

    [Test]
    public async Task Null_presence_result_preserves_default_snapshot()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.Setup<JsonElement?>("getPresenceState", _ => true).SetResult(null);
        await using var interop = new PresenceOverlayInterop(new BradixTestModuleImportUtil(JSInterop.JSRuntime));
        var result = await interop.GetPresenceState(default);
        await Assert.That(result.AnimationName).IsEqualTo("none");
        await Assert.That(result.HasActiveAnimation).IsNull();
    }

    [Test]
    public async Task Keyboard_callback_deserializes_browser_event_before_dispatch()
    {
        BradixDelegatedKeyboardEvent? received = null;
        var layer = new BradixDismissableLayer
        {
            OnEscapeKeyDownDetailed = EventCallback.Factory.Create<BradixEscapeKeyDownEventArgs>(this,
                args => received = args.OriginalEvent)
        };
        using var json = JsonDocument.Parse("""{"key":"Escape","ctrlKey":true,"ancestorIds":["dialog","root"]}""");
        await layer.HandleEscapeKeyDownFromJson(json.RootElement);
        await Assert.That(received!.Key).IsEqualTo("Escape");
        await Assert.That(received.CtrlKey).IsTrue();
        await Assert.That(received.AncestorIds[1]).IsEqualTo("root");
        await layer.HandleEscapeKeyDownFromJson();
        await Assert.That(received.Key).IsEqualTo(string.Empty);
    }
}
