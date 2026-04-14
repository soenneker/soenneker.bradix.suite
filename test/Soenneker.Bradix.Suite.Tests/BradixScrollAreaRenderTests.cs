using System;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Soenneker.Bradix.Suite.Tests;

public sealed class BradixScrollAreaRenderTests : BunitContext
{
    public BradixScrollAreaRenderTests()
    {
        BunitJSModuleInterop module = JSInterop.SetupModule("./_content/Soenneker.Bradix.Suite/js/bradix.js");
        module.SetupVoid("registerScrollAreaRoot", _ => true);
        module.SetupVoid("unregisterScrollAreaRoot", _ => true);
        module.SetupVoid("registerScrollAreaViewport", _ => true);
        module.SetupVoid("unregisterScrollAreaViewport", _ => true);
        module.SetupVoid("registerScrollAreaScrollbar", _ => true);
        module.SetupVoid("unregisterScrollAreaScrollbar", _ => true);

        Services.AddScoped<BradixSuiteInterop>();
        Services.AddScoped<IBradixSuiteInterop>(sp => sp.GetRequiredService<BradixSuiteInterop>());
    }

    [Test]
    public async Task Always_scroll_area_renders_scrollbar_parts()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateScrollArea(type: BradixScrollAreaType.Always, includeHorizontal: true));

        await Assert.That(cut.FindAll("[data-orientation]").Count).IsEqualTo(2);
        await Assert.That(cut.Markup).DoesNotContain("data-bradix-scroll-area-thumb");
    }

    [Test]
    public async Task Auto_scroll_area_mounts_scrollbar_when_overflow_detected()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateScrollArea(type: BradixScrollAreaType.Auto));
        IRenderedComponent<BradixScrollArea> root = cut.FindComponent<BradixScrollArea>();

        await Assert.That(cut.FindAll("[data-state='visible']")).IsEmpty();

        await root.Instance.HandleViewportMetricsChanged(0, 0, 500, 500, 100, 100);

        await Assert.That(cut.FindAll("[data-state='visible']")).IsNotEmpty();
    }

    [Test]
    public async Task Auto_scroll_area_keeps_viewport_overflow_hidden_until_scrollbar_is_rendered()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateScrollArea(type: BradixScrollAreaType.Auto));

        IElement viewport = cut.Find("[data-radix-scroll-area-viewport]");

        await Assert.That(viewport.GetAttribute("style")).Contains("overflow-x: hidden");
        await Assert.That(viewport.GetAttribute("style")).Contains("overflow-y: hidden");
    }

    [Test]
    public async Task Hover_scroll_area_shows_scrollbar_on_hover()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateScrollArea(type: BradixScrollAreaType.Hover));
        IRenderedComponent<BradixScrollArea> root = cut.FindComponent<BradixScrollArea>();

        await root.Instance.HandleViewportMetricsChanged(0, 0, 500, 500, 100, 100);
        await Assert.That(cut.FindAll("[data-state='visible']")).IsEmpty();

        await root.Instance.HandleHoverChanged(true);
        await Assert.That(cut.FindAll("[data-state='visible']")).IsNotEmpty();
    }

    [Test]
    public async Task Hover_scroll_area_hides_after_scroll_hide_delay_on_pointer_leave()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateScrollArea(type: BradixScrollAreaType.Hover, scrollHideDelay: 10));
        IRenderedComponent<BradixScrollArea> root = cut.FindComponent<BradixScrollArea>();

        await root.Instance.HandleViewportMetricsChanged(0, 0, 500, 500, 100, 100);
        await root.Instance.HandleHoverChanged(true);

        await Assert.That(cut.FindAll("[data-state='visible']")).IsNotEmpty();

        await root.Instance.HandleHoverChanged(false);
        await Assert.That(cut.FindAll("[data-state='visible']")).IsNotEmpty();

        await Task.Delay(40, CancellationToken.None);

        await Assert.That(cut.FindAll("[data-state='visible']")).IsEmpty();
    }

    [Test]
    public async Task Scroll_type_hides_after_scroll_end_and_scroll_hide_delay()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateScrollArea(type: BradixScrollAreaType.Scroll, scrollHideDelay: 20));
        IRenderedComponent<BradixScrollArea> root = cut.FindComponent<BradixScrollArea>();

        await root.Instance.HandleViewportMetricsChanged(0, 40, 500, 500, 100, 100);

        await Assert.That(cut.FindAll("[data-state='visible']")).IsNotEmpty();

        await Task.Delay(160, CancellationToken.None);

        await cut.WaitForAssertionAsync(async () =>
        {
            await Assert.That(cut.FindAll("[data-state='visible']")).IsEmpty();
        }, TimeSpan.FromSeconds(2));
    }

    [Test]
    public async Task Scroll_type_keeps_scrollbar_visible_while_pointer_is_over_it()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateScrollArea(type: BradixScrollAreaType.Scroll, scrollHideDelay: 20));
        IRenderedComponent<BradixScrollArea> root = cut.FindComponent<BradixScrollArea>();

        await root.Instance.HandleViewportMetricsChanged(0, 40, 500, 500, 100, 100);
        await Assert.That(cut.FindAll("[data-state='visible']")).IsNotEmpty();

        await root.Instance.HandleScrollbarPointerChanged("vertical", true);

        await Task.Delay(160, CancellationToken.None);

        await Assert.That(cut.FindAll("[data-state='visible']")).IsNotEmpty();

        await root.Instance.HandleScrollbarPointerChanged("vertical", false);
        await Task.Delay(40, CancellationToken.None);

        await Assert.That(cut.FindAll("[data-state='visible']")).IsEmpty();
    }

    [Test]
    public async Task Corner_renders_when_both_scrollbars_visible()
    {
        IRenderedComponent<ContainerFragment> cut = Render(CreateScrollArea(type: BradixScrollAreaType.Always, includeHorizontal: true, includeCorner: true));
        IRenderedComponent<BradixScrollArea> root = cut.FindComponent<BradixScrollArea>();

        await root.Instance.HandleViewportMetricsChanged(0, 0, 500, 500, 100, 100);
        await root.Instance.HandleScrollbarMetricsChanged("horizontal", 100, 12, 0, 0);
        await root.Instance.HandleScrollbarMetricsChanged("vertical", 12, 100, 0, 0);

        await Assert.That(cut.FindAll(".corner")).HasSingleItem();
    }

    [Test]
    public async Task Inherited_direction_sets_root_dir()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixDirectionProvider>(0);
            builder.AddAttribute(1, nameof(BradixDirectionProvider.Dir), "rtl");
            builder.AddAttribute(2, nameof(BradixDirectionProvider.ChildContent), (RenderFragment)(contentBuilder =>
            {
                CreateScrollArea(type: BradixScrollAreaType.Always)(contentBuilder);
            }));
            builder.CloseComponent();
        });

        IRenderedComponent<BradixScrollArea> root = cut.FindComponent<BradixScrollArea>();
        await root.Instance.HandleViewportMetricsChanged(0, 0, 500, 500, 100, 100);

        await Assert.That(cut.Find(".root").GetAttribute("dir")).IsEqualTo("rtl");
    }

    [Test]
    public async Task Viewport_forwards_nonce_to_injected_style_tag()
    {
        IRenderedComponent<ContainerFragment> cut = Render(builder =>
        {
            builder.OpenComponent<BradixScrollArea>(0);
            builder.AddAttribute(1, nameof(BradixScrollArea.ChildContent), (RenderFragment)(contentBuilder =>
            {
                contentBuilder.OpenComponent<BradixScrollAreaViewport>(0);
                contentBuilder.AddAttribute(1, nameof(BradixScrollAreaViewport.Nonce), "csp-nonce");
                contentBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        await Assert.That(cut.Find("style").GetAttribute("nonce")).IsEqualTo("csp-nonce");
    }

    private static RenderFragment CreateScrollArea(BradixScrollAreaType type, bool includeHorizontal = false, bool includeCorner = false, int? scrollHideDelay = null)
    {
        return builder =>
        {
            builder.OpenComponent<BradixScrollArea>(0);
            builder.AddAttribute(1, nameof(BradixScrollArea.Type), (object) type);
            builder.AddAttribute(2, nameof(BradixScrollArea.Class), "root");
            if (scrollHideDelay.HasValue)
                builder.AddAttribute(4, nameof(BradixScrollArea.ScrollHideDelay), scrollHideDelay.Value);
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
                contentBuilder.AddAttribute(11, nameof(BradixScrollAreaScrollbar.Orientation), (object) BradixOrientation.Vertical);
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
                    contentBuilder.AddAttribute(21, nameof(BradixScrollAreaScrollbar.Orientation), (object) BradixOrientation.Horizontal);
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