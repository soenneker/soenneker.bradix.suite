const collapsibleObservers = new WeakMap();
const rovingFocusHandlers = new WeakMap();
const labelTextSelectionHandlers = new WeakMap();
const rovingFocusKeys = new Set([
  "ArrowLeft",
  "ArrowRight",
  "ArrowUp",
  "ArrowDown",
  "Home",
  "End",
  "PageUp",
  "PageDown"
]);

function updateCollapsibleSize(element) {
  if (!element || element.hidden) {
    return;
  }

  const rect = element.getBoundingClientRect();

  if (rect.height > 0) {
    element.style.setProperty("--radix-collapsible-content-height", `${rect.height}px`);
  }

  if (rect.width > 0) {
    element.style.setProperty("--radix-collapsible-content-width", `${rect.width}px`);
  }
}

export function observeCollapsibleContent(element) {
  if (!element) {
    return;
  }

  unobserveCollapsibleContent(element);

  const observer = new ResizeObserver(() => updateCollapsibleSize(element));
  observer.observe(element);

  collapsibleObservers.set(element, observer);

  requestAnimationFrame(() => updateCollapsibleSize(element));
}

export function unobserveCollapsibleContent(element) {
  const observer = collapsibleObservers.get(element);

  if (!observer) {
    return;
  }

  observer.disconnect();
  collapsibleObservers.delete(element);
}

export function registerRovingFocusNavigationKeys(element) {
  if (!element) {
    return;
  }

  unregisterRovingFocusNavigationKeys(element);

  const handler = (event) => {
    if (rovingFocusKeys.has(event.key)) {
      event.preventDefault();
    }
  };

  element.addEventListener("keydown", handler);
  rovingFocusHandlers.set(element, handler);
}

export function unregisterRovingFocusNavigationKeys(element) {
  const handler = rovingFocusHandlers.get(element);

  if (!handler) {
    return;
  }

  element.removeEventListener("keydown", handler);
  rovingFocusHandlers.delete(element);
}

export function registerLabelTextSelectionGuard(element) {
  if (!element) {
    return;
  }

  unregisterLabelTextSelectionGuard(element);

  const handler = (event) => {
    const target = event.target;

    if (target && typeof target.closest === "function" && target.closest("button, input, select, textarea")) {
      return;
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
