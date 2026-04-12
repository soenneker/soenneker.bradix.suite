import { createDelegatedEventSnapshot } from "./core/eventSnapshots.js";

const delegatedInteractionHandlers = new WeakMap();
let delegatedInteractionListenersRegistered = false;

export function registerDelegatedInteraction(element, dotNetRef, options) {
  if (!element || !dotNetRef) {
    return;
  }

  delegatedInteractionHandlers.set(element, { dotNetRef, options: options || {} });
  ensureDelegatedInteractionListeners();

  if (typeof options?.readyMethod === "string" && options.readyMethod) {
    dotNetRef.invokeMethodAsync(options.readyMethod).catch(() => {});
  }
}

export function unregisterDelegatedInteraction(element) {
  if (!element) {
    return;
  }

  delegatedInteractionHandlers.delete(element);
}

function ensureDelegatedInteractionListeners() {
  if (delegatedInteractionListenersRegistered) {
    return;
  }

  delegatedInteractionListenersRegistered = true;

  document.addEventListener("click", (event) => dispatchDelegatedInteraction("click", event));
  document.addEventListener("mousedown", (event) => dispatchDelegatedInteraction("mousedown", event));
  document.addEventListener("pointerdown", (event) => dispatchDelegatedInteraction("pointerdown", event));
  document.addEventListener("keydown", (event) => dispatchDelegatedInteraction("keydown", event));
  document.addEventListener("focusin", (event) => dispatchDelegatedInteraction("focusin", event));
  document.addEventListener("focusout", (event) => dispatchDelegatedInteraction("focusout", event));
}

function dispatchDelegatedInteraction(type, event) {
  const registration = findDelegatedInteractionRegistration(event.target);

  if (!registration) {
    return;
  }

  const config = registration.options && registration.options[type];

  if (!config) {
    return;
  }

  if (config.currentTargetOnly !== false && event.target !== registration.element) {
    return;
  }

  if (config.checkForDefaultPrevented !== false && event.defaultPrevented) {
    return;
  }

  if (Array.isArray(config.keys) && !config.keys.includes(event.key)) {
    return;
  }

  if (type === "pointerdown" && event.target instanceof HTMLElement) {
    const target = event.target;

    if (typeof target.hasPointerCapture === "function" &&
      typeof target.releasePointerCapture === "function") {
      try {
        if (target.hasPointerCapture(event.pointerId)) {
          target.releasePointerCapture(event.pointerId);
        }
      } catch {
      }
    }
  }

  if (typeof config.filter === "string" && config.filter === "primaryMousedown") {
    if (event.button !== 0 || event.ctrlKey) {
      return;
    }
  }

  if (typeof config.filter === "string" && config.filter === "primaryMousePointerDown") {
    if (event.button !== 0 || event.ctrlKey || event.pointerType !== "mouse") {
      return;
    }
  }

  if (Array.isArray(config.preventDefaultKeys) && config.preventDefaultKeys.includes(event.key)) {
    event.preventDefault();
  } else if (config.preventDefault) {
    event.preventDefault();
  }

  if (!config.method) {
    return;
  }

  registration.dotNetRef.invokeMethodAsync(config.method, createDelegatedEventSnapshot(type, event)).catch(() => {});
}

function findDelegatedInteractionRegistration(start) {
  let node = start instanceof Node ? start : null;

  while (node) {
    if (node instanceof HTMLElement) {
      const registration = delegatedInteractionHandlers.get(node);

      if (registration) {
        return {
          element: node,
          dotNetRef: registration.dotNetRef,
          options: registration.options
        };
      }
    }

    node = node.parentNode;
  }

  return null;
}
