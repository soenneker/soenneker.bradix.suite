using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixPopperRenderTests : BunitContext
{
    public BradixPopperRenderTests()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerPopperContent", _ => true);
        module.SetupVoid("updatePopperContent", _ => true);
        module.SetupVoid("unregisterPopperContent", _ => true);

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Fact]
    public void Popper_renders_anchor_and_content()
    {
        var cut = Render(CreatePopper());

        Assert.Contains("Anchor", cut.Markup);
        Assert.Contains("Content", cut.Markup);
    }

    [Fact]
    public async Task Position_updates_are_reflected_in_attributes_and_callbacks()
    {
        int placedCount = 0;

        var cut = Render(builder =>
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

        var content = cut.FindComponent<BradixPopperContent>();
        await content.Instance.HandlePositionChanged("top", "start", 12, 20, 300, 240, 90, 24, 14, null, false, false, "14px", "100px");

        Assert.Contains("data-side=\"top\"", cut.Markup);
        Assert.Contains("data-align=\"start\"", cut.Markup);
        Assert.Equal("true", cut.Find("[aria-hidden='true']").GetAttribute("aria-hidden"));
        Assert.Equal(1, placedCount);
    }

    [Fact]
    public async Task Duplicate_position_updates_do_not_trigger_additional_rerenders()
    {
        int placedCount = 0;

        var cut = Render(builder =>
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

        var content = cut.FindComponent<BradixPopperContent>();
        await content.Instance.HandlePositionChanged("top", "start", 12, 20, 300, 240, 90, 24, 14, null, false, false, "14px", "100px");
        string markup = cut.Markup;

        await content.Instance.HandlePositionChanged("top", "start", 12, 20, 300, 240, 90, 24, 14, null, false, false, "14px", "100px");

        Assert.Equal(markup, cut.Markup);
        Assert.Equal(1, placedCount);
    }

    [Fact]
    public void Popper_registers_rtl_direction_in_positioning_options()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
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

        var invocation = module.Invocations.Single(call => call.Identifier == "registerPopperContent");
        var options = invocation.Arguments[4];
        var dir = options?.GetType().GetProperty("dir")?.GetValue(options)?.ToString();

        Assert.Equal("rtl", dir);
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
