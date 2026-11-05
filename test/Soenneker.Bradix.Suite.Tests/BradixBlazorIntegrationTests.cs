using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixBlazorIntegrationTests : BunitContext
{
    public BradixBlazorIntegrationTests()
    {
        JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js").Mode = JSRuntimeMode.Loose;
        Services.AddBradixTestInterops();
    }

    [Test]
    [Arguments(typeof(BradixDialog), typeof(BradixDialogTrigger))]
    [Arguments(typeof(BradixAlertDialog), typeof(BradixAlertDialogTrigger))]
    [Arguments(typeof(BradixPopover), typeof(BradixPopoverTrigger))]
    [Arguments(typeof(BradixCollapsible), typeof(BradixCollapsibleTrigger))]
    public async Task Custom_trigger_preserves_semantics_and_awaits_cancelable_activation(Type rootType, Type triggerType)
    {
        var callbacks = new List<string>();
        bool cancel = true;
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        BradixTriggerContext? context = null;
        RenderFragment<BradixTriggerContext> template = value => b =>
        {
            context = value;
            b.OpenElement(0, "button");
            b.AddMultipleAttributes(1, value.Attributes);
            b.AddAttribute(2, "onclick", value.OnClick);
            b.AddElementReferenceCapture(3, element => value.Element = element);
            b.AddContent(4, "Custom trigger");
            b.CloseElement();
        };
        RenderFragment child = b =>
        {
            b.OpenComponent(0, triggerType);
            b.AddAttribute(1, "TriggerTemplate", template);
            b.AddAttribute(2, "OnClick", EventCallback.Factory.Create<MouseEventArgs>(this, _ => callbacks.Add("click")));
            b.AddAttribute(3, "OnActivate", EventCallback.Factory.Create<BradixActivationEventArgs>(this, async args =>
            {
                callbacks.Add("activate");
                await completion.Task;
                if (cancel) args.PreventDefault();
            }));
            b.CloseComponent();
        };
        var cut = Render<DynamicComponent>(p => p.Add(c => c.Type, rootType)
            .Add(c => c.Parameters, new Dictionary<string, object> { ["ChildContent"] = child }));
        await Assert.That(cut.FindAll("button").Count).IsEqualTo(1);
        await Assert.That(context!.Element.Id).IsNotNullOrEmpty();
        Task click = cut.Find("button").ClickAsync(new MouseEventArgs());
        await Assert.That(cut.Find("button").GetAttribute("aria-expanded")).IsEqualTo("false");
        completion.SetResult();
        await click;
        await Assert.That(string.Join(",", callbacks)).IsEqualTo("click,activate");
        await Assert.That(cut.Find("button").GetAttribute("aria-expanded")).IsEqualTo("false");
        cancel = false;
        await cut.Find("button").ClickAsync(new MouseEventArgs());
        await Assert.That(cut.Find("button").GetAttribute("aria-expanded")).IsEqualTo("true");
        if (rootType == typeof(BradixDialog))
            await Assert.That(cut.FindComponent<BradixDialog>().Instance.TriggerElement.Id).IsEqualTo(context.Element.Id);
    }

    [Test]
    public async Task Disabled_trigger_does_not_invoke_activation_even_when_callback_is_called_directly()
    {
        int calls = 0;
        BradixTriggerContext? context = null;
        var cut = Render<BradixDialog>(p => p.AddChildContent<BradixDialogTrigger>(t => t
            .Add(c => c.Disabled, true)
            .Add(c => c.OnActivate, _ => calls++)
            .Add(c => c.TriggerTemplate, value => b => { context = value; })));
        await cut.InvokeAsync(() => context!.OnClick.InvokeAsync(new MouseEventArgs()));
        await Assert.That(calls).IsEqualTo(0);
        await Assert.That(cut.Instance.CurrentOpen).IsFalse();
    }

    [Test]
    public async Task Select_value_template_updates_and_preserves_placeholder_precedence()
    {
        RenderFragment content = b =>
        {
            b.OpenComponent<BradixSelectValue>(0);
            b.AddAttribute(1, "Placeholder", (RenderFragment)(c => c.AddContent(0, "Choose")));
            b.AddAttribute(2, "ValueTemplate", (RenderFragment<string?>)(value => c =>
            {
                c.OpenElement(0, "strong"); c.AddContent(1, value); c.CloseElement();
            }));
            b.CloseComponent();
        };
        var cut = Render<BradixSelect>(p => p.Add(c => c.Value, "first").AddChildContent(content));
        await Assert.That(cut.Find("strong").TextContent).IsEqualTo("first");
        cut.Render(p => p.Add(c => c.Value, "second"));
        await Assert.That(cut.Find("strong").TextContent).IsEqualTo("second");
        cut.Render(p => p.Add(c => c.Value, ""));
        await Assert.That(cut.FindAll("strong").Count).IsEqualTo(0);
        await Assert.That(cut.FindComponent<BradixSelectValue>().Markup).Contains("Choose");
    }

    [Test]
    public async Task Typed_adapter_updates_model_field_state_and_validation_without_a_wrapper()
    {
        var model = new BradixInputTestModel();
        var editContext = new EditContext(model);
        var field = new FieldIdentifier(model, nameof(model.Choice));
        var messages = new ValidationMessageStore(editContext);
        BradixInputContext<string?>? context = null;
        RenderFragment input = b =>
        {
            b.OpenComponent<BradixInput<string?>>(0);
            b.AddAttribute(1, "Value", model.Choice);
            b.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, value => model.Choice = value));
            b.AddAttribute(3, "ValueExpression", (Expression<Func<string?>>)(() => model.Choice));
            b.AddAttribute(4, "ChildContent", (RenderFragment<BradixInputContext<string?>>)(value => c =>
            {
                context = value;
                c.OpenElement(0, "button"); c.AddMultipleAttributes(1, value.Attributes); c.AddContent(2, value.Value); c.CloseElement();
            }));
            b.CloseComponent();
        };
        var cut = Render<CascadingValue<EditContext>>(p => p.Add(c => c.Value, editContext).AddChildContent(input));
        await cut.InvokeAsync(() => context!.ValueChanged.InvokeAsync("second"));
        await Assert.That(model.Choice).IsEqualTo("second");
        await Assert.That(editContext.IsModified(field)).IsTrue();
        await Assert.That(cut.Find("button").ClassList.Contains("modified")).IsTrue();
        await cut.InvokeAsync(() => { messages.Add(field, "Choose another value"); editContext.NotifyValidationStateChanged(); });
        await Assert.That(context!.ValidationMessages.Single()).IsEqualTo("Choose another value");
        await Assert.That(cut.Find("button").GetAttribute("aria-invalid")).IsEqualTo("true");
        await cut.InvokeAsync(() => { messages.Clear(field); editContext.NotifyValidationStateChanged(); });
        await Assert.That(context!.ValidationMessages.Count).IsEqualTo(0);
        await Assert.That(cut.Find("button").HasAttribute("aria-invalid")).IsFalse();
        await Assert.That(cut.Nodes.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Announcements_share_regions_change_priority_and_clean_up_on_removal()
    {
        var cut = Render<BradixAnnounceProvider>(p => p.AddChildContent(Announcements("Saved", "polite")));
        await Assert.That(cut.FindAll("[aria-live]").Count).IsEqualTo(2);
        await cut.WaitForAssertionAsync(async () => await Assert.That(cut.Find("[aria-live=polite]").TextContent).Contains("Saved"));
        cut.Render(p => p.AddChildContent(Announcements("Failed", "assertive")));
        await cut.WaitForAssertionAsync(async () => await Assert.That(cut.Find("[aria-live=assertive]").TextContent).Contains("Failed"));
        await Assert.That(cut.Find("[aria-live=polite]").TextContent.Trim()).IsEqualTo("Second");
        cut.Render(p => p.AddChildContent(Announcements("Silent", "off")));
        await cut.WaitForAssertionAsync(async () => await Assert.That(cut.Find("[aria-live=assertive]").TextContent.Trim()).IsEqualTo(""));
        cut.Render(p => p.AddChildContent((RenderFragment)(_ => { })));
        await cut.WaitForAssertionAsync(async () => await Assert.That(cut.Find("[aria-live=polite]").TextContent.Trim()).IsEqualTo(""));
        await Assert.That(cut.FindAll("[aria-live]").Count).IsEqualTo(2);
    }

    [Test]
    public async Task Prerender_emits_empty_live_regions_without_requiring_browser_interop()
    {
        await using var renderer = new HtmlRenderer(Services, NullLoggerFactory.Instance);
        string html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var root = await renderer.RenderComponentAsync<BradixAnnounceProvider>(ParameterView.FromDictionary(
                new Dictionary<string, object?> { ["ChildContent"] = Announcements("After interactivity", "polite") }));
            return root.ToHtmlString();
        });
        await Assert.That(html).Contains("aria-live=\"polite\"");
        await Assert.That(html).Contains("aria-live=\"assertive\"");
        await Assert.That(html).DoesNotContain("After interactivity");
        await Assert.That(JSInterop.Invocations.Count).IsEqualTo(0);
    }

    private static RenderFragment Announcements(string message, string type) => b =>
    {
        b.OpenComponent<BradixAnnounce>(0);
        b.AddAttribute(1, "Type", type);
        b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddContent(0, message)));
        b.CloseComponent();
        b.OpenComponent<BradixAnnounce>(3);
        b.AddAttribute(4, "ChildContent", (RenderFragment)(c => c.AddContent(0, "Second")));
        b.CloseComponent();
    };
}
