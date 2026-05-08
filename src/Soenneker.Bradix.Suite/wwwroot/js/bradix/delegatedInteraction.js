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
  document.addEventListener("keydown", (event) => dispatchDelegatedInteraction("keydown", event), true);
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
      if (event.button !== 0 || event.ctrlKey || (event.pointerType && event.pointerType !== "mouse")) {
        continue;
      }
    }

    if (config.retargetPointerUpToOption) {
      retargetPointerUpToOption(event, registration.dotNetRef, config.pointerSelectMethod);
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

function retargetPointerUpToOption(pointerDownEvent, dotNetRef, pointerSelectMethod) {
  const originX = pointerDownEvent.clientX;
  const originY = pointerDownEvent.clientY;
  const originPageX = pointerDownEvent.pageX;
  const originPageY = pointerDownEvent.pageY;

  const pointerUp = (event) => {
    const deltaX = Math.abs(Math.round(event.clientX) - Math.round(originX));
    const deltaY = Math.abs(Math.round(event.clientY) - Math.round(originY));

    if (deltaX > 10 || deltaY > 10) {
      return;
    }

    let attempts = 0;

    const retarget = () => {
      const releaseTarget = document.elementFromPoint(event.clientX, event.clientY);
      const option = releaseTarget instanceof HTMLElement
        ? releaseTarget.closest("[role='option']")
        : null;

      if (!option) {
        if (attempts < 8) {
          attempts += 1;
          requestAnimationFrame(retarget);
        }

        return;
      }

      if (option.hasAttribute("data-disabled") || option.getAttribute("aria-disabled") === "true") {
        return;
      }

      const value = option.getAttribute("data-value");

      if (value && typeof pointerSelectMethod === "string" && pointerSelectMethod) {
        dotNetRef.invokeMethodAsync(pointerSelectMethod, value).catch(() => {});
        return;
      }

      option.dispatchEvent(new PointerEvent("pointerup", {
        bubbles: true,
        cancelable: true,
        pointerType: "mouse",
        pointerId: event.pointerId || pointerDownEvent.pointerId || 1,
        button: event.button,
        buttons: event.buttons,
        clientX: event.clientX,
        clientY: event.clientY,
        pageX: event.pageX || originPageX,
        pageY: event.pageY || originPageY,
        screenX: event.screenX,
        screenY: event.screenY,
        ctrlKey: event.ctrlKey,
        shiftKey: event.shiftKey,
        altKey: event.altKey,
        metaKey: event.metaKey
      }));
    };

    requestAnimationFrame(retarget);
  };

  document.addEventListener("pointerup", pointerUp, { capture: true, once: true });
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
