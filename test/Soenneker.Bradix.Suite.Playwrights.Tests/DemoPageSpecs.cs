using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Playwright;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

internal static class DemoPageSpecs
{
    private static readonly IReadOnlyList<DemoPageSpec> _all =
    [
        new("/", "Overview", "Bradix primitives", "A low-level Blazor component library for building accessible design systems and web apps.",
            page => page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Bradix primitives", Exact = true })),
        new("/accessibleicons", "AccessibleIcon", "AccessibleIcon",
            "Hide decorative glyphs from assistive technology while exposing a reliable accessible name.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Close panel", Exact = true })),
        new("/aspectratios", "AspectRatio", "AspectRatio", "Preserve media proportions with an unstyled wrapper that stays honest to layout constraints.",
            page => page.GetByAltText("Landscape photograph by Tobias Tullius")),
        new("/avatars", "Avatar", "Avatar", "Model image loading, fallback timing, and identity surfaces for profile and presence UI.",
            page => page.GetByAltText("Colm Tuite")),
        new("/collections", "Collection", "Collection", "Validate ordered item registration and typeahead behavior used by menu-like composites.",
            page => page.Locator("#typeahead-input")),
        new("/labels", "Label", "Label", "Compose labels around controls without sacrificing native semantics or selection behavior.",
            page => page.Locator("#firstName")),
        new("/portals", "Portal", "Portal", "Reparent UI into `body` or a custom container for overlays and layered experiences.",
            page => page.GetByText("Portaled into body.", new PageGetByTextOptions { Exact = true })),
        new("/presences", "Presence", "Presence", "Keep content mounted long enough for enter and exit animations to complete cleanly.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Toggle presence", Exact = true })),
        new("/separators", "Separator", "Separator", "Render semantic or decorative dividers with correct orientation metadata.",
            page => page.GetByText("Radix Primitives", new PageGetByTextOptions { Exact = true })),
        new("/slots", "Slot", "Slot", "Merge attributes and event handlers for future `asChild`-style composition.", page => page.Locator("#slot-button")),
        new("/visuallyhidden", "VisuallyHidden", "VisuallyHidden", "Expose screen-reader-only content without adding visible text to the layout.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save the file", Exact = true })),

        new("/accordions", "Accordion", "Accordion", "Coordinate single and multiple item disclosure with orientation-aware keyboard support.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Is it accessible?", Exact = true })),
        new("/alertdialogs", "AlertDialog", "AlertDialog", "Present destructive confirmations with modal semantics and protected dismissal.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Delete account", Exact = true })),
        new("/collapsibles", "Collapsible", "Collapsible", "Toggle content visibility while preserving trigger, content, and measurement behavior.",
            page => page.Locator(".component-example__preview").GetByText("starred 3 repositories")),
        new("/dialogs", "Dialog", "Dialog", "Build modal and non-modal dialogs with layering, restoration, and accessible naming.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true })),
        new("/hovercards", "HoverCard", "HoverCard", "Preview contextual information with delayed hover and focus interactions.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open hover card dialog", Exact = true })),
        new("/popovers", "Popover", "Popover", "Anchor rich content to a trigger with optional modal behavior and popper positioning.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Update dimensions", Exact = true })),
        new("/toasts", "Toast", "Toast", "Queue ephemeral notifications with viewport focus management and swipe dismissal.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add to calendar", Exact = true })),
        new("/tooltips", "Tooltip", "Tooltip", "Show lightweight descriptions with shared provider timing and anchored positioning.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "+", Exact = true })),

        new("/checkboxes", "Checkbox", "Checkbox", "Exercise tri-state selection, indicators, form resets, and mixed accessibility semantics.",
            page => page.GetByText("Accept terms and conditions.", new PageGetByTextOptions { Exact = true })),
        new("/forms", "Form", "Form", "Compose fields around native constraint validation, custom matchers, and server-invalid handoff.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Post question", Exact = true })),
        new("/onetimepasswordfields", "One-Time Password Field", "One-Time Password Field",
            "Capture segmented one-time codes with paste distribution and keyboard-friendly behavior.", page => page.Locator(".otp-slot")
                .First),
        new("/progresses", "Progress", "Progress", "Represent determinate and indeterminate progress with correct ARIA metadata.",
            page => page.Locator(".website-demo-page .card").First),
        new("/radiogroups", "RadioGroup", "RadioGroup", "Model single-choice selection with roving focus and hidden input synchronization.",
            page => page.GetByText("Compact", new PageGetByTextOptions { Exact = true })),
        new("/selects", "Select", "Select", "Compose trigger and listbox content with grouping, indicators, and form participation.", page => page
            .Locator("[role='combobox']")
            .First),
        new("/sliders", "Slider", "Slider", "Handle single and multiple thumbs, direction, orientation, and keyboard geometry.",
            page => page.GetByRole(AriaRole.Slider, new PageGetByRoleOptions { Name = "Volume", Exact = true })),
        new("/switches", "Switch", "Switch", "Expose binary on-off state with switch semantics and form integration.",
            page => page.GetByText("Airplane mode", new PageGetByTextOptions { Exact = true })),
        new("/toggles", "Toggle", "Toggle", "Exercise pressed state and `aria-pressed` metadata for standalone toggles.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Toggle italic", Exact = true })),
        new("/togglegroups", "ToggleGroup", "ToggleGroup", "Coordinate single and multiple pressed items with roving focus and direction awareness.",
            page => page.GetByLabel("Text alignment")),

        new("/contextmenus", "ContextMenu", "ContextMenu", "Open menu content from right-click and long-press gestures over a virtual anchor.",
            page => page.GetByText("Right-click here.", new PageGetByTextOptions { Exact = true })),
        new("/dropdownmenus", "DropdownMenu", "DropdownMenu", "Trigger layered menu content with checkbox, radio, and submenu composition.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Customise options", Exact = true })),
        new("/menubars", "Menubar", "Menubar", "Coordinate top-level application menus with cross-menu arrow navigation.",
            page => page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "File", Exact = true })),
        new("/menus", "Menu", "Menu", "Validate the shared menu substrate with grouping, typeahead, and roving focus.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open modal menu", Exact = true })),
        new("/navigationmenuinline", "NavigationMenu Inline", "NavigationMenu Inline",
            "Exercise navigation menu switching without the shared viewport so inline content stays in the item subtree.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true })),
        new("/navigationmenuminimal", "NavigationMenu Minimal", "NavigationMenu Minimal",
            "Exercise navigation menu switching with plain content only to isolate viewport and content-host behavior.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true })),
        new("/navigationmenus", "NavigationMenu", "NavigationMenu", "Build top navigation flyouts, viewport-backed panels, and nested submenu flows.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true })),
        new("/navigationmenuuncontrolled", "NavigationMenu Uncontrolled", "NavigationMenu Uncontrolled",
            "Exercise the same top navigation flyout composition without external value binding to isolate primitive behavior.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true })),
        new("/scrollareas", "ScrollArea", "ScrollArea", "Replace default scrollbars with custom viewport, thumb, and corner primitives.",
            page => page.GetByText("Tags", new PageGetByTextOptions { Exact = true })),
        new("/tabs", "Tabs", "Tabs", "Compose tablists, triggers, and content with automatic or manual activation.",
            page => page.GetByLabel("Manage your account")),
        new("/toolbars", "Toolbar", "Toolbar", "Provide keyboard-friendly grouped controls with embedded toggle groups and separators.",
            page => page.GetByLabel("Formatting options")),

        new("/dismissablelayers", "DismissableLayer", "DismissableLayer", "Coordinate outside interaction and escape handling for layered content.",
            page => page.Locator(".portal-surface")),
        new("/focusguards", "FocusGuards", "FocusGuards", "Mount document-edge sentinels so portalled scopes can observe focus boundaries reliably.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Toggle guards", Exact = true })),
        new("/focusscopes", "FocusScope", "FocusScope", "Trap or loop focus inside managed regions used by layered primitives.",
            page => page.Locator(".portal-surface")),
        new("/poppers", "Popper", "Popper", "Position floating content relative to anchors with placement metadata and collision handling.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Anchor", Exact = true })),
        new("/removescrolls", "RemoveScroll", "RemoveScroll", "Lock body scrolling while preserving intended interaction inside modal surfaces.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Toggle scroll lock", Exact = true }))
    ];

    public static IReadOnlyList<DemoPageSpec> All => _all;

    public static IEnumerable<object[]> AllRoutes()
    {
        return _all.Select(spec => new object[] { spec.Route });
    }

    public static DemoPageSpec Get(string route)
    {
        return _all.First(spec => string.Equals(spec.Route, route, StringComparison.OrdinalIgnoreCase));
    }
}
