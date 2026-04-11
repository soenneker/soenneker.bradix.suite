using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixCollapsibleRenderTests : BunitContext
{
    public BradixCollapsibleRenderTests()
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
    public void Uncontrolled_toggle_updates_state_and_aria()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateCollapsible());

        IElement trigger = cut.Find("button");
        string contentId = trigger.GetAttribute("aria-controls")!;
        IElement content = cut.Find($"#{contentId}");

        Assert.Equal("false", trigger.GetAttribute("aria-expanded"));
        Assert.Equal("closed", trigger.GetAttribute("data-state"));
        Assert.True(content.HasAttribute("hidden"));
        Assert.DoesNotContain("Content", cut.Markup);

        trigger.Click();

        Assert.Equal("true", trigger.GetAttribute("aria-expanded"));
        Assert.Equal("open", trigger.GetAttribute("data-state"));
        Assert.False(content.HasAttribute("hidden"));
        Assert.Contains("Content", cut.Markup);
    }

    [Fact]
    public void Controlled_toggle_notifies_parent_without_closing_content()
    {
        bool? requestedOpen = null;

        IRenderedComponent<ContainerFragment> cut = Render(CreateCollapsible(
            open: true,
            onOpenChange: EventCallback.Factory.Create<bool>(this, open => requestedOpen = open)));

        IElement trigger = cut.Find("button");
        string contentId = trigger.GetAttribute("aria-controls")!;
        IElement content = cut.Find($"#{contentId}");

        trigger.Click();

        Assert.Equal(false, requestedOpen);
        Assert.Equal("true", trigger.GetAttribute("aria-expanded"));
        Assert.False(content.HasAttribute("hidden"));
        Assert.Contains("Content", cut.Markup);
    }

    private static RenderFragment CreateCollapsible(bool? open = null, bool defaultOpen = false, EventCallback<bool> onOpenChange = default)
    {
        return builder =>
        {
            builder.OpenComponent<BradixCollapsible>(0);

            if (open.HasValue)
                builder.AddAttribute(1, nameof(BradixCollapsible.Open), open.Value);

            builder.AddAttribute(2, nameof(BradixCollapsible.DefaultOpen), defaultOpen);

            if (onOpenChange.HasDelegate)
                builder.AddAttribute(3, nameof(BradixCollapsible.OnOpenChange), onOpenChange);

            builder.AddAttribute(4, nameof(BradixCollapsible.ChildContent), (RenderFragment) (contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixCollapsibleTrigger>(0);
                contentBuilder.AddAttribute(1, nameof(BradixCollapsibleTrigger.ChildContent), (RenderFragment) (triggerBuilder =>
                {
                    triggerBuilder.AddContent(0, "Trigger");
                }));
                contentBuilder.CloseComponent();

                contentBuilder.OpenComponent<BradixCollapsibleContent>(2);
                contentBuilder.AddAttribute(3, nameof(BradixCollapsibleContent.ChildContent), (RenderFragment) (childBuilder =>
                {
                    childBuilder.AddContent(0, "Content");
                }));
                contentBuilder.CloseComponent();
            }));

            builder.CloseComponent();
        };
    }
}
