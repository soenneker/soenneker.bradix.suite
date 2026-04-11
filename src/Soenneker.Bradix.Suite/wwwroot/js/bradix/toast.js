import { getTabbableCandidates, focusFirst, focusElement } from "./core/focus.js";

const toastViewportHandlers = new WeakMap();

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
      dotNetRef.invokeMethodAsync("HandlePauseAsync");
    }
  };
  const invokeResume = () => {
    if (dotNetRef) {
      dotNetRef.invokeMethodAsync("HandleResumeAsync");
    }
  };
  const getSortedCandidates = (backwards) => {
    const toastItems = Array.from(viewport.querySelectorAll('[data-radix-toast-root]'));
    const orderedItems = backwards ? toastItems : [...toastItems].reverse();
    return orderedItems.flatMap((toast) => {
      const candidates = [toast, ...getTabbableCandidates(toast)];
      return backwards ? candidates.reverse() : candidates;
    });
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
    }
  };
  const focusin = () => invokePause();
  const focusout = (event) => {
    if (!resolvedWrapper || resolvedWrapper.contains(event.relatedTarget)) {
      return;
    }

    invokeResume();
  };
  const pointermove = () => invokePause();
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
    if (targetIsViewport && backwards) {
      focusElement(resolvedHeadProxy, false);
      return;
    }

    const sortedCandidates = getSortedCandidates(backwards);
    const index = sortedCandidates.findIndex((candidate) => candidate === document.activeElement);
    const nextCandidates = sortedCandidates.slice(index + 1);
    const previous = document.activeElement;
    focusFirst(nextCandidates, false);
    if (document.activeElement === previous) {
      focusElement(backwards ? resolvedHeadProxy : resolvedTailProxy, false);
    }

    event.preventDefault();
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
  if (resolvedWrapper) {
    resolvedWrapper.addEventListener("focusin", focusin);
    resolvedWrapper.addEventListener("focusout", focusout);
    resolvedWrapper.addEventListener("pointermove", pointermove);
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
    focusin,
    focusout,
    pointermove,
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
  if (handlers.wrapper) {
    handlers.wrapper.removeEventListener("focusin", handlers.focusin);
    handlers.wrapper.removeEventListener("focusout", handlers.focusout);
    handlers.wrapper.removeEventListener("pointermove", handlers.pointermove);
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

export function isToastFocused(toast) {
  return !!(toast && toast.contains(document.activeElement));
}

export function getToastAnnounceText(element) {
  if (!element) {
    return [];
  }

  return collectToastAnnounceText(element);
}

function collectToastAnnounceText(container) {
  const textContent = [];
  const childNodes = Array.from(container.childNodes || []);

  childNodes.forEach((node) => {
    if (node.nodeType === Node.TEXT_NODE && node.textContent) {
      textContent.push(node.textContent);
      return;
    }

    if (!(node instanceof HTMLElement)) {
      return;
    }

    const isHidden = node.ariaHidden === "true" || node.hidden || node.style.display === "none";
    const isExcluded = node.dataset && node.dataset.radixToastAnnounceExclude === "";

    if (isHidden) {
      return;
    }

    if (isExcluded) {
      const altText = node.dataset.radixToastAnnounceAlt;
      if (altText) {
        textContent.push(altText);
      }
      return;
    }

    textContent.push(...collectToastAnnounceText(node));
  });

  return textContent;
}
