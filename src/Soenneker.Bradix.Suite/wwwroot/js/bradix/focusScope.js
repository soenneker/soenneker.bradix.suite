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
    lastFocusedElement: null,
    previouslyFocusedElement: document.activeElement instanceof HTMLElement ? document.activeElement : null
  };

  const focusin = (event) => {
    if (scope.paused || !scope.trapped) {
      return;
    }

    const target = event.target;
    if (scope.element.contains(target)) {
      scope.lastFocusedElement = target;
    } else {
      focusElement(scope.lastFocusedElement || scope.element, true);
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
      focusElement(scope.lastFocusedElement || scope.element, true);
    }
  };

  const mutationObserver = new MutationObserver((mutations) => {
    if (!scope.trapped) {
      return;
    }

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
  mutationObserver.observe(scope.element, { childList: true, subtree: true });

  addFocusScopeToStack(scope);

  const previous = scope.previouslyFocusedElement;
  const hasFocusedCandidate = previous && scope.element.contains(previous);
  if (!hasFocusedCandidate) {
    const mountAutoFocusPrevented = await dotNetRef.invokeMethodAsync("HandleMountAutoFocusAsync");
    if (!scope.preventMountAutoFocus && !mountAutoFocusPrevented) {
      focusFirst(removeLinks(getTabbableCandidates(scope.element)), true);
      if (document.activeElement === previous) {
        focusElement(scope.element, false);
      }
    }
  }

  focusScopeHandlers.set(element, { scope, focusin, focusout, keydown, mutationObserver });
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
