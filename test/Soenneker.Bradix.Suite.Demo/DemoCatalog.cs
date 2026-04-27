using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Soenneker.Bradix.Suite.Demo;

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
            new DemoPageLink("/accessible-icon", "Accessible Icon", "Foundations", "A utility that hides decorative content from screen readers while exposing a label."),
            new DemoPageLink("/aspect-ratio", "Aspect Ratio", "Foundations", "Displays content within a desired ratio."),
            new DemoPageLink("/avatar", "Avatar", "Foundations", "An image element with a fallback for representing the user."),
            new DemoPageLink("/collection", "Collection", "Foundations", "Utilities for collecting and ordering items in composite components."),
            new DemoPageLink("/label", "Label", "Foundations", "Renders an accessible label associated with controls."),
            new DemoPageLink("/portal", "Portal", "Foundations", "Portals content into a container outside the source tree."),
            new DemoPageLink("/presence", "Presence", "Foundations", "Mounts and unmounts content while preserving animation control."),
            new DemoPageLink("/separator", "Separator", "Foundations", "Visually or semantically separates content."),
            new DemoPageLink("/slot", "Slot", "Foundations", "Merges its props onto its immediate child."),
            new DemoPageLink("/visually-hidden", "Visually Hidden", "Foundations", "Hides content visually while keeping it available to screen readers.")
        })),

        new DemoPageGroup("Disclosure And Overlays",
        "Dialogs, floating surfaces, and reveal patterns that need careful focus management, dismissal, and layering.",
        new ReadOnlyCollection<DemoPageLink>(new[]
        {
            new DemoPageLink("/accordion", "Accordion", "Disclosure And Overlays", "A vertically stacked set of interactive headings that each reveal an associated section of content."),
            new DemoPageLink("/alert-dialog", "Alert Dialog", "Disclosure And Overlays", "A modal dialog that interrupts the user with important content and expects a response."),
            new DemoPageLink("/collapsible", "Collapsible", "Disclosure And Overlays", "An interactive component which expands and collapses a panel."),
            new DemoPageLink("/dialog", "Dialog", "Disclosure And Overlays", "A window overlaid on either the primary window or another dialog window."),
            new DemoPageLink("/hover-card", "Hover Card", "Disclosure And Overlays", "For sighted users to preview content available behind a link."),
            new DemoPageLink("/popover", "Popover", "Disclosure And Overlays", "Displays rich content in a portal, triggered by a button."),
            new DemoPageLink("/toast", "Toast", "Disclosure And Overlays", "A succinct message that is displayed temporarily."),
            new DemoPageLink("/tooltip", "Tooltip", "Disclosure And Overlays", "A popup that displays information related to an element when it receives keyboard focus or hover.")
        })),

        new DemoPageGroup("Forms And Selection",
        "Input primitives that model state, validation, selection, and submission semantics in production-style workflows.",
        new ReadOnlyCollection<DemoPageLink>(new[]
        {
            new DemoPageLink("/checkbox", "Checkbox", "Forms And Selection", "A control that allows the user to toggle between checked and not checked."),
            new DemoPageLink("/form", "Form", "Forms And Selection", "Compose fields around native constraint validation, custom matchers, and server-invalid handoff."),
            new DemoPageLink("/one-time-password-field", "One-Time Password Field", "Forms And Selection", "A set of inputs for capturing one-time password codes."),
            new DemoPageLink("/progress", "Progress", "Forms And Selection", "Displays an indicator showing the completion progress of a task."),
            new DemoPageLink("/radio-group", "Radio Group", "Forms And Selection", "A set of checkable buttons where only one can be checked at a time."),
            new DemoPageLink("/select", "Select", "Forms And Selection", "Displays a list of options for the user to pick from."),
            new DemoPageLink("/slider", "Slider", "Forms And Selection", "An input where the user selects a value from within a given range."),
            new DemoPageLink("/switch", "Switch", "Forms And Selection", "A control that allows the user to toggle between checked and not checked."),
            new DemoPageLink("/toggle", "Toggle", "Forms And Selection", "A two-state button that can be either on or off."),
            new DemoPageLink("/toggle-group", "Toggle Group", "Forms And Selection", "A set of two-state buttons that can be toggled on or off.")
        })),

        new DemoPageGroup("Navigation And Menus",
        "High-signal composites for app chrome, command surfaces, and structured navigation patterns.",
        new ReadOnlyCollection<DemoPageLink>(new[]
        {
            new DemoPageLink("/context-menu", "Context Menu", "Navigation And Menus", "Displays a menu located at the pointer, triggered by right click or long press."),
            new DemoPageLink("/dropdown-menu", "Dropdown Menu", "Navigation And Menus", "Displays a menu to the user, triggered by a button."),
            new DemoPageLink("/menubar", "Menubar", "Navigation And Menus", "A visually persistent menu common in desktop applications."),
            new DemoPageLink("/menu", "Menu", "Navigation And Menus", "Validate the shared menu substrate with grouping, typeahead, and roving focus."),
            new DemoPageLink("/navigation-menu-inline", "Navigation Menu Inline", "Navigation And Menus", "A collection of links for navigating websites."),
            new DemoPageLink("/navigation-menu-minimal", "Navigation Menu Minimal", "Navigation And Menus", "A collection of links for navigating websites."),
            new DemoPageLink("/navigation-menu", "Navigation Menu", "Navigation And Menus", "A collection of links for navigating websites."),
            new DemoPageLink("/navigation-menu-uncontrolled", "Navigation Menu Uncontrolled", "Navigation And Menus", "A collection of links for navigating websites."),
            new DemoPageLink("/scroll-area", "Scroll Area", "Navigation And Menus", "Augments native scroll functionality for custom, cross-browser styling."),
            new DemoPageLink("/tabs", "Tabs", "Navigation And Menus", "A set of layered sections of content, known as tab panels."),
            new DemoPageLink("/toolbar", "Toolbar", "Navigation And Menus", "A container for grouping a set of controls.")
        })),

        new DemoPageGroup("Infrastructure",
        "The invisible substrate that makes polished overlay and focus behavior reliable across the suite.",
        new ReadOnlyCollection<DemoPageLink>(new[]
        {
            new DemoPageLink("/dismissable-layer", "Dismissable Layer", "Infrastructure", "A layer that can be dismissed by pointer or focus interactions outside it."),
            new DemoPageLink("/focus-guards", "Focus Guards", "Infrastructure", "Sentinel elements used to keep focus behavior reliable around portals."),
            new DemoPageLink("/focus-scope", "Focus Scope", "Infrastructure", "Manages focus containment, looping, and restoration within a subtree."),
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
