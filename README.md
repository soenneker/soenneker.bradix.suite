[![](https://img.shields.io/nuget/v/soenneker.bradix.suite.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.bradix.suite/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.bradix.suite/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.bradix.suite/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.bradix.suite.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.bradix.suite/)
[![](https://img.shields.io/badge/Demo-Live-blueviolet?style=for-the-badge&logo=github)](https://soenneker.github.io/soenneker.bradix.suite)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.bradix.suite/codeql.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.bradix.suite/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Bradix.Suite

**Radix-inspired, unstyled UI primitives for Blazor.**

`Soenneker.Bradix.Suite` gives Blazor teams the same kind of composable primitive layer that made Radix compelling in the React world: dialogs, menus, popovers, selects, tabs, tooltips, scroll areas, form primitives, focus management, and the hard browser behavior that usually turns into a pile of one-off fixes.

This package is for teams that want:

- primitives instead of opinionated components
- accessibility-minded behavior without fighting the platform
- full control over styling and design systems
- a serious foundation for building product UI in Blazor

It is **not** a theme, and it does **not** try to hide complexity behind magical abstractions. It gives you the right low-level building blocks so your app can stay consistent, accessible, and maintainable as it grows.

## Why Bradix

Most Blazor UI libraries optimize for speed of demos. Bradix optimizes for **foundations**.

- **Composable APIs**: use trigger/content/item/viewport-style building blocks instead of giant all-in-one controls.
- **Unstyled by default**: bring your own CSS, tokens, and brand system.
- **Real browser behavior**: focus scopes, dismissable layers, portals, popper positioning, roving focus, form participation, and more are handled through a dedicated interop layer.
- **Blazor-native consumption**: install one NuGet package, register services, and start composing primitives.
- **Broad surface area**: the suite already covers core primitives, overlays, menus, forms, and infrastructure needed to build on top of them.

## What Ships Today

Bradix is a **single package** with a broad set of primitives.

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

If you want to see the suite the way consumers will actually use it, the best place to start is the live demo:

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

That example shows the design philosophy of the library:

- state can be controlled from your component
- primitives stay narrowly focused
- markup stays explicit
- styling stays entirely in your hands

## What To Expect

### 1. Unstyled on purpose

Bradix gives you structure, semantics, behavior, and composition. It does not impose a visual system. That means you can align it with your product instead of trying to undo somebody else's design choices.

### 2. Built around serious UI behavior

This suite is not just wrappers around HTML tags. A large portion of the library exists to solve the browser behavior that makes primitive libraries valuable in the first place: layered interactions, keyboard navigation, focus restoration, content positioning, scroll locking, form integration, and related interop.

### 3. Familiar controlled and uncontrolled patterns

Components such as `BradixDialog` support patterns like:

- `Open`
- `DefaultOpen`
- `OpenChanged`
- `OnOpenChange`

That keeps the primitives flexible enough for product code, demos, and design-system wrappers.

### 4. Designed for composition

Many primitives follow a Radix-style structure:

- root
- trigger
- portal
- overlay
- content
- item / indicator / viewport / separator depending on the primitive

This is the right model when you care about long-term UI consistency and extensibility.

## Requirements

- **.NET 10**: the package currently targets `net10.0`
- **Interactive Blazor rendering**: many primitives rely on browser interop for behavior
- **Your own styling**: Bradix is a primitive suite, not a themed component library

## Quality Bar

Bradix is backed by more than static samples:

- a dedicated demo application for every shipped primitive
- component-level tests
- Playwright end-to-end coverage against a running demo app
- CI packaging and verification workflows

That matters for a foundational library. Consumers should be able to trust behavior, not just screenshots.

## Is Bradix A Port Of Radix UI?

No. Bradix is **Radix-inspired**, not an official Radix port and not a drop-in parity promise with the React packages.

The goal is to bring the same philosophy to Blazor:

- small composable primitives
- strong behavioral foundations
- accessibility-minded interaction patterns
- design-system freedom

## Demo

Browse the primitives individually and see the expected composition patterns here:

[https://soenneker.github.io/soenneker.bradix.suite](https://soenneker.github.io/soenneker.bradix.suite)

## Contributing

If you are building on Bradix and hit a gap, open an issue or a PR. Foundational libraries get better when real product pressure shapes them.

## License

MIT
