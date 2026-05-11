using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixAccordionAnimationRenderTests : BunitContext
{
    public BradixAccordionAnimationRenderTests()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("observeCollapsibleContent", _ => true).SetVoidResult();
        module.SetupVoid("unobserveCollapsibleContent", _ => true).SetVoidResult();
        module.SetupVoid("registerPresence", _ => true).SetVoidResult();
        module.SetupVoid("unregisterPresence", _ => true).SetVoidResult();
        module.SetupVoid("registerRovingFocusNavigationKeys", _ => true).SetVoidResult();
        module.SetupVoid("unregisterRovingFocusNavigationKeys", _ => true).SetVoidResult();
        module.Setup<BradixPresenceSnapshot>("getPresenceState", _ => true)
            .SetResult(new BradixPresenceSnapshot { AnimationName = "accordion-up", Display = "block" });
        Services.AddBradixTestInterops();
    }

    [Test]
    public async Task Content_stays_mounted_in_closed_state_when_exit_animation_is_detected()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateAccordion());

        IElement trigger = cut.Find("button");

        await trigger.ClickAsync();
        await Assert.That(cut.Markup).Contains("Content One");

        await trigger.ClickAsync();

        IElement content = cut.Find("[role='region'][data-state='closed']");
        await Assert.That(content.HasAttribute("hidden")).IsFalse();
        await Assert.That(cut.Markup).Contains("Content One");
        await Assert.That(content.HasAttribute("data-closed")).IsTrue();
    }

    private static RenderFragment CreateAccordion()
    {
        return builder =>
        {
            builder.OpenComponent<BradixAccordion>(0);
            builder.AddAttribute(1, nameof(BradixAccordion.Type), (object)SelectionMode.Multiple);
            builder.AddAttribute(2, nameof(BradixAccordion.Collapsible), true);
            builder.AddAttribute(3, nameof(BradixAccordion.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixAccordionItem>(0);
                contentBuilder.AddAttribute(1, nameof(BradixAccordionItem.Value), "one");
                contentBuilder.AddAttribute(2, nameof(BradixAccordionItem.ChildContent), (RenderFragment)(itemBuilder =>
                {
                    itemBuilder.OpenComponent<BradixAccordionHeader>(0);
                    itemBuilder.AddAttribute(1, nameof(BradixAccordionHeader.ChildContent), (RenderFragment)(headerBuilder =>
                    {
                        headerBuilder.OpenComponent<BradixAccordionTrigger>(0);
                        headerBuilder.AddAttribute(1, nameof(BradixAccordionTrigger.ChildContent), (RenderFragment)(triggerBuilder =>
                        {
                            triggerBuilder.AddContent(0, "Trigger One");
                        }));
                        headerBuilder.CloseComponent();
                    }));
                    itemBuilder.CloseComponent();

                    itemBuilder.OpenComponent<BradixAccordionContent>(2);
                    itemBuilder.AddAttribute(3, nameof(BradixAccordionContent.ChildContent), (RenderFragment)(contentInnerBuilder =>
                    {
                        contentInnerBuilder.AddContent(0, "Content One");
                    }));
                    itemBuilder.CloseComponent();
                }));
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }
}
