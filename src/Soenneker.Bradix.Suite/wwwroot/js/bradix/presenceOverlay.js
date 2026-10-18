const presenceHandlers = new WeakMap();
const presenceElements = new Map();
const pendingPresenceInvocations = new Map();
let focusGuardsCount = 0;
const hideOthersState = new Map();
let accessibilityLayers = [];
const hiddenElements = new WeakMap();
const removeScrollRegistrations = new Map();
let removeScrollLegacyCount = 0;
const removeScrollLegacyAllowPinchZoomStack = [];
let originalBodyOverflow = "";
let originalBodyPaddingRight = "";
let originalDocumentTouchAction = "";

function invokeDotNetSafely(dotNetRef, methodName, ...args) {
  const id = dotNetRef?._id;
  try {
    const invocation = Promise.resolve(dotNetRef?.invokeMethodAsync?.(methodName, ...args)).catch(() => {});
    let pending = pendingPresenceInvocations.get(id);
    if (!pending) pendingPresenceInvocations.set(id, pending = new Set());
    pending.add(invocation);
    invocation.then(() => {
      pending.delete(invocation);
      if (pending.size === 0) pendingPresenceInvocations.delete(id);
    });
  } catch {
  }
}

export function waitForPresenceCallbacks(dotNetRef) {
  return Promise.all(pendingPresenceInvocations.get(dotNetRef?._id) || []).then(() => undefined);
}

export function registerPresence(element, dotNetRef, registrationId) {
  if (!element) {
    return;
  }

  unregisterPresence(element, registrationId);
  if (registrationId) presenceElements.set(registrationId, element);

  let fillModeForced = false;
  let previousAnimationFillMode = "";

  const restoreAnimationFillMode = () => {
    if (!fillModeForced) {
      return;
    }

    element.style.animationFillMode = previousAnimationFillMode;
    previousAnimationFillMode = "";
    fillModeForced = false;
  };

  const forceAnimationFillModeForwards = () => {
    if (!fillModeForced) {
      previousAnimationFillMode = element.style.animationFillMode || "";
      fillModeForced = true;
    }

    element.style.animationFillMode = "forwards";
  };

  const isClosing = () => element.getAttribute("data-state") === "closed";

  const handleAnimationStart = (event) => {
    if (event.target === element) {
      restoreAnimationFillMode();
      invokeDotNetSafely(dotNetRef, "HandleAnimationStart", event.animationName || "none", getComputedStyle(element).animationName || "none");
    }
  };

  const handleAnimationEnd = (event) => {
    if (event.target === element) {
      if (isClosing()) {
        forceAnimationFillModeForwards();
      }

      invokeDotNetSafely(dotNetRef, "HandleAnimationEnd", event.animationName || "none", getComputedStyle(element).animationName || "none");
    }
  };

  element.addEventListener("animationstart", handleAnimationStart);
  element.addEventListener("animationcancel", handleAnimationEnd);
  element.addEventListener("animationend", handleAnimationEnd);

  presenceHandlers.set(element, {
    handleAnimationStart,
    handleAnimationEnd
  });
}

export function getPresenceState(element) {
  if (!element) {
    return { animationName: "none", display: "none", hasActiveAnimation: false };
  }

  const styles = getComputedStyle(element);
  const animations = typeof element.getAnimations === "function" ? element.getAnimations() : [];
  return {
    animationName: styles.animationName || "none",
    display: styles.display || "",
    hasActiveAnimation: animations.some(animation => animation.playState === "pending" || animation.playState === "running")
  };
}

export function unregisterPresence(element, registrationId) {
  element = presenceElements.get(registrationId) || element;
  if (registrationId) presenceElements.delete(registrationId);
  const handlers = presenceHandlers.get(element);
  if (!handlers) {
    return;
  }

  element.removeEventListener("animationstart", handlers.handleAnimationStart);
  element.removeEventListener("animationcancel", handlers.handleAnimationEnd);
  element.removeEventListener("animationend", handlers.handleAnimationEnd);
  presenceHandlers.delete(element);
}

function createFocusGuard() {
  const element = document.createElement("span");
  element.setAttribute("data-radix-focus-guard", "");
  element.tabIndex = 0;
  element.style.outline = "none";
  element.style.opacity = "0";
  element.style.position = "fixed";
  element.style.pointerEvents = "none";
  return element;
}

export function registerFocusGuards() {
  const edgeGuards = document.querySelectorAll("[data-radix-focus-guard]");
  document.body.insertAdjacentElement("afterbegin", edgeGuards[0] || createFocusGuard());
  document.body.insertAdjacentElement("beforeend", edgeGuards[1] || createFocusGuard());
  focusGuardsCount += 1;
}

export function unregisterFocusGuards() {
  if (focusGuardsCount <= 1) {
    document.querySelectorAll("[data-radix-focus-guard]").forEach((node) => node.remove());
  }

  focusGuardsCount = Math.max(focusGuardsCount - 1, 0);
}

export function updateAccessibilityLayers(layers) {
  accessibilityLayers = layers;
  for (const registration of hideOthersState.values()) applyHideOthers(registration);
}

export function registerHideOthers(element, registrationId) {
  const body = element?.ownerDocument?.body;
  if (!element || !body?.contains(element)) return;
  const key = registrationId || element;
  unregisterHideOthers(element, registrationId);
  const registration = { element, changed: new Set() };
  hideOthersState.set(key, registration);
  applyHideOthers(registration);
}

function releaseHiddenElement(child) {
  const state = hiddenElements.get(child);
  if (!state || --state.count > 0) return;
  child.inert = state.inert;
  if (state.ariaHidden === null) child.removeAttribute("aria-hidden");
  else child.setAttribute("aria-hidden", state.ariaHidden);
  hiddenElements.delete(child);
}

function applyHideOthers(registration) {
  const { element, changed } = registration;
  const body = element.ownerDocument.body;
  const targets = new Set([element, ...body.querySelectorAll("[aria-live]")]);
  const layerIndex = accessibilityLayers.findIndex(layer => layer === element || layer.contains(element) || element.contains(layer));
  if (layerIndex >= 0) {
    for (let index = layerIndex + 1; index < accessibilityLayers.length; index++) {
      const layer = accessibilityLayers[index];
      if (layer.ownerDocument === element.ownerDocument) targets.add(layer);
    }
  }
  const retainedAncestors = new Set();
  for (const target of targets) {
    for (let parent = target; parent && parent !== body; parent = parent.parentElement) retainedAncestors.add(parent);
  }
  const next = new Set();
  const hideSiblings = parent => {
    for (const child of parent.children) {
      if (targets.has(child)) continue;
      if (retainedAncestors.has(child)) hideSiblings(child);
      else next.add(child);
    }
  };
  if (element.isConnected) hideSiblings(body);
  for (const child of changed) if (!next.has(child)) releaseHiddenElement(child);
  for (const child of next) {
    if (changed.has(child)) continue;
    const state = hiddenElements.get(child);
    if (state) state.count++;
    else {
      hiddenElements.set(child, { count: 1, inert: child.inert === true, ariaHidden: child.getAttribute("aria-hidden") });
      child.inert = true;
      child.setAttribute("aria-hidden", "true");
    }
  }
  registration.changed = next;
}

export function unregisterHideOthers(element, registrationId) {
  const key = registrationId || element;
  const registration = hideOthersState.get(key);
  if (!registration) return;
  for (const child of registration.changed) releaseHiddenElement(child);
  hideOthersState.delete(key);
}

export function registerRemoveScroll(registrationIdOrAllowPinchZoom, maybeAllowPinchZoom) {
  if (!document.body || !document.documentElement) {
    return;
  }

  const registrationId = getRemoveScrollRegistrationId(registrationIdOrAllowPinchZoom);
  const allowPinchZoom = registrationId ? maybeAllowPinchZoom : registrationIdOrAllowPinchZoom;

  if (!hasActiveRemoveScroll()) {
    originalBodyOverflow = document.body.style.overflow || "";
    originalBodyPaddingRight = document.body.style.paddingRight || "";
    originalDocumentTouchAction = document.documentElement.style.touchAction || "";

    const stableGutter = getComputedStyle(document.documentElement).getPropertyValue("scrollbar-gutter").includes("stable");
    const scrollbarWidth = stableGutter ? 0 : Math.max(window.innerWidth - document.documentElement.clientWidth, 0);
    const paddingRight = scrollbarWidth > 0 ? parseFloat(getComputedStyle(document.body).paddingRight) || 0 : 0;
    document.body.style.overflow = "hidden";

    if (scrollbarWidth > 0) {
      document.body.style.paddingRight = `${paddingRight + scrollbarWidth}px`;
    }
  }

  if (registrationId) {
    removeScrollRegistrations.set(registrationId, Boolean(allowPinchZoom));
  } else {
    removeScrollLegacyAllowPinchZoomStack.push(Boolean(allowPinchZoom));
    removeScrollLegacyCount += 1;
  }

  updateRemoveScrollTouchAction();
}

export function unregisterRemoveScroll(registrationId) {
  if (!document.body || !document.documentElement) {
    return;
  }

  const activeRegistrationId = getRemoveScrollRegistrationId(registrationId);

  if (activeRegistrationId) {
    removeScrollRegistrations.delete(activeRegistrationId);
  } else {
    if (removeScrollLegacyAllowPinchZoomStack.length > 0) {
      removeScrollLegacyAllowPinchZoomStack.pop();
    }

    removeScrollLegacyCount = Math.max(removeScrollLegacyCount - 1, 0);
  }

  if (!hasActiveRemoveScroll()) {
    document.body.style.overflow = originalBodyOverflow;
    document.body.style.paddingRight = originalBodyPaddingRight;
    document.documentElement.style.touchAction = originalDocumentTouchAction;
    removeScrollLegacyAllowPinchZoomStack.length = 0;
  } else {
    updateRemoveScrollTouchAction();
  }
}

function getRemoveScrollRegistrationId(value) {
  return typeof value === "string" && value.length > 0 ? value : null;
}

function hasActiveRemoveScroll() {
  return removeScrollRegistrations.size > 0 || removeScrollLegacyCount > 0;
}

function updateRemoveScrollTouchAction() {
  if (!document.documentElement) {
    return;
  }

  let allowPinchZoom = removeScrollLegacyAllowPinchZoomStack.some(Boolean);
  if (!allowPinchZoom) {
    for (const registrationAllowsPinchZoom of removeScrollRegistrations.values()) {
      if (registrationAllowsPinchZoom) {
        allowPinchZoom = true;
        break;
      }
    }
  }
  document.documentElement.style.touchAction = allowPinchZoom ? originalDocumentTouchAction : "none";
}
