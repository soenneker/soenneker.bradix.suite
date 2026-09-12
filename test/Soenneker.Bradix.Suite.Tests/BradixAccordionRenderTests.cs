using System.Collections.Generic;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixAccordionRenderTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public BradixAccordionRenderTests()
    {
        BunitJSModuleInterop module = _module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("observeCollapsibleContent", _ => true).SetVoidResult();
        module.SetupVoid("unobserveCollapsibleContent", _ => true).SetVoidResult();
        module.SetupVoid("registerPresence", _ => true).SetVoidResult();
        module.SetupVoid("unregisterPresence", _ => true).SetVoidResult();
        module.SetupVoid("registerRovingFocusNavigationKeys", _ => true).SetVoidResult();
        module.SetupVoid("unregisterRovingFocusNavigationKeys", _ => true).SetVoidResult();
        module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "none", Display = "block" });
        Services.AddBradixTestInterops();
    }

    [Test]
    public async ValueTask Single_mode_keeps_open_item_expanded_until_another_item_opens()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSingleAccordion());

        IReadOnlyList<IElement> triggers = cut.FindAll("button");
        IElement firstItem = cut.Find("[data-state='closed'][data-orientation='vertical']");

        await triggers[0].ClickAsync();

        triggers = cut.FindAll("button");

        await Assert.That(firstItem.GetAttribute("data-orientation")).IsEqualTo("vertical");
        await Assert.That(cut.Markup).Contains("Content One");
        await Assert.That(triggers[0].GetAttribute("aria-expanded")).IsEqualTo("true");
        await Assert.That(triggers[0].GetAttribute("aria-disabled")).IsEqualTo("true");

        await triggers[0].ClickAsync();

        triggers = cut.FindAll("button");

        await Assert.That(cut.Markup).Contains("Content One");

        await triggers[1].ClickAsync();

        triggers = cut.FindAll("button");

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.Markup).DoesNotContain("Content One");
            await Assert.That(cut.Markup).Contains("Content Two");
        });
        await Assert.That(triggers[1].GetAttribute("aria-expanded")).IsEqualTo("true");
    }

    [Test]
    public async Task Multiple_mode_allows_independent_item_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMultipleAccordion());

        IReadOnlyList<IElement> triggers = cut.FindAll("button");

        await triggers[0].ClickAsync();
        await triggers[1].ClickAsync();

        triggers = cut.FindAll("button");

        await Assert.That(cut.Markup).Contains("Content One");
        await Assert.That(cut.Markup).Contains("Content Two");

        await triggers[0].ClickAsync();

        triggers = cut.FindAll("button");

        await cut.WaitForAssertionAsync(async  () =>
        {
            await Assert.That(cut.Markup).DoesNotContain("Content One");
            await Assert.That(cut.Markup).Contains("Content Two");
        });
        await Assert.That(triggers[0].GetAttribute("aria-expanded")).IsEqualTo("false");
        await Assert.That(triggers[1].GetAttribute("aria-expanded")).IsEqualTo("true");
    }

    [Test]
    public async Task Multiple_mode_preserves_open_order_in_value_callback()
    {
        IReadOnlyCollection<string>? requestedValues = null;

        IRenderedComponent<ContainerFragment> cut = Render(CreateAccordion(
            SelectionMode.Multiple,
            collapsible: true,
            onValuesChange: EventCallback.Factory.Create<IReadOnlyCollection<string>>(this, values => requestedValues = values)));

        IReadOnlyList<IElement> triggers = cut.FindAll("button");
        await triggers[1].ClickAsync();
        await triggers[0].ClickAsync();

        await Assert.That(requestedValues).IsNotNull();
        await Assert.That(string.Join(",", requestedValues!)).IsEqualTo("two,one");
    }

    [Test]
    public async Task Content_style_merging_tolerates_missing_trailing_semicolon()
    {
        IReadOnlyDictionary<string, object> contentAttributes = new Dictionary<string, object>
        {
            ["style"] = "--radix-accordion-content-height: 0px"
        };

        IRenderedComponent<ContainerFragment> cut = Render(CreateAccordion(
            SelectionMode.Single,
            collapsible: true,
            contentAdditionalAttributes: contentAttributes));

        await cut.Find("button").ClickAsync();

        IElement content = cut.Find("[role='region']");
        string style = content.GetAttribute("style") ?? string.Empty;

        await Assert.That(style).Contains("--radix-accordion-content-height: 0px;");
        await Assert.That(style).Contains("--radix-accordion-content-width: var(--radix-collapsible-content-width);");
        await Assert.That(style).DoesNotContain("0px --radix-accordion-content-height");
    }

    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task Trigger_async_cleanup_runs_once_even_after_sync_disposal(bool disposeSynchronouslyFirst)
    {
        var cut = Render(CreateSingleAccordion());
        var trigger = cut.FindComponent<BradixAccordionTrigger>();
        await trigger.InvokeAsync(async () =>
        {
            if (disposeSynchronouslyFirst)
                trigger.Instance.Dispose();
            await trigger.Instance.DisposeAsync();
            await trigger.Instance.DisposeAsync();
        });
        await Assert.That(_module.Invocations["unregisterRovingFocusNavigationKeys"].Count).IsEqualTo(1);
    }

    private static RenderFragment CreateSingleAccordion()
    {
        return CreateAccordion(SelectionMode.Single, collapsible: false);
    }

    [Test]
    public async Task Keyboard_navigation_drops_removed_triggers()
    {
        var focus = JSInterop.SetupVoid("Blazor._internal.domWrapper.focus", _ => true);
        focus.SetVoidResult();
        bool showFirst = true;
        RenderFragment content = builder =>
        {
            if (showFirst)
                RenderItem(builder, 0, "one", "One");
            RenderItem(builder, 10, "two", "Two");
        };
        var cut = Render<BradixAccordion>(p => p.Add(c => c.ChildContent, content));
        await cut.FindAll("button")[1].KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Home" });
        var removedElement = (ElementReference)focus.Invocations["Blazor._internal.domWrapper.focus"][0].Arguments[0]!;
        showFirst = false;
        cut.Render();
        await cut.Find("button").KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Home" });
        var remainingElement = (ElementReference)focus.Invocations["Blazor._internal.domWrapper.focus"][1].Arguments[0]!;
        await Assert.That(remainingElement.Id).IsNotEqualTo(removedElement.Id);
        await cut.Find("button").KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowDown" });
        await Assert.That(((ElementReference)focus.Invocations["Blazor._internal.domWrapper.focus"][2].Arguments[0]!).Id).IsEqualTo(remainingElement.Id);
    }

    private static RenderFragment CreateMultipleAccordion()
    {
        return CreateAccordion(SelectionMode.Multiple, collapsible: true);
    }

    private static RenderFragment CreateAccordion(SelectionMode type, bool collapsible,
        EventCallback<IReadOnlyCollection<string>> onValuesChange = default, IReadOnlyDictionary<string, object>? contentAdditionalAttributes = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixAccordion>(0);
            builder.AddAttribute(1, nameof(BradixAccordion.Type), (object) type);
            builder.AddAttribute(2, nameof(BradixAccordion.Collapsible), collapsible);
            if (onValuesChange.HasDelegate)
                builder.AddAttribute(4, nameof(BradixAccordion.OnValuesChange), onValuesChange);

            builder.AddAttribute(3, nameof(BradixAccordion.ChildContent), (RenderFragment) (contentBuilder =>
            {
                RenderItem(contentBuilder, 0, "one", "One", contentAdditionalAttributes);
                RenderItem(contentBuilder, 10, "two", "Two", contentAdditionalAttributes);
            }));
            builder.CloseComponent();
        };
    }

    private static void RenderItem(RenderTreeBuilder builder, int sequence, string value, string label,
        IReadOnlyDictionary<string, object>? contentAdditionalAttributes = null)
    {
        builder.OpenComponent<BradixAccordionItem>(sequence);
        builder.AddAttribute(sequence + 1, nameof(BradixAccordionItem.Value), value);
        builder.AddAttribute(sequence + 2, nameof(BradixAccordionItem.ChildContent), (RenderFragment) (itemBuilder =>
        {
            itemBuilder.OpenComponent<BradixAccordionHeader>(0);
            itemBuilder.AddAttribute(1, nameof(BradixAccordionHeader.ChildContent), (RenderFragment) (headerBuilder =>
            {
                headerBuilder.OpenComponent<BradixAccordionTrigger>(0);
                headerBuilder.AddAttribute(1, nameof(BradixAccordionTrigger.ChildContent), (RenderFragment) (triggerBuilder =>
                {
                    triggerBuilder.AddContent(0, $"Trigger {label}");
                }));
                headerBuilder.CloseComponent();
            }));
            itemBuilder.CloseComponent();

            itemBuilder.OpenComponent<BradixAccordionContent>(2);
            if (contentAdditionalAttributes is not null)
                itemBuilder.AddAttribute(3, nameof(BradixAccordionContent.AdditionalAttributes), contentAdditionalAttributes);

            itemBuilder.AddAttribute(4, nameof(BradixAccordionContent.ChildContent), (RenderFragment) (contentInnerBuilder =>
            {
                contentInnerBuilder.AddContent(0, $"Content {label}");
            }));
            itemBuilder.CloseComponent();
        }));
        builder.CloseComponent();
    }
}
