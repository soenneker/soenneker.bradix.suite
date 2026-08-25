import { getTabbableCandidates, focusFirst, focusElement } from "./core/focus.js";

const toastViewportHandlers = new WeakMap();
const toastSwipeHandlers = new WeakMap();

export function registerToastViewport(wrapper, viewport, headProxy, tailProxy, hotkey, dotNetRef) {
  if (!viewport) {
    return;
  }

  unregisterToastViewport(viewport);

  const resolvedWrapper = wrapper instanceof Element ? wrapper : null;
  const resolvedHeadProxy = headProxy instanceof Element ? headProxy : null;
  const resolvedTailProxy = tailProxy instanceof Element ? tailProxy : null;

  const hotkeys = Array.isArray(hotkey) ? hotkey : [];
  const hasToasts = () => viewport.childElementCount > 0;
  const invokePause = () => {
    if (hasToasts() && dotNetRef) {
      dotNetRef.invokeMethodAsync("HandlePause");
    }
  };
  const invokeResume = () => {
    if (dotNetRef) {
      dotNetRef.invokeMethodAsync("HandleResume");
    }
  };
  const getSortedCandidates = (backwards) => {
    const toastItems = viewport.querySelectorAll('[data-radix-toast-root]');
    const candidates = [];

    if (backwards) {
      for (const toast of toastItems) {
        const toastCandidates = getTabbableCandidates(toast);
        for (let index = toastCandidates.length - 1; index >= 0; index--) {
          candidates.push(toastCandidates[index]);
        }
        candidates.push(toast);
      }
    } else {
      for (let toastIndex = toastItems.length - 1; toastIndex >= 0; toastIndex--) {
        const toast = toastItems[toastIndex];
        candidates.push(toast);
        const toastCandidates = getTabbableCandidates(toast);
        for (const candidate of toastCandidates) {
          candidates.push(candidate);
        }
      }
    }

    return candidates;
  };
  const focusFromProxy = (backwards) => {
    const previous = document.activeElement;
    if (viewport.contains(previous)) {
      return;
    }

    focusFirst(getSortedCandidates(backwards), false);
  };
  const keydown = (event) => {
    const isHotkeyPressed = hotkeys.length !== 0 && hotkeys.every((key) => {
      return event[key] || event.code === key || event.key === key;
    });

    if (isHotkeyPressed) {
      focusElement(viewport, false);
      requestAnimationFrame(() => focusElement(viewport, false));
    }
  };
  const focusToastFromViewport = (backwards) => {
    if (backwards) {
      focusElement(resolvedHeadProxy, false);
      return;
    }

    const toastItems = viewport.querySelectorAll('[data-radix-toast-root]');
    const target = toastItems.length > 0 ? toastItems[toastItems.length - 1] : null;
    focusElement(target, false);

    if (document.activeElement !== target) {
      focusElement(resolvedTailProxy, false);
    }
  };
  const documentTabKeydown = (event) => {
    const isMetaKey = event.altKey || event.ctrlKey || event.metaKey;
    if (event.key !== "Tab" || isMetaKey || document.activeElement !== viewport) {
      return;
    }

    event.preventDefault();
    focusToastFromViewport(event.shiftKey);
    requestAnimationFrame(() => focusToastFromViewport(event.shiftKey));
  };
  const focusin = () => invokePause();
  const focusout = (event) => {
    if (!resolvedWrapper || resolvedWrapper.contains(event.relatedTarget)) {
      return;
    }

    invokeResume();
  };
  const pointerenter = () => invokePause();
  const pointerleave = () => {
    if (!resolvedWrapper || resolvedWrapper.contains(document.activeElement)) {
      return;
    }

    invokeResume();
  };
  const windowBlur = () => invokePause();
  const windowFocus = () => invokeResume();
  const viewportKeydown = (event) => {
    const isMetaKey = event.altKey || event.ctrlKey || event.metaKey;
    if (event.key !== "Tab" || isMetaKey) {
      return;
    }

    const backwards = event.shiftKey;
    const targetIsViewport = event.target === viewport;
    if (targetIsViewport) {
      event.preventDefault();
      focusToastFromViewport(backwards);
      requestAnimationFrame(() => focusToastFromViewport(backwards));
      return;
    }

    const sortedCandidates = getSortedCandidates(backwards);
    const index = sortedCandidates.findIndex((candidate) => candidate === document.activeElement);
    if (focusFirst(sortedCandidates, false, index + 1)) {
      event.preventDefault();
    } else {
      focusElement(backwards ? resolvedHeadProxy : resolvedTailProxy, false);
    }
  };
  const headFocus = (event) => {
    if (!viewport.contains(event.relatedTarget)) {
      focusFromProxy(false);
    }
  };
  const tailFocus = (event) => {
    if (!viewport.contains(event.relatedTarget)) {
      focusFromProxy(true);
    }
  };

  document.addEventListener("keydown", keydown);
  document.addEventListener("keydown", documentTabKeydown, true);
  if (resolvedWrapper) {
    resolvedWrapper.addEventListener("focusin", focusin);
    resolvedWrapper.addEventListener("focusout", focusout);
    resolvedWrapper.addEventListener("pointerenter", pointerenter);
    resolvedWrapper.addEventListener("pointerleave", pointerleave);
  }
  window.addEventListener("blur", windowBlur);
  window.addEventListener("focus", windowFocus);
  viewport.addEventListener("keydown", viewportKeydown);
  if (resolvedHeadProxy) {
    resolvedHeadProxy.addEventListener("focus", headFocus);
  }
  if (resolvedTailProxy) {
    resolvedTailProxy.addEventListener("focus", tailFocus);
  }

  toastViewportHandlers.set(viewport, {
    wrapper: resolvedWrapper,
    keydown,
    documentTabKeydown,
    focusin,
    focusout,
    pointerenter,
    pointerleave,
    windowBlur,
    windowFocus,
    viewportKeydown,
    headProxy: resolvedHeadProxy,
    headFocus,
    tailProxy: resolvedTailProxy,
    tailFocus
  });
}

export function unregisterToastViewport(viewport) {
  const handlers = toastViewportHandlers.get(viewport);

  if (!handlers) {
    return;
  }

  document.removeEventListener("keydown", handlers.keydown);
  document.removeEventListener("keydown", handlers.documentTabKeydown, true);
  if (handlers.wrapper) {
    handlers.wrapper.removeEventListener("focusin", handlers.focusin);
    handlers.wrapper.removeEventListener("focusout", handlers.focusout);
    handlers.wrapper.removeEventListener("pointerenter", handlers.pointerenter);
    handlers.wrapper.removeEventListener("pointerleave", handlers.pointerleave);
  }
  window.removeEventListener("blur", handlers.windowBlur);
  window.removeEventListener("focus", handlers.windowFocus);
  viewport.removeEventListener("keydown", handlers.viewportKeydown);
  if (handlers.headProxy) {
    handlers.headProxy.removeEventListener("focus", handlers.headFocus);
  }
  if (handlers.tailProxy) {
    handlers.tailProxy.removeEventListener("focus", handlers.tailFocus);
  }

  toastViewportHandlers.delete(viewport);
}

export function registerToastSwipe(toast, direction, threshold, notifyStart, notifyMove, dotNetRef) {
  if (!toast) {
    return;
  }

  unregisterToastSwipe(toast);

  const state = {
    pointerId: null,
    pointerType: "",
    startX: 0,
    startY: 0,
    moveX: 0,
    moveY: 0,
    started: false
  };

  const reset = (clearVisualState) => {
    state.pointerId = null;
    state.started = false;
    state.moveX = 0;
    state.moveY = 0;
    if (clearVisualState) {
      delete toast.dataset.swipe;
    }
  };

  const pointerdown = (event) => {
    if (event.button !== 0) {
      return;
    }

    state.pointerId = event.pointerId;
    state.pointerType = event.pointerType || "";
    state.startX = event.clientX;
    state.startY = event.clientY;
    state.moveX = 0;
    state.moveY = 0;
    state.started = false;
    delete toast.dataset.swipe;
    clearSwipeProperties(toast);
  };

  const pointermove = (event) => {
    if (state.pointerId === null || state.pointerId !== event.pointerId) {
      return;
    }

    const x = event.clientX - state.startX;
    const y = event.clientY - state.startY;
    const horizontal = direction === "left" || direction === "right";
    const moveX = horizontal ? clampSwipeDelta(x, direction) : 0;
    const moveY = horizontal ? 0 : clampSwipeDelta(y, direction);
    const startBuffer = state.pointerType.toLowerCase() === "touch" ? 10 : 2;

    if (!state.started) {
      if (!isSwipeDeltaInDirection(moveX, moveY, direction, startBuffer)) {
        if (Math.abs(x) > startBuffer || Math.abs(y) > startBuffer) {
          reset(true);
        }
        return;
      }

      state.started = true;
      toast.dataset.swipe = "start";
      try {
        toast.setPointerCapture(event.pointerId);
      } catch {
      }

      if (notifyStart) {
        dotNetRef?.invokeMethodAsync("HandleSwipeStartFromJs", moveX, moveY).catch(console.error);
      }
    }

    state.moveX = moveX;
    state.moveY = moveY;
    toast.dataset.swipe = "move";
    toast.style.setProperty("--radix-toast-swipe-move-x", `${moveX}px`);
    toast.style.setProperty("--radix-toast-swipe-move-y", `${moveY}px`);

    if (notifyMove) {
      dotNetRef?.invokeMethodAsync("HandleSwipeMoveFromJs", moveX, moveY).catch(console.error);
    }
  };

  const complete = (event, canceled) => {
    if (state.pointerId === null || state.pointerId !== event.pointerId) {
      return;
    }

    try {
      toast.releasePointerCapture(event.pointerId);
    } catch {
    }

    if (!state.started) {
      reset(true);
      return;
    }

    const moveX = state.moveX;
    const moveY = state.moveY;
    const ended = !canceled && isSwipeDeltaInDirection(moveX, moveY, direction, threshold);
    toast.dataset.swipe = ended ? "end" : "cancel";
    toast.style.removeProperty("--radix-toast-swipe-move-x");
    toast.style.removeProperty("--radix-toast-swipe-move-y");

    if (ended) {
      toast.style.setProperty("--radix-toast-swipe-end-x", `${moveX}px`);
      toast.style.setProperty("--radix-toast-swipe-end-y", `${moveY}px`);
    } else {
      toast.style.removeProperty("--radix-toast-swipe-end-x");
      toast.style.removeProperty("--radix-toast-swipe-end-y");
    }

    reset(false);
    dotNetRef?.invokeMethodAsync("HandleSwipeCompletedFromJs", ended, moveX, moveY).catch(console.error);
  };

  const pointerup = event => complete(event, false);
  const pointercancel = event => complete(event, true);
  toast.addEventListener("pointerdown", pointerdown);
  toast.addEventListener("pointermove", pointermove);
  toast.addEventListener("pointerup", pointerup);
  toast.addEventListener("pointercancel", pointercancel);
  toastSwipeHandlers.set(toast, { pointerdown, pointermove, pointerup, pointercancel });
}

export function unregisterToastSwipe(toast) {
  const handlers = toastSwipeHandlers.get(toast);
  if (!handlers) {
    return;
  }

  toast.removeEventListener("pointerdown", handlers.pointerdown);
  toast.removeEventListener("pointermove", handlers.pointermove);
  toast.removeEventListener("pointerup", handlers.pointerup);
  toast.removeEventListener("pointercancel", handlers.pointercancel);
  toastSwipeHandlers.delete(toast);
}

function clearSwipeProperties(toast) {
  toast.style.removeProperty("--radix-toast-swipe-move-x");
  toast.style.removeProperty("--radix-toast-swipe-move-y");
  toast.style.removeProperty("--radix-toast-swipe-end-x");
  toast.style.removeProperty("--radix-toast-swipe-end-y");
}

function clampSwipeDelta(delta, direction) {
  return direction === "left" || direction === "up" ? Math.min(0, delta) : Math.max(0, delta);
}

function isSwipeDeltaInDirection(x, y, direction, threshold) {
  const deltaX = Math.abs(x);
  const deltaY = Math.abs(y);
  const isDeltaX = deltaX > deltaY;
  if (direction === "right") return isDeltaX && x > threshold;
  if (direction === "left") return isDeltaX && x < -threshold;
  if (direction === "down") return !isDeltaX && y > threshold;
  return !isDeltaX && y < -threshold;
}

export function isToastFocused(toast) {
  return !!(toast && toast.contains(document.activeElement));
}

export function getToastAnnounceText(element) {
  if (!element) {
    return [];
  }

  return collectToastAnnounceText(element);
}

function collectToastAnnounceText(container, textContent = []) {
  const childNodes = container.childNodes || [];

  for (const node of childNodes) {
    if (node.nodeType === Node.TEXT_NODE && node.textContent) {
      textContent.push(node.textContent);
      continue;
    }

    if (!(node instanceof HTMLElement)) {
      continue;
    }

    const isHidden = node.ariaHidden === "true" || node.hidden || node.style.display === "none";
    const isExcluded = node.dataset && node.dataset.radixToastAnnounceExclude === "";

    if (isHidden) {
      continue;
    }

    if (isExcluded) {
      const altText = node.dataset.radixToastAnnounceAlt;
      if (altText) {
        textContent.push(altText);
      }
      continue;
    }

    collectToastAnnounceText(node, textContent);
  }

  return textContent;
}
