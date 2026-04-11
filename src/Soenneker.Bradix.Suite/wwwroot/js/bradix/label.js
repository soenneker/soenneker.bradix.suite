import { createDelegatedEventSnapshot } from "./core/eventSnapshots.js";

const labelTextSelectionHandlers = new WeakMap();

export function registerLabelTextSelectionGuard(element, dotNetRef) {
  if (!element) {
    return;
  }

  unregisterLabelTextSelectionGuard(element);

  const handler = (event) => {
    const target = event.target;

    if (target && typeof target.closest === "function" && target.closest("button, input, select, textarea")) {
      return;
    }

    if (dotNetRef) {
      dotNetRef.invokeMethodAsync("HandleMouseDownFromJsAsync", createDelegatedEventSnapshot("mousedown", event)).catch(() => {});
    }

    if (!event.defaultPrevented && event.detail > 1) {
      event.preventDefault();
    }
  };

  element.addEventListener("mousedown", handler);
  labelTextSelectionHandlers.set(element, handler);
}

export function unregisterLabelTextSelectionGuard(element) {
  const handler = labelTextSelectionHandlers.get(element);

  if (!handler) {
    return;
  }

  element.removeEventListener("mousedown", handler);
  labelTextSelectionHandlers.delete(element);
}
