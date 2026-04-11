import { cssEscape, readBooleanDataAttribute } from "./core/dom.js";

const rovingFocusHandlers = new WeakMap();
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
