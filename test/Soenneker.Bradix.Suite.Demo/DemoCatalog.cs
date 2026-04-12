using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Soenneker.Bradix.Demo;

public sealed record DemoPageLink(string Route, string Title, string Category, string Description)
{
    public string Href => Route == "/" ? string.Empty : Route.TrimStart('/');
    public string Slug => Route == "/" ? "overview" : Route.Trim('/').Replace('-', ' ');
}

public sealed record DemoPageGroup(string Title, string Description, IReadOnlyList<DemoPageLink> Pages);

public static class DemoCatalog
{
    public static readonly DemoPageLink Overview = new("/",
        "Overview",
        "Overview",
        "Explore the full Bradix suite through polished, behavior-first demos organized like a real component library.");

    public static readonly IReadOnlyList<DemoPageGroup> Groups = new ReadOnlyCollection<DemoPageGroup>(new[]
    {
        new DemoPageGroup("Foundations",
        "Low-level building blocks, accessibility utilities, and shared composition patterns that many primitives build on.",
        new ReadOnlyCollection<DemoPageLink>(new[]
        {
            new DemoPageLink("/accessible-icon", "AccessibleIcon", "Foundations", "Hide decorative glyphs from assistive technology while exposing a reliable accessible name."),
            new DemoPageLink("/aspect-ratio", "AspectRatio", "Foundations", "Preserve media proportions with an unstyled wrapper that stays honest to layout constraints."),
            new DemoPageLink("/avatar", "Avatar", "Foundations", "Model image loading, fallback timing, and identity surfaces for profile and presence UI."),
            new DemoPageLink("/collection", "Collection", "Foundations", "Validate ordered item registration and typeahead behavior used by menu-like composites."),
            new DemoPageLink("/label", "Label", "Foundations", "Compose labels around controls without sacrificing native semantics or selection behavior."),
            new DemoPageLink("/portal", "Portal", "Foundations", "Reparent UI into `body` or a custom container for overlays and layered experiences."),
            new DemoPageLink("/presence", "Presence", "Foundations", "Keep content mounted long enough for enter and exit animations to complete cleanly."),
            new DemoPageLink("/separator", "Separator", "Foundations", "Render semantic or decorative dividers with correct orientation metadata."),
            new DemoPageLink("/slot", "Slot", "Foundations", "Merge attributes and event handlers for future `asChild`-style composition."),
            new DemoPageLink("/visually-hidden", "VisuallyHidden", "Foundations", "Expose screen-reader-only content without adding visible text to the layout.")
        })),

        new DemoPageGroup("Disclosure And Overlays",
        "Dialogs, floating surfaces, and reveal patterns that need careful focus management, dismissal, and layering.",
        new ReadOnlyCollection<DemoPageLink>(new[]
        {
            new DemoPageLink("/accordion", "Accordion", "Disclosure And Overlays", "Coordinate single and multiple item disclosure with orientation-aware keyboard support."),
            new DemoPageLink("/alert-dialog", "AlertDialog", "Disclosure And Overlays", "Present destructive confirmations with modal semantics and protected dismissal."),
            new DemoPageLink("/collapsible", "Collapsible", "Disclosure And Overlays", "Toggle content visibility while preserving trigger, content, and measurement behavior."),
            new DemoPageLink("/dialog", "Dialog", "Disclosure And Overlays", "Build modal and non-modal dialogs with layering, restoration, and accessible naming."),
            new DemoPageLink("/hover-card", "HoverCard", "Disclosure And Overlays", "Preview contextual information with delayed hover and focus interactions."),
            new DemoPageLink("/popover", "Popover", "Disclosure And Overlays", "Anchor rich content to a trigger with optional modal behavior and popper positioning."),
            new DemoPageLink("/toast", "Toast", "Disclosure And Overlays", "Queue ephemeral notifications with viewport focus management and swipe dismissal."),
            new DemoPageLink("/tooltip", "Tooltip", "Disclosure And Overlays", "Show lightweight descriptions with shared provider timing and anchored positioning.")
        })),

        new DemoPageGroup("Forms And Selection",
        "Input primitives that model state, validation, selection, and submission semantics in production-style workflows.",
        new ReadOnlyCollection<DemoPageLink>(new[]
        {
            new DemoPageLink("/checkbox", "Checkbox", "Forms And Selection", "Exercise tri-state selection, indicators, form resets, and mixed accessibility semantics."),
            new DemoPageLink("/form", "Form", "Forms And Selection", "Compose fields around native constraint validation, custom matchers, and server-invalid handoff."),
            new DemoPageLink("/one-time-password-field", "One-Time Password Field", "Forms And Selection", "Capture segmented one-time codes with paste distribution and keyboard-friendly behavior."),
            new DemoPageLink("/progress", "Progress", "Forms And Selection", "Represent determinate and indeterminate progress with correct ARIA metadata."),
            new DemoPageLink("/radio-group", "RadioGroup", "Forms And Selection", "Model single-choice selection with roving focus and hidden input synchronization."),
            new DemoPageLink("/select", "Select", "Forms And Selection", "Compose trigger and listbox content with grouping, indicators, and form participation."),
            new DemoPageLink("/slider", "Slider", "Forms And Selection", "Handle single and multiple thumbs, direction, orientation, and keyboard geometry."),
            new DemoPageLink("/switch", "Switch", "Forms And Selection", "Expose binary on-off state with switch semantics and form integration."),
            new DemoPageLink("/toggle", "Toggle", "Forms And Selection", "Exercise pressed state and `aria-pressed` metadata for standalone toggles."),
            new DemoPageLink("/toggle-group", "ToggleGroup", "Forms And Selection", "Coordinate single and multiple pressed items with roving focus and direction awareness.")
        })),

        new DemoPageGroup("Navigation And Menus",
        "High-signal composites for app chrome, command surfaces, and structured navigation patterns.",
        new ReadOnlyCollection<DemoPageLink>(new[]
        {
            new DemoPageLink("/context-menu", "ContextMenu", "Navigation And Menus", "Open menu content from right-click and long-press gestures over a virtual anchor."),
            new DemoPageLink("/dropdown-menu", "DropdownMenu", "Navigation And Menus", "Trigger layered menu content with checkbox, radio, and submenu composition."),
            new DemoPageLink("/menubar", "Menubar", "Navigation And Menus", "Coordinate top-level application menus with cross-menu arrow navigation."),
            new DemoPageLink("/menu", "Menu", "Navigation And Menus", "Validate the shared menu substrate with grouping, typeahead, and roving focus."),
            new DemoPageLink("/navigation-menu", "NavigationMenu", "Navigation And Menus", "Build top navigation flyouts, viewport-backed panels, and nested submenu flows."),
            new DemoPageLink("/scroll-area", "ScrollArea", "Navigation And Menus", "Replace default scrollbars with custom viewport, thumb, and corner primitives."),
            new DemoPageLink("/tabs", "Tabs", "Navigation And Menus", "Compose tablists, triggers, and content with automatic or manual activation."),
            new DemoPageLink("/toolbar", "Toolbar", "Navigation And Menus", "Provide keyboard-friendly grouped controls with embedded toggle groups and separators.")
        })),

        new DemoPageGroup("Infrastructure",
        "The invisible substrate that makes polished overlay and focus behavior reliable across the suite.",
        new ReadOnlyCollection<DemoPageLink>(new[]
        {
            new DemoPageLink("/dismissable-layer", "DismissableLayer", "Infrastructure", "Coordinate outside interaction and escape handling for layered content."),
            new DemoPageLink("/focus-guards", "FocusGuards", "Infrastructure", "Mount document-edge sentinels so portalled scopes can observe focus boundaries reliably."),
            new DemoPageLink("/focus-scope", "FocusScope", "Infrastructure", "Trap or loop focus inside managed regions used by layered primitives."),
            new DemoPageLink("/popper", "Popper", "Infrastructure", "Position floating content relative to anchors with placement metadata and collision handling."),
            new DemoPageLink("/remove-scroll", "RemoveScroll", "Infrastructure", "Lock body scrolling while preserving intended interaction inside modal surfaces.")
        }))
    });

    public static readonly IReadOnlyList<DemoPageLink> AllPages = new ReadOnlyCollection<DemoPageLink>(
        new[] { Overview }.Concat(Groups.SelectMany(group => group.Pages)).ToArray());

    public static int ComponentCount => AllPages.Count - 1;

    public static DemoPageLink? Find(string? route)
    {
        string normalized = NormalizeRoute(route);
        return AllPages.FirstOrDefault(page => string.Equals(page.Route, normalized, StringComparison.OrdinalIgnoreCase));
    }

    public static DemoPageLink? Previous(string? route)
    {
        string normalized = NormalizeRoute(route);
        int index = AllPages.ToList().FindIndex(page => string.Equals(page.Route, normalized, StringComparison.OrdinalIgnoreCase));
        return index > 0 ? AllPages[index - 1] : null;
    }

    public static DemoPageLink? Next(string? route)
    {
        string normalized = NormalizeRoute(route);
        int index = AllPages.ToList().FindIndex(page => string.Equals(page.Route, normalized, StringComparison.OrdinalIgnoreCase));
        return index >= 1 && index < AllPages.Count - 1 ? AllPages[index + 1] : null;
    }

    public static IReadOnlyList<DemoPageGroup> Filtered(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Groups;

        string search = query.Trim();
        List<DemoPageGroup> matches = new();

        foreach (DemoPageGroup group in Groups)
        {
            DemoPageLink[] pages = group.Pages
                .Where(page => page.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                               page.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                               page.Category.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                               page.Route.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (pages.Length > 0)
                matches.Add(group with { Pages = new ReadOnlyCollection<DemoPageLink>(pages) });
        }

        return matches;
    }

    public static string NormalizeRoute(string? route)
    {
        if (string.IsNullOrWhiteSpace(route))
            return "/";

        string normalized = route.Split('?', '#')[0];

        if (!normalized.StartsWith("/", StringComparison.Ordinal))
            normalized = "/" + normalized;

        if (normalized.Length > 1)
            normalized = normalized.TrimEnd('/');

        return normalized;
    }
}
