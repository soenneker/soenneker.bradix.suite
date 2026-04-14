using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixPopperRenderTests : BunitContext
{
    public BradixPopperRenderTests()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerPopperContent", _ => true);
        module.SetupVoid("updatePopperContent", _ => true);
        module.SetupVoid("unregisterPopperContent", _ => true);

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Popper_renders_anchor_and_content()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreatePopper());

        await Assert.That(cut.Markup).Contains("Anchor");
        await Assert.That(cut.Markup).Contains("Content");
    }

    [Test]
    public async Task Position_updates_are_reflected_in_attributes_and_callbacks()
    {
        int placedCount = 0;

        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixPopper>(0);
            builder.AddAttribute(1, nameof(BradixPopper.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixPopperAnchor>(0);
                content.AddAttribute(1, nameof(BradixPopperAnchor.ChildContent), (RenderFragment)(anchor =>
                {
                    anchor.OpenElement(0, "button");
                    anchor.AddContent(1, "Anchor");
                    anchor.CloseElement();
                }));
                content.CloseComponent();

                content.OpenComponent<BradixPopperContent>(2);
                content.AddAttribute(3, nameof(BradixPopperContent.OnPlaced), EventCallback.Factory.Create(this, () => placedCount++));
                content.AddAttribute(4, nameof(BradixPopperContent.ChildContent), (RenderFragment)(popperContent =>
                {
                    popperContent.AddContent(0, "Content");
                    popperContent.OpenComponent<BradixPopperArrow>(1);
                    popperContent.AddAttribute(2, nameof(BradixPopperArrow.ChildContent), (RenderFragment)(arrow =>
                    {
                        arrow.OpenElement(0, "span");
                        arrow.AddContent(1, "Arrow");
                        arrow.CloseElement();
                    }));
                    popperContent.CloseComponent();
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });

        IRenderedComponent<BradixPopperContent> content = cut.FindComponent<BradixPopperContent>();
        await content.Instance.HandlePositionChanged("top", "start", 12, 20, 300, 240, 90, 24, 14, null, false, false, "14px", "100px");

        await Assert.That(cut.Markup).Contains("data-side=\"top\"");
        await Assert.That(cut.Markup).Contains("data-align=\"start\"");
        await Assert.That(cut.Find("[aria-hidden='true']").GetAttribute("aria-hidden")).IsEqualTo("true");
        await Assert.That(placedCount).IsEqualTo(1);
    }

    [Test]
    public async Task Duplicate_position_updates_do_not_trigger_additional_rerenders()
    {
        int placedCount = 0;

        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixPopper>(0);
            builder.AddAttribute(1, nameof(BradixPopper.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixPopperAnchor>(0);
                content.AddAttribute(1, nameof(BradixPopperAnchor.ChildContent), (RenderFragment)(anchor =>
                {
                    anchor.OpenElement(0, "button");
                    anchor.AddContent(1, "Anchor");
                    anchor.CloseElement();
                }));
                content.CloseComponent();

                content.OpenComponent<BradixPopperContent>(2);
                content.AddAttribute(3, nameof(BradixPopperContent.OnPlaced), EventCallback.Factory.Create(this, () => placedCount++));
                content.AddAttribute(4, nameof(BradixPopperContent.ChildContent), (RenderFragment)(popperContent =>
                {
                    popperContent.AddContent(0, "Content");
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        });

        IRenderedComponent<BradixPopperContent> content = cut.FindComponent<BradixPopperContent>();
        await content.Instance.HandlePositionChanged("top", "start", 12, 20, 300, 240, 90, 24, 14, null, false, false, "14px", "100px");
        string markup = cut.Markup;

        await content.Instance.HandlePositionChanged("top", "start", 12, 20, 300, 240, 90, 24, 14, null, false, false, "14px", "100px");

        await Assert.That(cut.Markup).IsEqualTo(markup);
        await Assert.That(placedCount).IsEqualTo(1);
    }

    [Test]
    public async Task Popper_registers_rtl_direction_in_positioning_options()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerPopperContent", _ => true);
        module.SetupVoid("updatePopperContent", _ => true);
        module.SetupVoid("unregisterPopperContent", _ => true);

        Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, "Dir", "rtl");
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(content =>
            {
                content.AddContent(0, CreatePopper());
            }));
            builder.CloseComponent();
        });

        JSRuntimeInvocation invocation = module.Invocations.Single(call => call.Identifier == "registerPopperContent");
        object? options = invocation.Arguments[4];
        var dir = options?.GetType().GetProperty("dir")?.GetValue(options)?.ToString();

        await Assert.That(dir).IsEqualTo("rtl");
    }

    private static RenderFragment CreatePopper()
    {
        return builder =>
        {
            builder.OpenComponent<BradixPopper>(0);
            builder.AddAttribute(1, nameof(BradixPopper.ChildContent), (RenderFragment)(content =>
            {
                content.OpenComponent<BradixPopperAnchor>(0);
                content.AddAttribute(1, nameof(BradixPopperAnchor.ChildContent), (RenderFragment)(anchor =>
                {
                    anchor.OpenElement(0, "button");
                    anchor.AddContent(1, "Anchor");
                    anchor.CloseElement();
                }));
                content.CloseComponent();

                content.OpenComponent<BradixPopperContent>(2);
                content.AddAttribute(3, nameof(BradixPopperContent.ChildContent), (RenderFragment)(popperContent =>
                {
                    popperContent.AddContent(0, "Content");
                }));
                content.CloseComponent();
            }));
            builder.CloseComponent();
        };
    }
}