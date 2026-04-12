const presenceHandlers = new WeakMap();
let focusGuardsCount = 0;
const hideOthersState = new WeakMap();
let removeScrollCount = 0;
const removeScrollAllowPinchZoomStack = [];
let originalBodyOverflow = "";
let originalBodyPaddingRight = "";
let originalDocumentTouchAction = "";

export function registerPresence(element, dotNetRef) {
  if (!element) {
    return;
  }

  unregisterPresence(element);

  const handleAnimationStart = (event) => {
    if (event.target === element) {
      dotNetRef.invokeMethodAsync("HandleAnimationStart", event.animationName || "none", getComputedStyle(element).animationName || "none");
    }
  };

  const handleAnimationEnd = (event) => {
    if (event.target === element) {
      dotNetRef.invokeMethodAsync("HandleAnimationEnd", event.animationName || "none", getComputedStyle(element).animationName || "none");
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
    return { animationName: "none", display: "none" };
  }

  const styles = getComputedStyle(element);
  return {
    animationName: styles.animationName || "none",
    display: styles.display || ""
  };
}

export function unregisterPresence(element) {
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

export function registerHideOthers(element) {
  if (!element || !document.body) {
    return;
  }

  unregisterHideOthers(element);

  const changed = [];
  const bodyChildren = Array.from(document.body.children);
  const branchSelector = "[data-bradix-dismissable-layer-branch]";

  for (const child of bodyChildren) {
    if (child === element || child.contains(element) || element.contains(child)) {
      continue;
    }

    if (child.matches(branchSelector) || child.querySelector(branchSelector)) {
      continue;
    }

    changed.push({
      element: child,
      previous: child.getAttribute("aria-hidden")
    });

    child.setAttribute("aria-hidden", "true");
  }

  hideOthersState.set(element, changed);
}

export function unregisterHideOthers(element) {
  const changed = hideOthersState.get(element);
  if (!changed) {
    return;
  }

  for (const entry of changed) {
    if (entry.previous === null) {
      entry.element.removeAttribute("aria-hidden");
    } else {
      entry.element.setAttribute("aria-hidden", entry.previous);
    }
  }

  hideOthersState.delete(element);
}

export function registerRemoveScroll(allowPinchZoom) {
  if (!document.body || !document.documentElement) {
    return;
  }

  if (removeScrollCount === 0) {
    originalBodyOverflow = document.body.style.overflow || "";
    originalBodyPaddingRight = document.body.style.paddingRight || "";
    originalDocumentTouchAction = document.documentElement.style.touchAction || "";

    const scrollbarWidth = Math.max(window.innerWidth - document.documentElement.clientWidth, 0);
    document.body.style.overflow = "hidden";

    if (scrollbarWidth > 0) {
      document.body.style.paddingRight = `${scrollbarWidth}px`;
    }
  }

  removeScrollAllowPinchZoomStack.push(Boolean(allowPinchZoom));
  removeScrollCount += 1;
  updateRemoveScrollTouchAction();
}

export function unregisterRemoveScroll() {
  if (!document.body) {
    return;
  }

  if (removeScrollAllowPinchZoomStack.length > 0) {
    removeScrollAllowPinchZoomStack.pop();
  }

  removeScrollCount = Math.max(removeScrollCount - 1, 0);

  if (removeScrollCount === 0) {
    document.body.style.overflow = originalBodyOverflow;
    document.body.style.paddingRight = originalBodyPaddingRight;
    document.documentElement.style.touchAction = originalDocumentTouchAction;
  } else {
    updateRemoveScrollTouchAction();
  }
}

function updateRemoveScrollTouchAction() {
  if (!document.documentElement) {
    return;
  }

  const allowPinchZoom = removeScrollAllowPinchZoomStack.some(Boolean);
  document.documentElement.style.touchAction = allowPinchZoom ? originalDocumentTouchAction : "none";
}
