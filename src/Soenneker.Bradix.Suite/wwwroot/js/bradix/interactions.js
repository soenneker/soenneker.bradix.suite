import { cssEscape, readBooleanDataAttribute } from "./core/dom.js";
import { createDelegatedEventSnapshot } from "./core/eventSnapshots.js";
import {
  getExitSideFromRect,
  getPaddedExitPoints,
  getPointsFromRect,
  getHull,
  isPointInPolygon
} from "./core/geometry.js";

const rovingFocusHandlers = new WeakMap();
const radioGroupItemHandlers = new WeakMap();
const delegatedInteractionHandlers = new WeakMap();
const tooltipTriggerHandlers = new WeakMap();
const tooltipContentHandlers = new WeakMap();
const menuSubmenuGraceHandlers = new WeakMap();
let delegatedInteractionListenersRegistered = false;
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
const TOOLTIP_OPEN_EVENT = "tooltip.open";

export function registerRovingFocusNavigationKeys(element, dotNetRef) {
  if (!element) {
    return;
  }

  unregisterRovingFocusNavigationKeys(element);

  const keydown = (event) => {
    if (event.target !== event.currentTarget) {
      return;
    }

    if (readBooleanDataAttribute(element, "bradixPreventEnter") && event.key === "Enter") {
      event.preventDefault();
      return;
    }

    if (readBooleanDataAttribute(element, "bradixSpaceClick") && (event.key === " " || event.key === "Spacebar")) {
      event.preventDefault();
      element.click();
      return;
    }

    const groupId = element.getAttribute("data-bradix-roving-group");

    if (!groupId) {
      if (rovingFocusKeys.has(event.key)) {
        event.preventDefault();
      }

      return;
    }

    if (event.metaKey || event.ctrlKey || event.altKey || event.shiftKey) {
      return;
    }

    const target = getRovingFocusTarget(element, event.key);

    if (!target) {
      return;
    }

    event.preventDefault();

    const clickOnFocus = readBooleanDataAttribute(element, "bradixRovingClickOnFocus");

    setTimeout(() => {
      target.focus();

      if (clickOnFocus) {
        target.click();
      }
    }, 0);
  };

  const mousedown = (event) => {
    if (readBooleanDataAttribute(element, "bradixPreventNonprimaryMousedown") && (event.button !== 0 || event.ctrlKey)) {
      event.preventDefault();
      return;
    }

    if (readBooleanDataAttribute(element, "bradixPreventMousedownWhenDisabled") && !isRovingFocusableElement(element)) {
      event.preventDefault();
    }
  };

  element.addEventListener("keydown", keydown);
  element.addEventListener("mousedown", mousedown);
  rovingFocusHandlers.set(element, { keydown, mousedown });

  if (dotNetRef) {
    dotNetRef.invokeMethodAsync("HandleRovingFocusBridgeReadyAsync").catch(() => {});
  }
}

export function unregisterRovingFocusNavigationKeys(element) {
  const handlers = rovingFocusHandlers.get(element);

  if (!handlers) {
    return;
  }

  element.removeEventListener("keydown", handlers.keydown);
  element.removeEventListener("mousedown", handlers.mousedown);
  rovingFocusHandlers.delete(element);
}

export function registerRadioGroupItemKeys(element) {
  if (!element) {
    return;
  }

  unregisterRadioGroupItemKeys(element);

  const handler = (event) => {
    if (event.key === "Enter") {
      event.preventDefault();
    }
  };

  element.addEventListener("keydown", handler);
  radioGroupItemHandlers.set(element, handler);
}

export function unregisterRadioGroupItemKeys(element) {
  const handler = radioGroupItemHandlers.get(element);

  if (!handler) {
    return;
  }

  element.removeEventListener("keydown", handler);
  radioGroupItemHandlers.delete(element);
}

export function registerDelegatedInteraction(element, dotNetRef, options) {
  if (!element || !dotNetRef) {
    return;
  }

  delegatedInteractionHandlers.set(element, { dotNetRef, options: options || {} });
  ensureDelegatedInteractionListeners();

  dotNetRef.invokeMethodAsync("HandleDelegatedInteractionReadyAsync").catch(() => {});
}

export function unregisterDelegatedInteraction(element) {
  if (!element) {
    return;
  }

  delegatedInteractionHandlers.delete(element);
}

export function registerTooltipTrigger(element, dotNetRef) {
  if (!element || !dotNetRef) {
    return;
  }

  unregisterTooltipTrigger(element);

  const pointerUp = () => {
    dotNetRef.invokeMethodAsync("HandleDocumentPointerUpAsync").catch(() => {});
  };

  document.addEventListener("pointerup", pointerUp);
  tooltipTriggerHandlers.set(element, { pointerUp });
}

export function unregisterTooltipTrigger(element) {
  const handlers = tooltipTriggerHandlers.get(element);

  if (!handlers) {
    return;
  }

  document.removeEventListener("pointerup", handlers.pointerUp);
  tooltipTriggerHandlers.delete(element);
}

export function dispatchTooltipOpen(contentId) {
  document.dispatchEvent(new CustomEvent(TOOLTIP_OPEN_EVENT, {
    detail: { contentId: contentId || "" }
  }));
}

export function registerTooltipContent(content, trigger, dotNetRef, contentId, hoverableContent) {
  if (!content || !trigger || !dotNetRef) {
    return;
  }

  unregisterTooltipContent(content);

  const tooltipOpen = (event) => {
    if (event.detail && event.detail.contentId === contentId) {
      return;
    }

    dotNetRef.invokeMethodAsync("HandleTooltipOpenFromOutsideAsync").catch(() => {});
  };

  const scroll = (event) => {
    const target = event.target;

    if (target instanceof HTMLElement && target.contains(trigger)) {
      dotNetRef.invokeMethodAsync("HandleTooltipTriggerScrollAsync").catch(() => {});
    }
  };

  document.addEventListener(TOOLTIP_OPEN_EVENT, tooltipOpen);
  window.addEventListener("scroll", scroll, { capture: true });

  const state = {
    trigger,
    tooltipOpen,
    scroll,
    triggerLeave: null,
    contentLeave: null,
    pointerMove: null,
    pointerGraceArea: null
  };

  if (hoverableContent) {
    const removeGraceArea = () => {
      state.pointerGraceArea = null;

      if (state.pointerMove) {
        document.removeEventListener("pointermove", state.pointerMove);
        state.pointerMove = null;
      }

      dotNetRef.invokeMethodAsync("HandlePointerGraceAreaChangedAsync", false).catch(() => {});
    };

    const createGraceArea = (event, hoverTarget) => {
      const currentTarget = event.currentTarget;

      if (!(currentTarget instanceof HTMLElement) || !(hoverTarget instanceof HTMLElement)) {
        return;
      }

      const exitPoint = { x: event.clientX, y: event.clientY };
      const exitSide = getExitSideFromRect(exitPoint, currentTarget.getBoundingClientRect());
      const paddedExitPoints = getPaddedExitPoints(exitPoint, exitSide);
      const hoverTargetPoints = getPointsFromRect(hoverTarget.getBoundingClientRect());
      state.pointerGraceArea = getHull([...paddedExitPoints, ...hoverTargetPoints]);

      dotNetRef.invokeMethodAsync("HandlePointerGraceAreaChangedAsync", true).catch(() => {});

      if (state.pointerMove) {
        document.removeEventListener("pointermove", state.pointerMove);
      }

      state.pointerMove = (moveEvent) => {
        const target = moveEvent.target;
        const pointerPosition = { x: moveEvent.clientX, y: moveEvent.clientY };
        const hasEnteredTarget =
          (trigger instanceof HTMLElement && trigger.contains(target)) ||
          (content instanceof HTMLElement && content.contains(target));
        const isPointerOutsideGraceArea =
          state.pointerGraceArea && !isPointInPolygon(pointerPosition, state.pointerGraceArea);

        if (hasEnteredTarget) {
          removeGraceArea();
        } else if (isPointerOutsideGraceArea) {
          removeGraceArea();
          dotNetRef.invokeMethodAsync("HandleTooltipGraceAreaExitAsync").catch(() => {});
        }
      };

      document.addEventListener("pointermove", state.pointerMove);
    };

    state.triggerLeave = (event) => createGraceArea(event, content);
    state.contentLeave = (event) => createGraceArea(event, trigger);

    trigger.addEventListener("pointerleave", state.triggerLeave);
    content.addEventListener("pointerleave", state.contentLeave);
  }

  tooltipContentHandlers.set(content, state);
}

export function unregisterTooltipContent(content) {
  const handlers = tooltipContentHandlers.get(content);

  if (!handlers) {
    return;
  }

  document.removeEventListener(TOOLTIP_OPEN_EVENT, handlers.tooltipOpen);
  window.removeEventListener("scroll", handlers.scroll, { capture: true });

  if (handlers.triggerLeave && handlers.trigger) {
    handlers.trigger.removeEventListener("pointerleave", handlers.triggerLeave);
  }

  if (handlers.contentLeave) {
    content.removeEventListener("pointerleave", handlers.contentLeave);
  }

  if (handlers.pointerMove) {
    document.removeEventListener("pointermove", handlers.pointerMove);
  }

  tooltipContentHandlers.delete(content);
}

export function beginMenuSubmenuPointerGrace(trigger, content, clientX, clientY, dotNetRef) {
  if (!trigger || !content || !dotNetRef) {
    return false;
  }

  cancelMenuSubmenuPointerGrace(trigger);

  const contentSideElement = content.closest("[data-side]");
  const side = contentSideElement instanceof HTMLElement
    ? contentSideElement.dataset.side
    : null;

  if (!side) {
    return false;
  }

  const contentRect = content.getBoundingClientRect();
  if (!contentRect || (contentRect.width <= 0 && contentRect.height <= 0)) {
    return false;
  }

  const rightSide = side === "right";
  const intendedDirection = rightSide ? "right" : "left";
  const bleed = rightSide ? -5 : 5;
  const contentNearEdge = contentRect[rightSide ? "left" : "right"];
  const contentFarEdge = contentRect[rightSide ? "right" : "left"];
  let lastClientX = clientX;
  const pointerGraceArea = [
    { x: clientX + bleed, y: clientY },
    { x: contentNearEdge, y: contentRect.top },
    { x: contentFarEdge, y: contentRect.top },
    { x: contentFarEdge, y: contentRect.bottom },
    { x: contentNearEdge, y: contentRect.bottom }
  ];

  const cleanup = () => {
    const handlers = menuSubmenuGraceHandlers.get(trigger);

    if (!handlers) {
      return;
    }

    if (handlers.pointerMove) {
      document.removeEventListener("pointermove", handlers.pointerMove);
    }

    if (handlers.timeoutId) {
      window.clearTimeout(handlers.timeoutId);
    }

    menuSubmenuGraceHandlers.delete(trigger);
    dotNetRef.invokeMethodAsync("HandlePointerGraceChangedAsync", false).catch(() => {});
  };

  const pointerMove = (event) => {
    const target = event.target;
    const pointerPosition = { x: event.clientX, y: event.clientY };
    const pointerXHasChanged = event.clientX !== lastClientX;
    const pointerDirection = pointerXHasChanged
      ? (event.clientX > lastClientX ? "right" : "left")
      : null;
    const isMovingTowardsSubmenu = pointerDirection === null || pointerDirection === intendedDirection;
    const hasEnteredTarget =
      (trigger instanceof HTMLElement && trigger.contains(target)) ||
      (content instanceof HTMLElement && content.contains(target));
    const isPointerOutsideGraceArea = !isPointInPolygon(pointerPosition, pointerGraceArea);

    lastClientX = event.clientX;

    if (!isMovingTowardsSubmenu || hasEnteredTarget || isPointerOutsideGraceArea) {
      cleanup();
    }
  };

  const timeoutId = window.setTimeout(() => {
    cleanup();
  }, 300);

  menuSubmenuGraceHandlers.set(trigger, { pointerMove, timeoutId });
  document.addEventListener("pointermove", pointerMove);
  dotNetRef.invokeMethodAsync("HandlePointerGraceChangedAsync", true).catch(() => {});
  return true;
}

export function cancelMenuSubmenuPointerGrace(trigger) {
  const handlers = menuSubmenuGraceHandlers.get(trigger);

  if (!handlers) {
    return;
  }

  if (handlers.pointerMove) {
    document.removeEventListener("pointermove", handlers.pointerMove);
  }

  if (handlers.timeoutId) {
    window.clearTimeout(handlers.timeoutId);
  }

  menuSubmenuGraceHandlers.delete(trigger);
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

function isRovingFocusableElement(element) {
  if (!element) {
    return false;
  }

  if (element.hasAttribute("disabled")) {
    return false;
  }

  if (element.getAttribute("aria-disabled") === "true") {
    return false;
  }

  return !element.hasAttribute("data-disabled");
}

function getRovingFocusTarget(element, key) {
  const groupId = element.getAttribute("data-bradix-roving-group");

  if (!groupId) {
    return null;
  }

  const items = Array.from(document.querySelectorAll(`[data-bradix-roving-group="${cssEscape(groupId)}"]`))
    .filter((candidate) => candidate instanceof HTMLElement)
    .filter((candidate) => candidate.hasAttribute("data-bradix-roving-item"))
    .filter((candidate) => isRovingFocusableElement(candidate));

  const currentIndex = items.indexOf(element);

  if (currentIndex < 0 || items.length === 0) {
    return null;
  }

  const orientation = element.getAttribute("data-bradix-roving-orientation");
  const dir = element.getAttribute("data-bradix-roving-dir") === "rtl" ? "rtl" : "ltr";
  const loop = readBooleanDataAttribute(element, "bradixRovingLoop");
  const intent = getRovingFocusIntent(key, orientation, dir);

  if (!intent) {
    return null;
  }

  if (intent === "first") {
    return items[0];
  }

  if (intent === "last") {
    return items[items.length - 1];
  }

  const nextIndex = currentIndex + intent;

  if (nextIndex >= 0 && nextIndex < items.length) {
    return items[nextIndex];
  }

  if (!loop) {
    return null;
  }

  return intent < 0 ? items[items.length - 1] : items[0];
}

function getRovingFocusIntent(key, orientation, dir) {
  const horizontal = orientation !== "vertical";
  const previousKey = dir === "rtl" ? "ArrowRight" : "ArrowLeft";
  const nextKey = dir === "rtl" ? "ArrowLeft" : "ArrowRight";

  switch (key) {
    case "Home":
    case "PageUp":
      return "first";
    case "End":
    case "PageDown":
      return "last";
    case "ArrowUp":
      return horizontal ? null : -1;
    case "ArrowDown":
      return horizontal ? null : 1;
    default:
      if (key === previousKey) {
        return horizontal ? -1 : null;
      }

      if (key === nextKey) {
        return horizontal ? 1 : null;
      }

      return null;
  }
}
