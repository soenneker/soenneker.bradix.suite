[![](https://img.shields.io/nuget/v/soenneker.bradix.suite.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.bradix.suite/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.bradix.suite/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.bradix.suite/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.bradix.suite.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.bradix.suite/)
[![](https://img.shields.io/badge/Demo-Live-blueviolet?style=for-the-badge&logo=github)](https://soenneker.github.io/soenneker.bradix.suite)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.bradix.suite/codeql.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.bradix.suite/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Bradix.Suite

**Radix-inspired UI primitives for Blazor.**

`Soenneker.Bradix.Suite` is the behavioral foundation for building serious Blazor UI. It gives product teams a Radix-style primitive layer: dialogs, menus, popovers, selects, tabs, tooltips, scroll areas, form primitives, focus management, layered interactions, and the browser behavior that usually becomes scattered application code.

Bradix is intentionally **unstyled**. It handles structure, state, accessibility-minded interaction patterns, and JavaScript interop while leaving the visual system to your app, design system, or a higher-level component library.

Use Bradix when you want:

- primitives instead of opinionated components
- accessible interaction patterns without rewriting browser edge cases
- full control over styling, tokens, markup, and design-system wrappers
- reusable behavior that can support many product components
- one Blazor package instead of a mix of hand-rolled overlay, focus, and menu logic

Bradix is not a theme and not a high-level application component kit. It is the layer you build on when consistency, accessibility, and long-term maintainability matter.

## Why Bradix

Most UI libraries start with finished components. Bradix starts one layer lower, where the hard behavior lives.

- **Composable parts**: build with root, trigger, content, item, viewport, overlay, portal, and indicator pieces instead of monolithic controls.
- **Design-system freedom**: apply your own CSS, Tailwind utilities, tokens, or component wrappers without fighting built-in styles.
- **Hard behavior included**: focus scopes, dismissable layers, portals, positioning, roving focus, keyboard interaction, scroll locking, and form participation are handled by the suite.
- **Blazor-native setup**: install one NuGet package, register services, and use the primitives directly in Razor.
- **Broad primitive coverage**: overlays, disclosure, navigation, menus, forms, input controls, and infrastructure primitives live together in one suite.

## What Ships Today

Bradix ships as a single package with the primitives commonly needed to build polished application UI.

### Core utilities

`AccessibleIcon`, `AspectRatio`, `Avatar`, `Label`, `Portal`, `Presence`, `Separator`, `Slot`, `VisuallyHidden`

### Disclosure and overlays

`Accordion`, `AlertDialog`, `Collapsible`, `Dialog`, `HoverCard`, `Popover`, `Toast`, `Tooltip`

### Forms and input

`Checkbox`, `Form`, `OneTimePasswordField`, `Progress`, `RadioGroup`, `Select`, `Slider`, `Switch`, `Toggle`, `ToggleGroup`

### Navigation and menus

`ContextMenu`, `DropdownMenu`, `Menubar`, `Menu`, `NavigationMenu`, `ScrollArea`, `Tabs`, `Toolbar`

### Infrastructure primitives

`Collection`, `DismissableLayer`, `FocusGuards`, `FocusScope`, `Popper`, `RemoveScroll`

See the primitives in context:

[Open the demo site](https://soenneker.github.io/soenneker.bradix.suite)

## Installation

```bash
dotnet add package Soenneker.Bradix.Suite
```

Register Bradix in your Blazor app:

```csharp
using Soenneker.Bradix;

builder.Services.AddBradixSuiteAsScoped();
```

Import the namespace once:

```razor
@using Soenneker.Bradix
```

That is the only required setup on the .NET side.

You do **not** need to install a separate npm package or manually wire script tags. The suite ships its own browser module as part of the package.

## Quick Start

Bradix uses a composition model. Instead of a monolithic `DialogComponent`, you compose a dialog out of focused primitives with clear responsibilities.

```razor
@page "/example"

<BradixDialog Open="@_open" OpenChanged="HandleOpenChanged">
    <BradixDialogTrigger Class="btn btn-primary">
        Edit profile
    </BradixDialogTrigger>

    <BradixDialogPortal>
        <BradixDialogOverlay Class="dialog-overlay" />

        <BradixDialogContent Class="dialog-content">
            <BradixDialogTitle>Edit profile</BradixDialogTitle>
            <BradixDialogDescription>
                Make changes to your profile and save when you are done.
            </BradixDialogDescription>

            <label for="name">Name</label>
            <input id="name" @bind="_name" />

            <button type="button" @onclick="Close">
                Save
            </button>
        </BradixDialogContent>
    </BradixDialogPortal>
</BradixDialog>

@code {
    private bool _open;
    private string _name = "Pedro Duarte";

    private Task HandleOpenChanged(bool open)
    {
        _open = open;
        return Task.CompletedTask;
    }

    private Task Close()
    {
        _open = false;
        return Task.CompletedTask;
    }
}
```

That example shows the design philosophy:

- state can be controlled from your component
- primitives stay narrowly focused
- markup stays explicit
- styling stays entirely in your hands

## Quality Bar

Bradix is backed by more than static samples:

- a dedicated demo application for every shipped primitive
- bUnit component tests that exercise the primitives at the Razor/component boundary
- Playwright end-to-end coverage against a running demo app
- test coverage that touches essentially every shipped component and primitive
- CI packaging and verification workflows

## Is Bradix A Port Of Radix UI?

No. Bradix is **Radix-inspired**, not an official Radix port and not a perfect drop-in parity promise with the React packages.

The goal is to bring the same philosophy to Blazor:

- small composable primitives
- strong behavioral foundations
- accessibility-minded interaction patterns
- design-system freedom

