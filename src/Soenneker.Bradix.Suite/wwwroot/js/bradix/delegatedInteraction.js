import { createDelegatedEventSnapshot } from "./core/eventSnapshots.js";

const delegatedInteractionHandlers = new WeakMap();
const delegatedInteractionListeners = [
  "click", "mousedown", "pointerdown", "mouseover", "mouseenter",
  "pointermove", "pointerover", "keydown", "focusin", "focusout"
].map(type => ({
  type,
  capture: type === "click" || type === "mouseenter" || type === "keydown",
  handler: event => dispatchDelegatedInteraction(type, event),
  registrations: 0
}));

export function registerDelegatedInteraction(element, dotNetRef, options) {
  if (!element || !dotNetRef) {
    return;
  }

  const resolvedOptions = options || {};
  const previous = delegatedInteractionHandlers.get(element);
  const eventMask = updateDelegatedInteractionListeners(previous?.eventMask || 0, resolvedOptions);
  delegatedInteractionHandlers.set(element, { dotNetRef, options: resolvedOptions, eventMask });

  if (typeof options?.readyMethod === "string" && options.readyMethod) {
    dotNetRef.invokeMethodAsync(options.readyMethod).catch(() => {});
  }
}

export function unregisterDelegatedInteraction(element) {
  if (!element) {
    return;
  }

  const registration = delegatedInteractionHandlers.get(element);
  if (!registration) {
    return;
  }

  delegatedInteractionHandlers.delete(element);
  updateDelegatedInteractionListeners(registration.eventMask, null);
}

function updateDelegatedInteractionListeners(previousMask, next) {
  let eventMask = 0;
  let flag = 1;
  for (const listener of delegatedInteractionListeners) {
    const wasRegistered = (previousMask & flag) !== 0;
    const isRegistered = !!next?.[listener.type];
    if (isRegistered) eventMask |= flag;
    flag <<= 1;
    if (wasRegistered === isRegistered) continue;

    if (isRegistered) {
      if (listener.registrations++ === 0) {
        document.addEventListener(listener.type, listener.handler, listener.capture);
      }
    } else if (--listener.registrations === 0) {
      document.removeEventListener(listener.type, listener.handler, listener.capture);
    }
  }
  return eventMask;
}

function dispatchDelegatedInteraction(type, event) {
  let node = event.target instanceof Node ? event.target : null;

  while (node) {
    const registration = node instanceof HTMLElement
      ? delegatedInteractionHandlers.get(node)
      : null;

    if (!registration) {
      node = node.parentNode;
      continue;
    }

    const config = registration.options && registration.options[type];

    if (!config) {
      node = node.parentNode;
      continue;
    }

    if (config.currentTargetOnly !== false && event.target !== node) {
      node = node.parentNode;
      continue;
    }

    if (config.checkForDefaultPrevented !== false && event.defaultPrevented) {
      node = node.parentNode;
      continue;
    }

    if (Array.isArray(config.keys) && !config.keys.includes(event.key)) {
      node = node.parentNode;
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
        node = node.parentNode;
        continue;
      }
    }

    if (typeof config.filter === "string" && config.filter === "primaryMousePointerDown") {
      if (event.button !== 0 || event.ctrlKey || (event.pointerType && event.pointerType !== "mouse")) {
        node = node.parentNode;
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
      node = node.parentNode;
      continue;
    }

    registration.dotNetRef.invokeMethodAsync(config.method, createDelegatedEventSnapshot(type, event)).catch(() => {});
    node = node.parentNode;
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

    if (deltaX <= 10 && deltaY <= 10) {
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
