import { cssEscape, readBooleanDataAttribute } from "./core/dom.js";

const rovingFocusElements = new WeakSet();
const rovingFocusRegistrations = new Map();
let rovingFocusRegistrationCount = 0;
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
const rovingFocusSelectionKeys = new Set([
  "ArrowLeft",
  "ArrowRight",
  "ArrowUp",
  "ArrowDown"
]);

export function registerRovingFocusNavigationKeys(element, dotNetRef, registrationId) {
  if (!element) {
    return;
  }

  if (registrationId) rovingFocusRegistrations.set(registrationId, element);
  if (!rovingFocusElements.has(element)) {
    rovingFocusElements.add(element);
    rovingFocusRegistrationCount += 1;

    if (rovingFocusRegistrationCount === 1) {
      document.addEventListener("keydown", handleRovingFocusKeydown, true);
      document.addEventListener("mousedown", handleRovingFocusMousedown, true);
    }
  }

  if (dotNetRef) {
    dotNetRef.invokeMethodAsync("HandleRovingFocusBridgeReady").catch(() => {});
  }
}

export function unregisterRovingFocusNavigationKeys(element, registrationId) {
  element = rovingFocusRegistrations.get(registrationId) || element;
  if (registrationId) rovingFocusRegistrations.delete(registrationId);
  if (!element || !rovingFocusElements.has(element)) {
    return;
  }

  rovingFocusElements.delete(element);
  rovingFocusRegistrationCount = Math.max(0, rovingFocusRegistrationCount - 1);

  if (rovingFocusRegistrationCount === 0) {
    document.removeEventListener("keydown", handleRovingFocusKeydown, true);
    document.removeEventListener("mousedown", handleRovingFocusMousedown, true);
  }
}

function handleRovingFocusKeydown(event) {
  if (event.isComposing || event.keyCode === 229) return;
  const element = event.target instanceof HTMLElement ? event.target : null;

  if (!element || !rovingFocusElements.has(element) || !isRovingFocusableElement(element)) {
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

  // Keep sequential keypresses ordered without waiting for a timer or .NET render.
  target.focus();
  if (target.ownerDocument.activeElement === target && clickOnFocus && rovingFocusSelectionKeys.has(event.key)) {
    target.click();
  }
}

function handleRovingFocusMousedown(event) {
  let element = event.target instanceof HTMLElement ? event.target : null;

  while (element && !rovingFocusElements.has(element)) {
    element = element.parentElement;
  }

  if (!element) {
    return;
  }

  if (readBooleanDataAttribute(element, "bradixPreventNonprimaryMousedown") && (event.button !== 0 || event.ctrlKey)) {
    event.preventDefault();
    return;
  }

  if (readBooleanDataAttribute(element, "bradixPreventMousedownWhenDisabled") && !isRovingFocusableElement(element)) {
    event.preventDefault();
  }
}

function isRovingFocusableElement(element) {
  if (!element) {
    return false;
  }

  const ignoreAriaDisabled = element.getAttribute("data-bradix-roving-ignore-aria-disabled") === "true";

  if (element.matches(":disabled") || element.closest("[inert], [hidden]")) {
    return false;
  }

  if (!ignoreAriaDisabled && element.getAttribute("aria-disabled") === "true") {
    return false;
  }

  return !element.hasAttribute("data-disabled");
}

function getRovingFocusTarget(element, key) {
  const groupId = element.getAttribute("data-bradix-roving-group");

  if (!groupId) {
    return null;
  }

  const orientation = element.getAttribute("data-bradix-roving-orientation");
  const dir = element.getAttribute("data-bradix-roving-dir") === "rtl" ? "rtl" : "ltr";
  const intent = getRovingFocusIntent(key, orientation, dir);

  if (!intent) {
    return null;
  }
  const candidates = document.querySelectorAll(
    `[data-bradix-roving-group="${cssEscape(groupId)}"][data-bradix-roving-item]`
  );
  const current = Array.prototype.indexOf.call(candidates, element);
  if (current < 0) return null;

  const count = candidates.length;
  const step = intent === "last" || intent === -1 ? -1 : 1;
  let index = intent === "first" ? 0 : intent === "last" ? count - 1 : current + step;
  const loop = readBooleanDataAttribute(element, "bradixRovingLoop");
  for (let visited = 0; visited < count; visited++, index += step) {
    if (index < 0 || index >= count) {
      if (!loop) break;
      index = step > 0 ? 0 : count - 1;
    }
    const candidate = candidates[index];
    if (candidate instanceof HTMLElement && isRovingFocusableElement(candidate)) return candidate;
  }
  return null;
}

function getRovingFocusIntent(key, orientation, dir) {
  const horizontal = orientation === "horizontal";
  const vertical = orientation === "vertical";
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
        return vertical ? null : -1;
      }

      if (key === nextKey) {
        return vertical ? null : 1;
      }

      return null;
  }
}
