using System.Collections.Generic;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixAccordionRenderTests : BunitContext
{
    public BradixAccordionRenderTests()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("observeCollapsibleContent", _ => true);
        module.SetupVoid("unobserveCollapsibleContent", _ => true);
        module.SetupVoid("registerDelegatedInteraction", _ => true);
        module.SetupVoid("unregisterDelegatedInteraction", _ => true);

        Services.AddScoped<IBradixIdGenerator, BradixIdGenerator>();
        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Fact]
    public void Single_mode_keeps_open_item_expanded_until_another_item_opens()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateSingleAccordion());

        IReadOnlyList<IElement> triggers = cut.FindAll("button");

        triggers[0].Click();

        triggers = cut.FindAll("button");

        Assert.Contains("Content One", cut.Markup);
        Assert.Equal("true", triggers[0].GetAttribute("aria-expanded"));
        Assert.Equal("true", triggers[0].GetAttribute("aria-disabled"));

        triggers[0].Click();

        triggers = cut.FindAll("button");

        Assert.Contains("Content One", cut.Markup);

        triggers[1].Click();

        triggers = cut.FindAll("button");

        Assert.DoesNotContain("Content One", cut.Markup);
        Assert.Contains("Content Two", cut.Markup);
        Assert.Equal("true", triggers[1].GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Multiple_mode_allows_independent_item_state()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateMultipleAccordion());

        IReadOnlyList<IElement> triggers = cut.FindAll("button");

        triggers[0].Click();
        triggers[1].Click();

        triggers = cut.FindAll("button");

        Assert.Contains("Content One", cut.Markup);
        Assert.Contains("Content Two", cut.Markup);

        triggers[0].Click();

        triggers = cut.FindAll("button");

        Assert.DoesNotContain("Content One", cut.Markup);
        Assert.Contains("Content Two", cut.Markup);
        Assert.Equal("false", triggers[0].GetAttribute("aria-expanded"));
        Assert.Equal("true", triggers[1].GetAttribute("aria-expanded"));
    }

    private static RenderFragment CreateSingleAccordion()
    {
        return CreateAccordion("single", collapsible: false);
    }

    private static RenderFragment CreateMultipleAccordion()
    {
        return CreateAccordion("multiple", collapsible: true);
    }

    private static RenderFragment CreateAccordion(string type, bool collapsible)
    {
        return builder =>
        {
            builder.OpenComponent<BradixAccordion>(0);
            builder.AddAttribute(1, nameof(BradixAccordion.Type), type);
            builder.AddAttribute(2, nameof(BradixAccordion.Collapsible), collapsible);
            builder.AddAttribute(3, nameof(BradixAccordion.ChildContent), (RenderFragment) (contentBuilder =>
            {
                RenderItem(contentBuilder, 0, "one", "One");
                RenderItem(contentBuilder, 10, "two", "Two");
            }));
            builder.CloseComponent();
        };
    }

    private static void RenderItem(RenderTreeBuilder builder, int sequence, string value, string label)
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
            itemBuilder.AddAttribute(3, nameof(BradixAccordionContent.ChildContent), (RenderFragment) (contentInnerBuilder =>
            {
                contentInnerBuilder.AddContent(0, $"Content {label}");
            }));
            itemBuilder.CloseComponent();
        }));
        builder.CloseComponent();
    }
}
