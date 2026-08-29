[![](https://img.shields.io/nuget/v/soenneker.bradix.suite.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.bradix.suite/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.bradix.suite/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.bradix.suite/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.bradix.suite.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.bradix.suite/)
[![](https://img.shields.io/badge/Demo-Live-blueviolet?style=for-the-badge&logo=github)](https://soenneker.github.io/soenneker.bradix.suite)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.bradix.suite/codeql.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.bradix.suite/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Bradix.Suite

Radix-inspired, unstyled UI primitives for Blazor.

Bradix provides the behavior and structure needed for dialogs, menus, popovers, selects, tabs, tooltips, form controls, and other interactive UI. It manages concerns such as focus, keyboard navigation, dismissal, portals, positioning, and scroll locking while leaving all visual styling to your application.

Use Bradix when you are building a design system or application UI and want composable behavior instead of pre-styled controls. It is not a theme, CSS framework, or official port of Radix UI.

> This package currently targets .NET 10.

## Installation

```bash
dotnet add package Soenneker.Bradix.Suite
```

Register the suite in `Program.cs`:

```csharp
using Soenneker.Bradix;

builder.Services.AddBradixSuiteAsScoped();
```

Make the components available to Razor files by adding this to `_Imports.razor`:

```razor
@using Soenneker.Bradix
```

No npm package or manual `<script>` reference is required. Bradix loads its JavaScript modules from the package's static web assets.

## Quick start: dialog

Bradix components are assembled from small parts. This example renders an uncontrolled modal dialog: Bradix owns its open state, the trigger opens it, and each `BradixDialogClose` closes it.

```razor
<BradixDialog>
    <BradixDialogTrigger Class="dialog-trigger">
        Edit profile
    </BradixDialogTrigger>

    <BradixDialogPortal>
        <BradixDialogOverlay Class="dialog-overlay" />

        <BradixDialogContent Class="dialog-content">
            <BradixDialogTitle>Edit profile</BradixDialogTitle>
            <BradixDialogDescription>
                Update your name, then save your changes.
            </BradixDialogDescription>

            <label for="profile-name">Name</label>
            <input id="profile-name" @bind="_name" />

            <div class="dialog-actions">
                <BradixDialogClose>Cancel</BradixDialogClose>
                <BradixDialogClose OnClick="Save">Save</BradixDialogClose>
            </div>
        </BradixDialogContent>
    </BradixDialogPortal>
</BradixDialog>

@code {
    private string _name = "Ada Lovelace";

    private Task Save(MouseEventArgs _)
    {
        // Persist _name here. BradixDialogClose handles dismissal.
        return Task.CompletedTask;
    }
}
```

`BradixDialogClose` dismisses the dialog before invoking its `OnClick` callback. If a save must complete successfully before the dialog closes, use controlled state and a regular button, then set `Open` to `false` after the save succeeds.

Bradix is unstyled, so add the presentation your application needs. The following is enough to make the example usable; place it in your application's global stylesheet:

```css
.dialog-trigger,
.dialog-actions button {
    padding: 0.6rem 1rem;
    border: 1px solid #c9c9d2;
    border-radius: 0.4rem;
    background: white;
    cursor: pointer;
}

.dialog-overlay {
    position: fixed;
    inset: 0;
    z-index: 1000;
    background: rgb(0 0 0 / 55%);
}

.dialog-content {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 1001;
    width: min(90vw, 30rem);
    max-height: 85vh;
    overflow: auto;
    padding: 1.5rem;
    border-radius: 0.6rem;
    background: white;
    box-shadow: 0 1rem 3rem rgb(0 0 0 / 25%);
    transform: translate(-50%, -50%);
}

.dialog-content input {
    display: block;
    width: 100%;
    box-sizing: border-box;
    margin-top: 0.35rem;
    padding: 0.6rem;
}

.dialog-actions {
    display: flex;
    justify-content: flex-end;
    gap: 0.5rem;
    margin-top: 1.25rem;
}
```

`BradixDialogTitle` and `BradixDialogDescription` are connected to the dialog's ARIA attributes automatically. Modal dialogs also trap focus, disable outside pointer interaction, lock document scrolling, close on Escape or an outside pointer press, and return focus to the trigger.

## Controlled and uncontrolled state

Many Bradix roots support both state models:

- Omit `Open` to let Bradix manage state.
- Set `DefaultOpen="true"` to start an uncontrolled component open.
- Set `Open` and handle `OpenChanged` when application logic owns the state.

```razor
<BradixDialog Open="@_open" OpenChanged="HandleOpenChanged">
    <BradixDialogTrigger>Open settings</BradixDialogTrigger>

    <BradixDialogPortal>
        <BradixDialogOverlay Class="dialog-overlay" />
        <BradixDialogContent Class="dialog-content">
            <BradixDialogTitle>Settings</BradixDialogTitle>
            <BradixDialogDescription>Change your preferences.</BradixDialogDescription>

            <BradixDialogClose>Done</BradixDialogClose>
        </BradixDialogContent>
    </BradixDialogPortal>
</BradixDialog>

@code {
    private bool _open;

    private Task HandleOpenChanged(bool open)
    {
        _open = open;
        return Task.CompletedTask;
    }
}
```

In controlled mode, always update the value passed to `Open` from `OpenChanged`; otherwise the rendered state remains unchanged. The same pattern is used by other stateful primitives, with names such as `ValueChanged`, `CheckedChanged`, or `PressedChanged` where appropriate.

## Styling and interaction state

Component parts accept `Class`, `Style`, and unmatched HTML attributes. Bradix exposes state on the rendered elements so styles can respond without duplicating component state:

```css
.dialog-overlay[data-state="open"] {
    animation: fade-in 150ms ease-out;
}

.dialog-overlay[data-state="closed"] {
    animation: fade-out 100ms ease-in;
}

button[data-state="open"] {
    background: #eeeeff;
}
```

Depending on the primitive, useful attributes include `data-state`, `data-disabled`, `data-orientation`, `data-highlighted`, and standard ARIA attributes. Inspect the rendered element or the [live demo](https://soenneker.github.io/soenneker.bradix.suite) when defining a component's visual states.

Portal-based content is rendered outside its original DOM location. Prefer stable classes or other direct selectors for overlay, menu, popover, and tooltip content rather than selectors that depend on the trigger's DOM ancestry.

## Available primitives

| Category | Components |
| --- | --- |
| Core | `AccessibleIcon`, `AspectRatio`, `Avatar`, `Label`, `Portal`, `Presence`, `Separator`, `Slot`, `VisuallyHidden` |
| Disclosure and overlays | `Accordion`, `AlertDialog`, `Collapsible`, `Dialog`, `HoverCard`, `Popover`, `Toast`, `Tooltip` |
| Forms and input | `Checkbox`, `Form`, `OneTimePasswordField`, `Progress`, `RadioGroup`, `Select`, `Slider`, `Switch`, `Toggle`, `ToggleGroup` |
| Navigation and menus | `ContextMenu`, `DropdownMenu`, `Menubar`, `Menu`, `NavigationMenu`, `ScrollArea`, `Tabs`, `Toolbar` |
| Infrastructure | `Collection`, `DismissableLayer`, `FocusGuards`, `FocusScope`, `Popper`, `RemoveScroll` |

Most user-facing components follow a root/part composition model. For example, a popover uses `BradixPopover`, `BradixPopoverTrigger`, `BradixPopoverPortal`, and `BradixPopoverContent`; a select adds parts such as value, icon, viewport, item, item text, and item indicator.

Browse the [live component demos](https://soenneker.github.io/soenneker.bradix.suite) for complete markup and behavior for every primitive.

## Configuration

Floating UI is loaded from packaged static assets by default. To load that dependency from its CDN instead:

```csharp
builder.Services.AddBradixSuiteAsScoped(options =>
{
    options.UseCdn = true;
});
```

The packaged default is preferable when your application must avoid a runtime CDN dependency.

## Behavior notes

- Use `Dialog` for general modal content and `AlertDialog` when the user must explicitly confirm or cancel an important action.
- Keep a title and description in dialogs and alert dialogs. They provide accessible names and descriptions, even if you visually hide them.
- Portal overlay content uses layered behavior so nested popovers and menus dismiss in the correct order.
- Components with `ForceMount` can remain in the DOM while closed, which is useful when coordinating CSS animations or external animation libraries.
- Bradix is Radix-inspired, but its API is Blazor-specific and does not promise one-to-one parity with the React packages.

## Demo and source examples

The demo application contains working examples for every shipped primitive and is also used by the browser test suite:

- [Live demo](https://soenneker.github.io/soenneker.bradix.suite)
- [Demo source](https://github.com/soenneker/soenneker.bradix.suite/tree/main/test/Soenneker.Bradix.Suite.Demo/Pages)
