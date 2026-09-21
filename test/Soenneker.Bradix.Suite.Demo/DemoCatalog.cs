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
        "Build custom Blazor interfaces with Bradix, an open-source library of accessible, unstyled UI components based on Radix UI. Explore demos and Razor examples.");

    public static readonly IReadOnlyList<DemoPageGroup> Groups = new ReadOnlyCollection<DemoPageGroup>(new[]
    {
        new DemoPageGroup("Foundations",
        "Low-level building blocks, accessibility utilities, and shared composition patterns that many primitives build on.",
        new ReadOnlyCollection<DemoPageLink>(new[]
        {
            new DemoPageLink("/accessibleicons", "Accessible Icon", "Foundations", "Give Blazor icons accessible text labels with Bradix AccessibleIcon. Explore a labeled close icon and the Razor markup for screen reader support."),
            new DemoPageLink("/aspectratios", "Aspect Ratio", "Foundations", "Keep Blazor images and media at a fixed aspect ratio. Compare landscape, square avatar, and portrait examples with custom styling and Bradix Razor code."),
            new DemoPageLink("/avatars", "Avatar", "Foundations", "Display user avatars in Blazor with profile images and fallback content. Try loaded images, delayed fallbacks, and missing-image states with Bradix."),
            new DemoPageLink("/collections", "Collection", "Foundations", "Build Blazor composite controls with an ordered item registry and typeahead search. Try reordering items and skipping disabled entries in the Bradix demo."),
            new DemoPageLink("/labels", "Label", "Foundations", "Associate accessible labels with Blazor text inputs and checkboxes. See Bradix Label examples using the For attribute to connect labels to form controls."),
            new DemoPageLink("/portals", "Portal", "Foundations", "Render Blazor content outside its original DOM location with Bradix Portal. Explore the portal building block used for dialogs, menus, and floating overlays."),
            new DemoPageLink("/presences", "Presence", "Foundations", "Coordinate Blazor component visibility with CSS exit animations. Try Bradix Presence, toggle mounted content, and observe the exit-complete callback."),
            new DemoPageLink("/separators", "Separator", "Foundations", "Separate Blazor content with horizontal and vertical dividers. Explore Bradix Separator for semantic or decorative separation with custom styling."),
            new DemoPageLink("/slots", "Slot", "Foundations", "Compose Blazor elements with Bradix Slot. See how element attributes, classes, styles, and child attributes combine in a customizable button example."),
            new DemoPageLink("/visuallyhidden", "Visually Hidden", "Foundations", "Add screen-reader text without displaying it visually in Blazor. Explore Bradix VisuallyHidden for accessible descriptions alongside icons and controls.")
        })),

        new DemoPageGroup("Disclosure And Overlays",
        "Dialogs, floating surfaces, and reveal patterns that need careful focus management, dismissal, and layering.",
        new ReadOnlyCollection<DemoPageLink>(new[]
        {
            new DemoPageLink("/accordions", "Accordion", "Disclosure And Overlays", "Build expandable Blazor accordion panels with single or multiple selection. Try disabled items, horizontal RTL layout, and force-mounted content in live demos."),
            new DemoPageLink("/alertdialogs", "Alert Dialog", "Disclosure And Overlays", "Create Blazor confirmation dialogs with action and cancel buttons. Explore delete confirmation and preventing Escape dismissal with Bradix Razor examples."),
            new DemoPageLink("/collapsibles", "Collapsible", "Disclosure And Overlays", "Create expandable Blazor content panels with Bradix Collapsible. Explore toggle controls, disabled panels, and force-mounted content with live Razor examples."),
            new DemoPageLink("/dialogs", "Dialog", "Disclosure And Overlays", "Build accessible modal dialogs in Blazor with overlays, titles, and close controls. Try an edit-profile form and nested popovers with Bradix Razor examples."),
            new DemoPageLink("/hovercards", "Hover Card", "Disclosure And Overlays", "Show rich Blazor link previews with Bradix HoverCard. Explore a profile preview and a hover card inside a dialog, with composable content and Razor examples."),
            new DemoPageLink("/popovers", "Popover", "Disclosure And Overlays", "Add anchored popovers to Blazor interfaces with custom content and close controls. Try form fields, controlled open state, and a custom listbox with Bradix."),
            new DemoPageLink("/toasts", "Toast", "Disclosure And Overlays", "Display Blazor toast notifications with a title, message, and action button. Explore controlled visibility and swipe dismissal with the Bradix Toast demo."),
            new DemoPageLink("/tooltips", "Tooltip", "Disclosure And Overlays", "Add contextual tooltips to Blazor controls with Bradix Tooltip. Explore opening delays, arrows, portal content, and tooltips inside modal dialogs.")
        })),

        new DemoPageGroup("Forms And Selection",
        "Input primitives that model state, validation, selection, and submission semantics in production-style workflows.",
        new ReadOnlyCollection<DemoPageLink>(new[]
        {
            new DemoPageLink("/checkboxes", "Checkbox", "Forms And Selection", "Add accessible Blazor checkboxes with checked, unchecked, and indeterminate states. Explore native form reset behavior and Bradix component code examples."),
            new DemoPageLink("/forms", "Form", "Forms And Selection", "Build Blazor forms with required fields, email validation, and accessible error messages. Explore Bradix Form labels, controls, and a complete submit example."),
            new DemoPageLink("/onetimepasswordfields", "One-Time Password Field", "Forms And Selection", "Build segmented OTP inputs in Blazor for verification codes. Try six-digit entry, controlled values, hidden form inputs, and form reset with Bradix."),
            new DemoPageLink("/progresses", "Progress", "Forms And Selection", "Show loading progress in Blazor with Bradix Progress. Compare a changing progress value, a completed bar, and an indeterminate state with Razor examples."),
            new DemoPageLink("/radiogroups", "Radio Group", "Forms And Selection", "Build single-choice Blazor radio groups with accessible labels. Try default, disabled, and controlled options for view density, support tiers, and billing."),
            new DemoPageLink("/selects", "Select", "Forms And Selection", "Build accessible Blazor select dropdowns with grouped options and placeholders. Try scroll boundaries, required form validation, and selects inside dialogs."),
            new DemoPageLink("/sliders", "Slider", "Forms And Selection", "Build Blazor range sliders with controlled values and multiple thumbs. Try minimum spacing, vertical layout, and RTL direction with Bradix Razor examples."),
            new DemoPageLink("/switches", "Switch", "Forms And Selection", "Add accessible on/off switches to Blazor forms with Bradix Switch. Compare default and controlled state, and try native form reset with live Razor examples."),
            new DemoPageLink("/toggles", "Toggle", "Forms And Selection", "Create two-state Blazor toggle buttons with Bradix Toggle. Compare uncontrolled, controlled, and disabled buttons for text formatting and notification settings."),
            new DemoPageLink("/togglegroups", "Toggle Group", "Forms And Selection", "Build Blazor toggle button groups for alignment and text formatting. Explore single or multiple selection, disabled items, vertical layout, and RTL support.")
        })),

        new DemoPageGroup("Navigation And Menus",
        "High-signal composites for app chrome, command surfaces, and structured navigation patterns.",
        new ReadOnlyCollection<DemoPageLink>(new[]
        {
            new DemoPageLink("/contextmenus", "Context Menu", "Navigation And Menus", "Create right-click context menus in Blazor with selectable items and submenus. Explore standalone menus and menus inside dialogs with Bradix Razor examples."),
            new DemoPageLink("/dropdownmenus", "Dropdown Menu", "Navigation And Menus", "Add Blazor dropdown menus with grouped actions, disabled items, and submenus. Explore nested menus inside dialogs and inspect the Bradix Razor source."),
            new DemoPageLink("/menubars", "Menubar", "Navigation And Menus", "Build desktop-style menus in Blazor with Bradix Menubar. Try controlled menu state, submenus, disabled actions, and RTL navigation with live Razor examples."),
            new DemoPageLink("/menus", "Menu", "Navigation And Menus", "Compare modal and non-modal menus in Blazor using Bradix Menu. Explore the shared menu building blocks and inspect the Razor code behind each example."),
            new DemoPageLink("/navigationmenuinline", "Navigation Menu Inline", "Navigation And Menus", "Render Blazor navigation panels inline without a shared viewport. Explore a compact Bradix menu with link groups and content attached to each trigger."),
            new DemoPageLink("/navigationmenuminimal", "Navigation Menu Minimal", "Navigation And Menus", "Start with a minimal Blazor navigation menu: two triggers, content panels, and a shared viewport. See the essential Bradix components in a small Razor example."),
            new DemoPageLink("/navigationmenus", "Navigation Menu", "Navigation And Menus", "Build Blazor site navigation with dropdown content, active links, and a shared viewport. Explore the Bradix NavigationMenu demo and its composable Razor parts."),
            new DemoPageLink("/navigationmenuuncontrolled", "Navigation Menu Uncontrolled", "Navigation And Menus", "Let Bradix manage navigation-menu open state while your Blazor app tracks active links. Explore selection callbacks, an indicator, and a shared content viewport."),
            new DemoPageLink("/scrollareas", "Scroll Area", "Navigation And Menus", "Create custom scrollable regions in Blazor with Bradix ScrollArea. Explore vertical content, horizontal release lists, and right-to-left scrolling examples."),
            new DemoPageLink("/tabs", "Tabs", "Navigation And Menus", "Build tabbed Blazor interfaces with Bradix Tabs. Explore disabled and controlled tabs, manual keyboard activation, vertical orientation, and RTL layout."),
            new DemoPageLink("/toolbars", "Toolbar", "Navigation And Menus", "Group Blazor formatting controls with Bradix Toolbar. Explore text-style toggle buttons, action groups, and separators in a customizable Razor toolbar.")
        })),

        new DemoPageGroup("Infrastructure",
        "The invisible substrate that makes polished overlay and focus behavior reliable across the suite.",
        new ReadOnlyCollection<DemoPageLink>(new[]
        {
            new DemoPageLink("/dismissablelayers", "Dismissable Layer", "Infrastructure", "Manage dismissable content in Blazor with Bradix DismissableLayer. Explore outside-interaction dismissal and C# callbacks for custom overlay components."),
            new DemoPageLink("/focusguards", "Focus Guards", "Infrastructure", "Use Bradix FocusGuards to add focus sentinels around Blazor content. Explore the keyboard-navigation building block used by overlay and portal components."),
            new DemoPageLink("/focusscopes", "Focus Scope", "Infrastructure", "Manage keyboard focus in Blazor with Bradix FocusScope. Compare looping Tab navigation with trapped focus for dialogs and other contained interactions."),
            new DemoPageLink("/poppers", "Popper", "Infrastructure", "Position floating Blazor content with Bradix Popper. Explore anchors, arrows, offsets, and collision boundaries for custom tooltips, menus, and popovers."),
            new DemoPageLink("/removescrolls", "RemoveScroll", "Infrastructure", "Lock background scrolling in Blazor while keeping overlay content scrollable. Explore Bradix RemoveScroll with pinch-zoom support and scrollable form content.")
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
        int index = FindIndex(normalized);
        return index > 0 ? AllPages[index - 1] : null;
    }

    public static DemoPageLink? Next(string? route)
    {
        string normalized = NormalizeRoute(route);
        int index = FindIndex(normalized);
        return index >= 1 && index < AllPages.Count - 1 ? AllPages[index + 1] : null;
    }

    private static int FindIndex(string route)
    {
        for (var i = 0; i < AllPages.Count; i++)
            if (string.Equals(AllPages[i].Route, route, StringComparison.OrdinalIgnoreCase))
                return i;
        return -1;
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

        int suffix = route.AsSpan().IndexOfAny('?', '#');
        string normalized = suffix < 0 ? route : route[..suffix];

        if (!normalized.StartsWith("/", StringComparison.Ordinal))
            normalized = "/" + normalized;

        if (normalized.Length > 1)
            normalized = normalized.TrimEnd('/');

        return normalized;
    }
}
