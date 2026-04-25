using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Playwright;

namespace Soenneker.Bradix.Suite.Playwrights.Tests;

internal sealed record DemoPageSpec(string Route, string Title, string Heading, string Description, Func<IPage, ILocator> ReadyLocator);

internal static class DemoPageSpecs
{
    private static readonly IReadOnlyList<DemoPageSpec> _all =
    [
        new("/", "Overview", "Bradix primitives", "Simple demos for each primitive, modeled after the straightforward presentation style of the Radix docs.",
            page => page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Bradix primitives", Exact = true })),
        new("/accessible-icon", "AccessibleIcon", "AccessibleIcon",
            "Hide decorative glyphs from assistive technology while exposing a reliable accessible name.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Close panel", Exact = true })),
        new("/aspect-ratio", "AspectRatio", "AspectRatio", "Preserve media proportions with an unstyled wrapper that stays honest to layout constraints.",
            page => page.GetByAltText("Landscape photograph by Tobias Tullius")),
        new("/avatar", "Avatar", "Avatar", "Model image loading, fallback timing, and identity surfaces for profile and presence UI.",
            page => page.GetByAltText("Colm Tuite")),
        new("/collection", "Collection", "Collection", "Validate ordered item registration and typeahead behavior used by menu-like composites.",
            page => page.Locator("#typeahead-input")),
        new("/label", "Label", "Label", "Compose labels around controls without sacrificing native semantics or selection behavior.",
            page => page.Locator("#firstName")),
        new("/portal", "Portal", "Portal", "Reparent UI into `body` or a custom container for overlays and layered experiences.",
            page => page.GetByText("Portaled into body.", new PageGetByTextOptions { Exact = true })),
        new("/presence", "Presence", "Presence", "Keep content mounted long enough for enter and exit animations to complete cleanly.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Toggle presence", Exact = true })),
        new("/separator", "Separator", "Separator", "Render semantic or decorative dividers with correct orientation metadata.",
            page => page.GetByText("Radix Primitives", new PageGetByTextOptions { Exact = true })),
        new("/slot", "Slot", "Slot", "Merge attributes and event handlers for future `asChild`-style composition.", page => page.Locator("#slot-button")),
        new("/visually-hidden", "VisuallyHidden", "VisuallyHidden", "Expose screen-reader-only content without adding visible text to the layout.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save the file", Exact = true })),

        new("/accordion", "Accordion", "Accordion", "Coordinate single and multiple item disclosure with orientation-aware keyboard support.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Is it accessible?", Exact = true })),
        new("/alert-dialog", "AlertDialog", "AlertDialog", "Present destructive confirmations with modal semantics and protected dismissal.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Delete account", Exact = true })),
        new("/collapsible", "Collapsible", "Collapsible", "Toggle content visibility while preserving trigger, content, and measurement behavior.",
            page => page.GetByText("starred 3 repositories")),
        new("/dialog", "Dialog", "Dialog", "Build modal and non-modal dialogs with layering, restoration, and accessible naming.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit profile", Exact = true })),
        new("/hover-card", "HoverCard", "HoverCard", "Preview contextual information with delayed hover and focus interactions.",
            page => page.GetByAltText("Radix UI")),
        new("/popover", "Popover", "Popover", "Anchor rich content to a trigger with optional modal behavior and popper positioning.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Update dimensions", Exact = true })),
        new("/toast", "Toast", "Toast", "Queue ephemeral notifications with viewport focus management and swipe dismissal.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add to calendar", Exact = true })),
        new("/tooltip", "Tooltip", "Tooltip", "Show lightweight descriptions with shared provider timing and anchored positioning.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "+", Exact = true })),

        new("/checkbox", "Checkbox", "Checkbox", "Exercise tri-state selection, indicators, form resets, and mixed accessibility semantics.",
            page => page.GetByText("Accept terms and conditions.", new PageGetByTextOptions { Exact = true })),
        new("/form", "Form", "Form", "Compose fields around native constraint validation, custom matchers, and server-invalid handoff.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Post question", Exact = true })),
        new("/one-time-password-field", "One-Time Password Field", "One-Time Password Field",
            "Capture segmented one-time codes with paste distribution and keyboard-friendly behavior.", page => page.Locator(".otp-slot")
                .First),
        new("/progress", "Progress", "Progress", "Represent determinate and indeterminate progress with correct ARIA metadata.",
            page => page.Locator(".website-demo-page .card").First),
        new("/radio-group", "RadioGroup", "RadioGroup", "Model single-choice selection with roving focus and hidden input synchronization.",
            page => page.GetByText("Compact", new PageGetByTextOptions { Exact = true })),
        new("/select", "Select", "Select", "Compose trigger and listbox content with grouping, indicators, and form participation.", page => page
            .Locator("[role='combobox']")
            .First),
        new("/slider", "Slider", "Slider", "Handle single and multiple thumbs, direction, orientation, and keyboard geometry.",
            page => page.GetByRole(AriaRole.Slider, new PageGetByRoleOptions { Name = "Volume", Exact = true })),
        new("/switch", "Switch", "Switch", "Expose binary on-off state with switch semantics and form integration.",
            page => page.GetByText("Airplane mode", new PageGetByTextOptions { Exact = true })),
        new("/toggle", "Toggle", "Toggle", "Exercise pressed state and `aria-pressed` metadata for standalone toggles.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Toggle italic", Exact = true })),
        new("/toggle-group", "ToggleGroup", "ToggleGroup", "Coordinate single and multiple pressed items with roving focus and direction awareness.",
            page => page.GetByLabel("Text alignment")),

        new("/context-menu", "ContextMenu", "ContextMenu", "Open menu content from right-click and long-press gestures over a virtual anchor.",
            page => page.GetByText("Right-click here.", new PageGetByTextOptions { Exact = true })),
        new("/dropdown-menu", "DropdownMenu", "DropdownMenu", "Trigger layered menu content with checkbox, radio, and submenu composition.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Customise options", Exact = true })),
        new("/menubar", "Menubar", "Menubar", "Coordinate top-level application menus with cross-menu arrow navigation.",
            page => page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions { Name = "File", Exact = true })),
        new("/menu", "Menu", "Menu", "Validate the shared menu substrate with grouping, typeahead, and roving focus.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Open modal menu", Exact = true })),
        new("/navigation-menu-inline", "NavigationMenu Inline", "NavigationMenu Inline",
            "Exercise navigation menu switching without the shared viewport so inline content stays in the item subtree.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true })),
        new("/navigation-menu-minimal", "NavigationMenu Minimal", "NavigationMenu Minimal",
            "Exercise navigation menu switching with plain content only to isolate viewport and content-host behavior.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true })),
        new("/navigation-menu", "NavigationMenu", "NavigationMenu", "Build top navigation flyouts, viewport-backed panels, and nested submenu flows.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true })),
        new("/navigation-menu-uncontrolled", "NavigationMenu Uncontrolled", "NavigationMenu Uncontrolled",
            "Exercise the same top navigation flyout composition without external value binding to isolate primitive behavior.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Learn", Exact = true })),
        new("/scroll-area", "ScrollArea", "ScrollArea", "Replace default scrollbars with custom viewport, thumb, and corner primitives.",
            page => page.GetByText("Tags", new PageGetByTextOptions { Exact = true })),
        new("/tabs", "Tabs", "Tabs", "Compose tablists, triggers, and content with automatic or manual activation.",
            page => page.GetByLabel("Manage your account")),
        new("/toolbar", "Toolbar", "Toolbar", "Provide keyboard-friendly grouped controls with embedded toggle groups and separators.",
            page => page.GetByLabel("Formatting options")),

        new("/dismissable-layer", "DismissableLayer", "DismissableLayer", "Coordinate outside interaction and escape handling for layered content.",
            page => page.GetByText("Default dismissal", new PageGetByTextOptions { Exact = true })),
        new("/focus-guards", "FocusGuards", "FocusGuards", "Mount document-edge sentinels so portalled scopes can observe focus boundaries reliably.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Toggle guards", Exact = true })),
        new("/focus-scope", "FocusScope", "FocusScope", "Trap or loop focus inside managed regions used by layered primitives.",
            page => page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Looping scope", Exact = true })),
        new("/popper", "Popper", "Popper", "Position floating content relative to anchors with placement metadata and collision handling.",
            page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Anchor", Exact = true })),
        new("/remove-scroll", "RemoveScroll", "RemoveScroll", "Lock body scrolling while preserving intended interaction inside modal surfaces.",
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
