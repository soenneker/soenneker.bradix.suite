import {
  arrayRemove,
  getTabbableCandidates,
  getTabbableEdges,
  focusElement,
  focusFirst,
  removeLinks
} from "./core/focus.js";

const focusScopeHandlers = new WeakMap();
const focusScopeStack = [];

function invokeDotNetSafely(dotNetRef, methodName, fallbackValue, ...args) {
  try {
    const invocation = dotNetRef?.invokeMethodAsync?.(methodName, ...args);
    if (invocation && typeof invocation.then === "function") {
      return invocation.catch(() => fallbackValue);
    }
    return Promise.resolve(invocation ?? fallbackValue);
  } catch {
    return Promise.resolve(fallbackValue);
  }
}

function addFocusScopeToStack(scope) {
  const active = focusScopeStack[0];
  if (active && active !== scope) {
    active.paused = true;
  }

  const next = arrayRemove(focusScopeStack, scope);
  next.unshift(scope);
  focusScopeStack.length = 0;
  focusScopeStack.push(...next);
}

function removeFocusScopeFromStack(scope) {
  const next = arrayRemove(focusScopeStack, scope);
  focusScopeStack.length = 0;
  focusScopeStack.push(...next);
  if (focusScopeStack[0]) {
    focusScopeStack[0].paused = false;
  }
}

export async function registerFocusScope(element, dotNetRef, loop, trapped, preventMountAutoFocus, preventUnmountAutoFocus) {
  if (!element) {
    return;
  }

  await unregisterFocusScope(element, true);

  const scope = {
    element,
    dotNetRef,
    loop: !!loop,
    trapped: !!trapped,
    preventMountAutoFocus: !!preventMountAutoFocus,
    preventUnmountAutoFocus: !!preventUnmountAutoFocus,
    paused: false,
    awaitingMountAutoFocus: false,
    lastFocusedElement: null,
    previouslyFocusedElement: document.activeElement instanceof HTMLElement ? document.activeElement : null
  };

  const tryMountAutoFocusCandidate = () => focusFirst(removeLinks(getTabbableCandidates(scope.element)), true);

  const completeMountAutoFocusIfFocused = () => {
    const activeElement = document.activeElement;
    if (activeElement && scope.element.contains(activeElement) && activeElement !== scope.element) {
      scope.awaitingMountAutoFocus = false;
      return true;
    }

    return false;
  };

  const focusLastInsideScope = () => {
    const target = scope.lastFocusedElement && scope.element.contains(scope.lastFocusedElement)
      ? scope.lastFocusedElement
      : scope.element;

    const restore = () => {
      const activeElement = document.activeElement;
      if (!scope.paused && scope.trapped && (!activeElement || !scope.element.contains(activeElement))) {
        focusElement(target, true);
      }
    };

    focusElement(target, true);
    queueMicrotask(() => {
      restore();
      requestAnimationFrame(restore);
    });
  };

  const focusin = (event) => {
    if (scope.paused || !scope.trapped) {
      return;
    }

    const target = event.target;
    if (scope.element.contains(target)) {
      scope.lastFocusedElement = target;
    } else {
      focusLastInsideScope();
    }
  };

  const focusout = (event) => {
    if (scope.paused || !scope.trapped) {
      return;
    }

    const relatedTarget = event.relatedTarget;
    if (relatedTarget === null) {
      return;
    }

    if (!scope.element.contains(relatedTarget)) {
      focusLastInsideScope();
    }
  };

  const mutationObserver = new MutationObserver((mutations) => {
    if (scope.awaitingMountAutoFocus && !scope.paused) {
      for (const mutation of mutations) {
        if (mutation.addedNodes.length > 0 || mutation.type === "attributes") {
          tryMountAutoFocusCandidate();
          completeMountAutoFocusIfFocused();
          break;
        }
      }
    }

    if (scope.trapped) {
      const focusedElement = document.activeElement;
      if (focusedElement !== document.body) {
        return;
      }

      for (const mutation of mutations) {
        if (mutation.removedNodes.length > 0) {
          focusElement(scope.element, false);
          break;
        }
      }
    }
  });

  const keydown = (event) => {
    if ((!scope.loop && !scope.trapped) || scope.paused) {
      return;
    }

    const isTabKey = event.key === "Tab" && !event.altKey && !event.ctrlKey && !event.metaKey;
    const focusedElement = document.activeElement;

    if (!isTabKey || !focusedElement) {
      return;
    }

    const [first, last] = getTabbableEdges(scope.element);
    const hasTabbableElementsInside = first && last;

    if (!hasTabbableElementsInside) {
      if (focusedElement === scope.element) {
        event.preventDefault();
      }
      return;
    }

    if (!event.shiftKey && focusedElement === last) {
      event.preventDefault();
      if (scope.loop) {
        focusElement(first, true);
      }
    } else if (event.shiftKey && focusedElement === first) {
      event.preventDefault();
      if (scope.loop) {
        focusElement(last, true);
      }
    }
  };

  document.addEventListener("focusin", focusin);
  document.addEventListener("focusout", focusout);
  scope.element.addEventListener("keydown", keydown);
  mutationObserver.observe(scope.element, { attributes: true, attributeFilter: ["data-state", "hidden", "style"], childList: true, subtree: true });

  const handlers = { scope, focusin, focusout, keydown, mutationObserver };
  focusScopeHandlers.set(element, handlers);

  addFocusScopeToStack(scope);

  const previous = scope.previouslyFocusedElement;
  const hasFocusedCandidate = previous && scope.element.contains(previous);
  if (!hasFocusedCandidate) {
    const mountAutoFocusPrevented = await invokeDotNetSafely(dotNetRef, "HandleMountAutoFocus", false);
    if (focusScopeHandlers.get(element) !== handlers) {
      return;
    }

    if (!scope.preventMountAutoFocus && !mountAutoFocusPrevented) {
      scope.awaitingMountAutoFocus = true;
      tryMountAutoFocusCandidate();
      if (document.activeElement === previous) {
        focusElement(scope.element, false);
      }

      const retryMountAutoFocus = () => {
        if (focusScopeHandlers.get(element) !== handlers || scope.paused) {
          return;
        }

        if (completeMountAutoFocusIfFocused()) {
          return;
        }

        if (!tryMountAutoFocusCandidate() && (document.activeElement === previous || document.activeElement === document.body)) {
          focusElement(scope.element, false);
        }

        completeMountAutoFocusIfFocused();
      };

      requestAnimationFrame(retryMountAutoFocus);
      setTimeout(retryMountAutoFocus, 0);
      setTimeout(retryMountAutoFocus, 50);
      setTimeout(retryMountAutoFocus, 100);
      setTimeout(retryMountAutoFocus, 250);
      setTimeout(retryMountAutoFocus, 500);
    }
  }
}

export function updateFocusScope(element, loop, trapped, preventMountAutoFocus, preventUnmountAutoFocus) {
  const handlers = focusScopeHandlers.get(element);

  if (!handlers) {
    return;
  }

  handlers.scope.loop = !!loop;
  handlers.scope.trapped = !!trapped;
  handlers.scope.preventMountAutoFocus = !!preventMountAutoFocus;
  handlers.scope.preventUnmountAutoFocus = !!preventUnmountAutoFocus;
}

export async function unregisterFocusScope(element, unmountAutoFocusPrevented = false) {
  const handlers = focusScopeHandlers.get(element);

  if (!handlers) {
    return;
  }

  document.removeEventListener("focusin", handlers.focusin);
  document.removeEventListener("focusout", handlers.focusout);
  element.removeEventListener("keydown", handlers.keydown);
  handlers.mutationObserver.disconnect();
  const { scope } = handlers;
  focusScopeHandlers.delete(element);

  setTimeout(() => {
    if (!scope.preventUnmountAutoFocus && !unmountAutoFocusPrevented) {
      focusElement(scope.previouslyFocusedElement || document.body, true);
    }

    removeFocusScopeFromStack(scope);
  }, 0);
}
