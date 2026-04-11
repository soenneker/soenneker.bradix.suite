using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixScrollAreaRenderTests : BunitContext
{
    public BradixScrollAreaRenderTests()
    {
        var module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerScrollAreaRoot", _ => true);
        module.SetupVoid("unregisterScrollAreaRoot", _ => true);
        module.SetupVoid("registerScrollAreaViewport", _ => true);
        module.SetupVoid("unregisterScrollAreaViewport", _ => true);
        module.SetupVoid("registerScrollAreaScrollbar", _ => true);
        module.SetupVoid("unregisterScrollAreaScrollbar", _ => true);

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Fact]
    public void Always_scroll_area_renders_scrollbar_parts()
    {
        var cut = Render(CreateScrollArea(type: "always", includeHorizontal: true));

        Assert.Equal(2, cut.FindAll("[data-orientation]").Count);
    }

    [Fact]
    public async Task Auto_scroll_area_mounts_scrollbar_when_overflow_detected()
    {
        var cut = Render(CreateScrollArea(type: "auto"));
        var root = cut.FindComponent<BradixScrollArea>();

        Assert.Empty(cut.FindAll("[data-state='visible']"));

        await root.Instance.HandleViewportMetricsChangedAsync(0, 0, 500, 500, 100, 100);

        Assert.NotEmpty(cut.FindAll("[data-state='visible']"));
    }

    [Fact]
    public async Task Hover_scroll_area_shows_scrollbar_on_hover()
    {
        var cut = Render(CreateScrollArea(type: "hover"));
        var root = cut.FindComponent<BradixScrollArea>();

        await root.Instance.HandleViewportMetricsChangedAsync(0, 0, 500, 500, 100, 100);
        Assert.Empty(cut.FindAll("[data-state='visible']"));

        await root.Instance.HandleHoverChangedAsync(true);
        Assert.NotEmpty(cut.FindAll("[data-state='visible']"));
    }

    [Fact]
    public async Task Corner_renders_when_both_scrollbars_visible()
    {
        var cut = Render(CreateScrollArea(type: "always", includeHorizontal: true, includeCorner: true));
        var root = cut.FindComponent<BradixScrollArea>();

        await root.Instance.HandleViewportMetricsChangedAsync(0, 0, 500, 500, 100, 100);
        await root.Instance.HandleScrollbarMetricsChangedAsync("horizontal", 100, 12, 0, 0);
        await root.Instance.HandleScrollbarMetricsChangedAsync("vertical", 12, 100, 0, 0);

        Assert.Single(cut.FindAll(".corner"));
    }

    [Fact]
    public async Task Inherited_direction_sets_root_dir()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, nameof(BradixDirectionProvider.Dir), "rtl");
            builder.AddAttribute(2, nameof(BradixDirectionProvider.ChildContent), (RenderFragment)(contentBuilder =>
            {
                CreateScrollArea(type: "always")(contentBuilder);
            }));
            builder.CloseComponent();
        });

        var root = cut.FindComponent<BradixScrollArea>();
        await root.Instance.HandleViewportMetricsChangedAsync(0, 0, 500, 500, 100, 100);

        Assert.Equal("rtl", cut.Find(".root").GetAttribute("dir"));
    }

    private static RenderFragment CreateScrollArea(string type, bool includeHorizontal = false, bool includeCorner = false)
    {
        return builder =>
        {
            builder.OpenComponent<BradixScrollArea>(0);
            builder.AddAttribute(1, nameof(BradixScrollArea.Type), type);
            builder.AddAttribute(2, nameof(BradixScrollArea.Class), "root");
            builder.AddAttribute(3, nameof(BradixScrollArea.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixScrollAreaViewport>(0);
                contentBuilder.AddAttribute(1, nameof(BradixScrollAreaViewport.ChildContent), (RenderFragment)(viewportBuilder =>
                {
                    viewportBuilder.OpenElement(0, "div");
                    viewportBuilder.AddContent(1, "Content");
                    viewportBuilder.CloseElement();
                }));
                contentBuilder.CloseComponent();

                contentBuilder.OpenComponent<BradixScrollAreaScrollbar>(10);
                contentBuilder.AddAttribute(11, nameof(BradixScrollAreaScrollbar.Orientation), "vertical");
                contentBuilder.AddAttribute(12, nameof(BradixScrollAreaScrollbar.Class), "scrollbar");
                contentBuilder.AddAttribute(13, nameof(BradixScrollAreaScrollbar.ChildContent), (RenderFragment)(scrollbarBuilder =>
                {
                    scrollbarBuilder.OpenComponent<BradixScrollAreaThumb>(0);
                    scrollbarBuilder.AddAttribute(1, nameof(BradixScrollAreaThumb.Class), "thumb");
                    scrollbarBuilder.CloseComponent();
                }));
                contentBuilder.CloseComponent();

                if (includeHorizontal)
                {
                    contentBuilder.OpenComponent<BradixScrollAreaScrollbar>(20);
                    contentBuilder.AddAttribute(21, nameof(BradixScrollAreaScrollbar.Orientation), "horizontal");
                    contentBuilder.AddAttribute(22, nameof(BradixScrollAreaScrollbar.Class), "scrollbar");
                    contentBuilder.AddAttribute(23, nameof(BradixScrollAreaScrollbar.ChildContent), (RenderFragment)(scrollbarBuilder =>
                    {
                        scrollbarBuilder.OpenComponent<BradixScrollAreaThumb>(0);
                        scrollbarBuilder.AddAttribute(1, nameof(BradixScrollAreaThumb.Class), "thumb");
                        scrollbarBuilder.CloseComponent();
                    }));
                    contentBuilder.CloseComponent();
                }

                if (includeCorner)
                {
                    contentBuilder.OpenComponent<BradixScrollAreaCorner>(30);
                    contentBuilder.AddAttribute(31, nameof(BradixScrollAreaCorner.Class), "corner");
                    contentBuilder.CloseComponent();
                }
            }));
            builder.CloseComponent();
        };
    }
}
