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

  document.addEventListener("click", (event) => dispatchDelegatedInteraction("click", event), true);
  document.addEventListener("mousedown", (event) => dispatchDelegatedInteraction("mousedown", event));
  document.addEventListener("pointerdown", (event) => dispatchDelegatedInteraction("pointerdown", event));
  document.addEventListener("mouseover", (event) => dispatchDelegatedInteraction("mouseover", event));
  document.addEventListener("mouseenter", (event) => dispatchDelegatedInteraction("mouseenter", event), true);
  document.addEventListener("pointermove", (event) => dispatchDelegatedInteraction("pointermove", event));
  document.addEventListener("pointerover", (event) => dispatchDelegatedInteraction("pointerover", event));
  document.addEventListener("keydown", (event) => dispatchDelegatedInteraction("keydown", event));
  document.addEventListener("focusin", (event) => dispatchDelegatedInteraction("focusin", event));
  document.addEventListener("focusout", (event) => dispatchDelegatedInteraction("focusout", event));
}

function dispatchDelegatedInteraction(type, event) {
  const registrations = findDelegatedInteractionRegistrations(event.target);

  if (registrations.length === 0) {
    return;
  }

  for (const registration of registrations) {
    const config = registration.options && registration.options[type];

    if (!config) {
      continue;
    }

    if (config.currentTargetOnly !== false && event.target !== registration.element) {
      continue;
    }

    if (config.checkForDefaultPrevented !== false && event.defaultPrevented) {
      continue;
    }

    if (Array.isArray(config.keys) && !config.keys.includes(event.key)) {
      continue;
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
        continue;
      }
    }

    if (typeof config.filter === "string" && config.filter === "primaryMousePointerDown") {
      if (event.button !== 0 || event.ctrlKey || event.pointerType !== "mouse") {
        continue;
      }
    }

    if (Array.isArray(config.preventDefaultKeys) && config.preventDefaultKeys.includes(event.key)) {
      event.preventDefault();
    } else if (config.preventDefault) {
      event.preventDefault();
    }

    if (config.stopPropagation) {
      event.stopPropagation();
      event.stopImmediatePropagation?.();
    }

    if (!config.method) {
      continue;
    }

    registration.dotNetRef.invokeMethodAsync(config.method, createDelegatedEventSnapshot(type, event)).catch(() => {});
  }
}

function findDelegatedInteractionRegistrations(start) {
  let node = start instanceof Node ? start : null;
  const registrations = [];

  while (node) {
    if (node instanceof HTMLElement) {
      const registration = delegatedInteractionHandlers.get(node);

      if (registration) {
        registrations.push({
          element: node,
          dotNetRef: registration.dotNetRef,
          options: registration.options
        });
      }
    }

    node = node.parentNode;
  }

  return registrations;
}
