import { focusElement } from "./core/focus.js";

const sliderPointerHandlers = new WeakMap();
const contextMenuKeyboardHandlers = new WeakMap();

export function registerContextMenuKeyboard(element, dotNetRef = null) {
  if (!element) return;
  unregisterContextMenuKeyboard(element);
  const abort = new AbortController();
  let timer = 0;
  const clear = () => { clearTimeout(timer); timer = 0; };
  const owns = event => !element.hasAttribute("data-disabled") &&
    event.target?.closest?.("[data-bradix-context-menu-trigger]:not([data-disabled])") === element;
  element.setAttribute("data-bradix-context-menu-trigger", "");
  element.addEventListener("keydown", event => {
    if (!owns(event) || event.isComposing || event.keyCode === 229 || event.defaultPrevented || event.ctrlKey || event.altKey || event.metaKey) return;
    if (event.key !== "ContextMenu" && !(event.key === "F10" && event.shiftKey)) return;
    event.preventDefault();
    const rect = element.getBoundingClientRect();
    element.dispatchEvent(new MouseEvent("contextmenu", {
      bubbles: true, cancelable: true, button: 2, clientX: rect.left, clientY: rect.bottom
    }));
  }, { signal: abort.signal });
  if (dotNetRef) {
    const open = (x, y) => {
      clear();
      dotNetRef.invokeMethodAsync("HandleNativeContextMenu", x, y).catch(() => {});
    };
    element.addEventListener("contextmenu", event => {
      if (!owns(event) || event.defaultPrevented) return;
      event.preventDefault();
      event.stopPropagation();
      open(event.clientX, event.clientY);
    }, { signal: abort.signal });
    element.addEventListener("pointerdown", event => {
      if (!owns(event) || event.pointerType === "mouse" || event.defaultPrevented) return;
      event.stopPropagation();
      clear();
      timer = setTimeout(() => open(event.clientX, event.clientY), 700);
    }, { signal: abort.signal });
    for (const type of ["pointermove", "pointerup", "pointercancel"])
      element.addEventListener(type, clear, { signal: abort.signal, passive: true });
  }
  contextMenuKeyboardHandlers.set(element, { abort, clear });
}

export function unregisterContextMenuKeyboard(element) {
  const state = contextMenuKeyboardHandlers.get(element);
  state?.abort.abort();
  state?.clear();
  element?.removeAttribute("data-bradix-context-menu-trigger");
  contextMenuKeyboardHandlers.delete(element);
}
const oneTimePasswordInputHandlers = new WeakMap();
const selectBubbleInputHandlers = new WeakMap();
const selectBubbleInputSyncing = new WeakSet();

export function syncCheckboxBubbleInputState(element, isChecked, isIndeterminate, dispatchEvent, bubbles = true) {
  if (!element) {
    return;
  }

  const control = element.previousElementSibling instanceof HTMLElement ? element.previousElementSibling : null;

  const renderedChecked = control?.getAttribute("aria-checked");
  if (renderedChecked === "true" || renderedChecked === "false" || renderedChecked === "mixed") {
    // Interop may complete after a newer render, particularly during initial module loading.
    // Never let an older write undo the rendered state or uncheck the current native radio.
    const currentChecked = renderedChecked === "true";
    const currentIndeterminate = renderedChecked === "mixed";
    dispatchEvent = dispatchEvent && !!isChecked === currentChecked && !!isIndeterminate === currentIndeterminate;
    isChecked = currentChecked;
    isIndeterminate = currentIndeterminate;
  }

  if (control && element.form) {
    const rect = control.getBoundingClientRect();

    if (rect.width > 0) {
      element.style.width = `${rect.width}px`;
    }

    if (rect.height > 0) {
      element.style.height = `${rect.height}px`;
    }
  }

  const inputProto = window.HTMLInputElement.prototype;
  const descriptor = Object.getOwnPropertyDescriptor(inputProto, "checked");
  const setChecked = descriptor && typeof descriptor.set === "function" ? descriptor.set : null;

  element.indeterminate = !!isIndeterminate;

  if (setChecked) {
    setChecked.call(element, !!isChecked);
  } else {
    element.checked = !!isChecked;
  }

  if (dispatchEvent) {
    element.dispatchEvent(new Event("click", { bubbles: !!bubbles }));
  }
}

export function syncSliderBubbleInputValue(element, value, dispatchEvent) {
  if (!element) {
    return;
  }

  const inputProto = window.HTMLInputElement.prototype;
  const descriptor = Object.getOwnPropertyDescriptor(inputProto, "value");
  const setValue = descriptor && typeof descriptor.set === "function" ? descriptor.set : null;
  const nextValue = value == null ? "" : String(value);

  if (setValue) {
    setValue.call(element, nextValue);
  } else {
    element.value = nextValue;
  }

  if (dispatchEvent) {
    element.dispatchEvent(new Event("input", { bubbles: true }));
  }
}

export function syncSelectBubbleInputValue(element, value, dispatchEvent) {
  if (!element) {
    return;
  }

  const selectProto = window.HTMLSelectElement.prototype;
  const descriptor = Object.getOwnPropertyDescriptor(selectProto, "value");
  const setValue = descriptor && typeof descriptor.set === "function" ? descriptor.set : null;
  const nextValue = value == null ? "" : String(value);

  if (setValue) {
    setValue.call(element, nextValue);
  } else {
    element.value = nextValue;
  }

  if (dispatchEvent) {
    selectBubbleInputSyncing.add(element);

    try {
      element.dispatchEvent(new Event("change", { bubbles: true }));
    } finally {
      selectBubbleInputSyncing.delete(element);
    }
  }
}

export function registerSelectBubbleInput(element, dotNetRef) {
  if (!element || !dotNetRef) {
    return;
  }

  unregisterSelectBubbleInput(element);

  const change = () => {
    if (selectBubbleInputSyncing.has(element)) {
      return;
    }

    const value = element.value === "" ? null : element.value;
    dotNetRef.invokeMethodAsync("HandleNativeChangeFromScript", value).catch(() => {});
  };

  element.addEventListener("change", change);
  selectBubbleInputHandlers.set(element, { change });
}

export function unregisterSelectBubbleInput(element) {
  if (!element) {
    return;
  }

  const handlers = selectBubbleInputHandlers.get(element);

  if (!handlers) {
    return;
  }

  element.removeEventListener("change", handlers.change);
  selectBubbleInputHandlers.delete(element);
  selectBubbleInputSyncing.delete(element);
}

export function clickElement(element) {
  if (!element) {
    return;
  }

  element.click();
}

export function focusElementDeferred(element) {
  if (!element || typeof element.focus !== "function") {
    return;
  }

  const focus = () => {
    const activeElement = document.activeElement;

    if (activeElement === element || activeElement === document.body || activeElement == null) {
      focusElement(element, false);
    }
  };

  window.setTimeout(focus, 0);
  window.setTimeout(focus, 50);
  window.setTimeout(focus, 150);
}

export function selectInputText(element) {
  if (!element || typeof element.select !== "function") {
    return;
  }

  element.select();
}

export function syncInputValue(element, value) {
  if (!element) {
    return;
  }

  const next = value ?? "";

  if (element.value !== next) {
    element.value = next;
  }

  if (next === "") {
    element.removeAttribute("value");
  } else {
    element.setAttribute("value", next);
  }
}

export function isDirectionRtl(element) {
  if (!element || typeof window.getComputedStyle !== "function") {
    return false;
  }

  return window.getComputedStyle(element).direction === "rtl";
}

export function registerSliderPointerBridge(element, dotNetRef) {
  if (!element) {
    return;
  }

  unregisterSliderPointerBridge(element);
  let suppressClickUntil = 0;
  let resizeObserver = null;
  let mutationObserver = null;

  const updateThumbOffsets = () => {
    for (const thumb of element.querySelectorAll('[role="slider"]')) {
      const wrapper = thumb.parentElement;
      if (!wrapper) {
        continue;
      }

      const orientation = thumb.getAttribute("data-orientation") || "horizontal";
      const rect = thumb.getBoundingClientRect();
      const size = orientation === "vertical" ? rect.height : rect.width;
      const percent = Number.parseFloat(wrapper.style.getPropertyValue("--bradix-slider-thumb-percent"));
      const direction = Number.parseFloat(wrapper.style.getPropertyValue("--bradix-slider-thumb-direction")) || 1;

      if (!Number.isFinite(size) || size <= 0 || !Number.isFinite(percent)) {
        wrapper.style.setProperty("--bradix-slider-thumb-in-bounds-offset", "0px");
        continue;
      }

      const halfSize = size / 2;
      const offsetAtPercent = (percent / 50) * halfSize;
      const offset = (halfSize - offsetAtPercent * direction) * direction;
      wrapper.style.setProperty("--bradix-slider-thumb-in-bounds-offset", `${offset}px`);
    }
  };

  updateThumbOffsets();

  if (typeof ResizeObserver === "function") {
    resizeObserver = new ResizeObserver(updateThumbOffsets);
    resizeObserver.observe(element);
    for (const thumb of element.querySelectorAll('[role="slider"]')) {
      resizeObserver.observe(thumb);
    }
  }

  if (typeof MutationObserver === "function") {
    mutationObserver = new MutationObserver(updateThumbOffsets);
    mutationObserver.observe(element, {
      subtree: true,
      childList: true,
      attributes: true,
      attributeFilter: ["aria-valuenow", "data-orientation"]
    });
  }

  const getFractions = (event, rect = element.getBoundingClientRect(), result = {}) => {
    const x = rect.width <= 0 ? 0 : (event.clientX - rect.left) / rect.width;
    const y = rect.height <= 0 ? 0 : (event.clientY - rect.top) / rect.height;

    result.x = Math.min(1, Math.max(0, x));
    result.y = Math.min(1, Math.max(0, y));
    return result;
  };

  const getThumbIndex = (event) => {
    const thumb = event.target && typeof event.target.closest === "function"
      ? event.target.closest('[role="slider"]')
      : null;

    if (!thumb) {
      return -1;
    }

    let index = 0;
    for (const candidate of element.querySelectorAll('[role="slider"]')) {
      if (candidate === thumb) {
        return index;
      }

      index++;
    }

    return -1;
  };

  const pointerdown = (event) => {
    const handlers = sliderPointerHandlers.get(element);
    if (event.button !== 0 || !handlers || handlers.cancelGesture) {
      return;
    }

    suppressClickUntil = window.performance.now() + 500;
    event.preventDefault();
    if (typeof element.setPointerCapture === "function") {
      try {
        element.setPointerCapture(event.pointerId);
      } catch {
      }
    }

    const thumbIndex = getThumbIndex(event);
    const dragRect = element.getBoundingClientRect();
    const fractions = getFractions(event, dragRect);

    let moveFrame = 0;
    let moveTask = null;
    let pending = false;
    let ended = false;
    let canceled = false;
    const pendingMove = { x: 0, y: 0 };
    const isCurrent = () => !canceled && sliderPointerHandlers.get(element) === handlers;
    const cancelPendingMove = () => {
      if (moveFrame) {
        cancelAnimationFrame(moveFrame);
        moveFrame = 0;
      }
      pending = false;
    };
    const runMoveFrame = () => {
      moveFrame = 0;
      flushMove().catch(console.error);
    };
    const scheduleMove = () => {
      if (moveFrame || moveTask || ended || !isCurrent()) return;
      moveFrame = requestAnimationFrame(runMoveFrame);
    };
    const detachPointer = () => {
      document.removeEventListener("pointermove", pointermove);
      document.removeEventListener("pointerup", pointerup);
      document.removeEventListener("pointercancel", pointercancel);
      releasePointer(element, event.pointerId);
    };
    const cancelGesture = () => {
      if (canceled) return;
      canceled = true;
      ended = true;
      detachPointer();
      cancelPendingMove();
      if (handlers.cancelGesture === cancelGesture) handlers.cancelGesture = null;
    };
    const sendMove = async () => {
      if (!await startTask || !isCurrent() || !pending) return;
      pending = false;
      await dotNetRef.invokeMethodAsync("HandlePointerMove", pendingMove.x, pendingMove.y);
    };
    const completeMove = () => {
      moveTask = null;
      if (pending) scheduleMove();
    };
    const flushMove = () => {
      if (moveTask) return moveTask;
      moveTask = sendMove().finally(completeMove);
      return moveTask;
    };
    const pointermove = moveEvent => {
      if (moveEvent.pointerId !== event.pointerId || ended) return;
      getFractions(moveEvent, dragRect, pendingMove);
      pending = true;
      scheduleMove();
    };
    const finish = async (endEvent, cancel) => {
      if (endEvent.pointerId !== event.pointerId || ended) return;
      ended = true;
      suppressClickUntil = window.performance.now() + 500;
      detachPointer();
      cancelPendingMove();
      try {
        if (!await startTask || !isCurrent()) return;
        if (moveTask) await moveTask;
        if (!isCurrent()) return;
        if (!cancel) {
          getFractions(endEvent, dragRect, pendingMove);
          pending = true;
          await flushMove();
        }
        if (isCurrent()) {
          await dotNetRef.invokeMethodAsync(cancel ? "HandlePointerCancel" : "HandlePointerEnd");
        }
      } finally {
        cancelGesture();
      }
    };
    const pointerup = upEvent => finish(upEvent, false).catch(console.error);
    const pointercancel = cancelEvent => finish(cancelEvent, true).catch(console.error);

    // Observe release immediately, including while the server is still handling PointerStart.
    document.addEventListener("pointermove", pointermove);
    document.addEventListener("pointerup", pointerup);
    document.addEventListener("pointercancel", pointercancel);
    handlers.cancelGesture = cancelGesture;
    const startTask = Promise.resolve()
      .then(() => isCurrent() ? dotNetRef.invokeMethodAsync("HandlePointerStart", fractions.x, fractions.y, thumbIndex) : null)
      .then(() => true, error => { cancelGesture(); console.error(error); return false; });
  };

  const click = async (event) => {
    if (event.button !== 0) {
      return;
    }

    const handlers = sliderPointerHandlers.get(element);
    if (!handlers || handlers.cancelGesture || window.performance.now() < suppressClickUntil) {
      return;
    }

    const fractions = getFractions(event);
    const thumbIndex = getThumbIndex(event);
    await dotNetRef.invokeMethodAsync("HandlePointerStart", fractions.x, fractions.y, thumbIndex);
    if (sliderPointerHandlers.get(element) !== handlers) return;
    await dotNetRef.invokeMethodAsync("HandlePointerEnd");
  };

  sliderPointerHandlers.set(element, { pointerdown, click, cancelGesture: null, resizeObserver, mutationObserver });
  element.addEventListener("pointerdown", pointerdown);
  element.addEventListener("click", click);
}

export function unregisterSliderPointerBridge(element) {
  const handlers = sliderPointerHandlers.get(element);

  if (!handlers) {
    return;
  }

  element.removeEventListener("pointerdown", handlers.pointerdown);
  element.removeEventListener("click", handlers.click);

  handlers.cancelGesture?.();

  if (handlers.resizeObserver) {
    handlers.resizeObserver.disconnect();
  }

  if (handlers.mutationObserver) {
    handlers.mutationObserver.disconnect();
  }

  sliderPointerHandlers.delete(element);
}

export function capturePointer(element, pointerId) {
  if (!element || typeof element.setPointerCapture !== "function") {
    return;
  }

  try {
    element.setPointerCapture(pointerId);
  } catch {
  }
}

export function releasePointer(element, pointerId) {
  if (!element || typeof element.hasPointerCapture !== "function" || typeof element.releasePointerCapture !== "function") {
    return;
  }

  try {
    if (element.hasPointerCapture(pointerId)) {
      element.releasePointerCapture(pointerId);
    }
  } catch {
  }
}

export function suppressNextClick(element) {
  if (!element) {
    return;
  }

  element.addEventListener("click", (event) => {
    event.preventDefault();
  }, { once: true });
}

export function focusElementById(elementId) {
  if (!elementId) {
    return;
  }

  focusElement(document.getElementById(elementId), false);
}

export function focusElementByIdDeferred(elementId) {
  if (!elementId) {
    return;
  }

  const focus = () => {
    const element = document.getElementById(elementId);
    const activeElement = document.activeElement;

    if (activeElement === element || activeElement === document.body || activeElement == null) {
      focusElement(element, false);
    }
  };

  setTimeout(focus, 0);
  setTimeout(focus, 50);
  setTimeout(focus, 150);
}

export function focusElementPreventScroll(element) {
  focusElement(element, false);
}

export async function focusFirstMatchingDescendant(element, selector) {
  if (!element || !selector) {
    return false;
  }

  const tryFocus = () => {
    const target = element.querySelector(selector);
    if (!(target instanceof HTMLElement)) {
      return false;
    }

    focusElement(target, false);
    return document.activeElement === target;
  };

  for (let attempt = 0; attempt < 5; attempt++) {
    if (tryFocus()) {
      return true;
    }

    await new Promise((resolve) => requestAnimationFrame(resolve));
  }

  return false;
}

export function scrollElementIntoViewNearest(element) {
  if (!element || typeof element.scrollIntoView !== "function") {
    return;
  }

  element.scrollIntoView({ block: "nearest" });
}

export function registerOneTimePasswordInput(element, dotNetRef) {
  if (!element) {
    return;
  }

  unregisterOneTimePasswordInput(element);

  const focusBoundaryInput = (start) => {
      const root = element.closest('[role="group"]');
      const inputs = (root || document).querySelectorAll("[data-radix-otp-input]");
      let next = null;

      if (start) {
        for (const input of inputs) {
          if (!input.disabled && !input.readOnly) {
            next = input;
            break;
          }
        }
      } else {
        for (let index = inputs.length - 1; index >= 0; index--) {
          const input = inputs[index];
          if (!input.disabled && !input.readOnly) {
            next = input;
            break;
          }
        }
      }

      if (next && typeof next.focus === "function") {
        next.focus({ preventScroll: true });
        if (typeof next.select === "function") {
          next.select();
        }
      }
  };

  const keydown = async (event) => {
  if (event.isComposing || event.keyCode === 229) return;
    if (event.target !== element) {
      return;
    }
    switch (event.key) {
      case "Home":
      case "End":
        event.preventDefault();
        focusBoundaryInput(event.key === "Home");
        if (dotNetRef) {
          await dotNetRef.invokeMethodAsync(
            "HandleManagedKeyDown",
            event.key,
            event.metaKey,
            event.ctrlKey,
            event.altKey,
            event.shiftKey
          );
        }
        return;
      case "Backspace":
      case "Delete":
      case "Clear":
      case "Enter":
      case "ArrowLeft":
      case "ArrowRight":
      case "ArrowUp":
      case "ArrowDown":
        event.preventDefault();
        if (dotNetRef) {
          await dotNetRef.invokeMethodAsync(
            "HandleManagedKeyDown",
            event.key,
            event.metaKey,
            event.ctrlKey,
            event.altKey,
            event.shiftKey
          );
        }
        return;
      default:
        return;
    }
  };

  const paste = (event) => {
    if (!dotNetRef) {
      return;
    }

    event.preventDefault();
    dotNetRef.invokeMethodAsync("HandlePaste", event.clipboardData?.getData("Text") || "");
  };

  const focus = () => {
    dotNetRef?.invokeMethodAsync("HandleManagedFocus").catch(() => {});
  };

  const blur = () => {
    dotNetRef?.invokeMethodAsync("HandleManagedBlur").catch(() => {});
  };

  element.addEventListener("keydown", keydown);
  element.addEventListener("paste", paste);
  element.addEventListener("focus", focus);
  element.addEventListener("blur", blur);
  oneTimePasswordInputHandlers.set(element, { keydown, paste, focus, blur });
  dotNetRef?.invokeMethodAsync("HandleManagedKeyBridgeReady").catch(() => {});
}

export function unregisterOneTimePasswordInput(element) {
  const handlers = oneTimePasswordInputHandlers.get(element);

  if (!handlers) {
    return;
  }

  element.removeEventListener("keydown", handlers.keydown);
  element.removeEventListener("paste", handlers.paste);
  element.removeEventListener("focus", handlers.focus);
  element.removeEventListener("blur", handlers.blur);
  oneTimePasswordInputHandlers.delete(element);
}

// Password values never cross this bridge. Only visibility changes notify Blazor.
const passwordInputs = new WeakMap();
const passwordToggles = new WeakMap();

export function registerPasswordInput(input, dotNetRef) {
  unregisterPasswordInput(input);
  const abort = new AbortController();
  const state = { abort, start: null, end: null, direction: null, restoreSelection: false };
  input.dataset.bradixPasswordReady = "true";
  passwordInputs.set(input, state);
  state.observer = new MutationObserver(() => {
    if (!state.restoreSelection) return;
    state.restoreSelection = false;
    if (input.ownerDocument.activeElement === input && state.start !== null)
      input.setSelectionRange(state.start, state.end, state.direction);
  });
  state.observer.observe(input, { attributes: true, attributeFilter: ["type"] });
  input.addEventListener("blur", () => {
    state.start = input.selectionStart;
    state.end = input.selectionEnd;
    state.direction = input.selectionDirection;
  }, { signal: abort.signal });
  const hide = event => {
    if (event.type === "reset" && event.defaultPrevented) return;
    // Synchronous masking precedes native submission, including on Blazor Server.
    input.type = "password";
    dotNetRef.invokeMethodAsync("HidePassword").catch(() => {});
  };
  input.form?.addEventListener("submit", hide, { signal: abort.signal });
  input.form?.addEventListener("reset", event => queueMicrotask(() => {
    if (!abort.signal.aborted) hide(event);
  }), { signal: abort.signal });
}

export function unregisterPasswordInput(input) {
  passwordInputs.get(input)?.abort.abort();
  passwordInputs.get(input)?.observer.disconnect();
  passwordInputs.delete(input);
  input?.removeAttribute("data-bradix-password-ready");
}

export function registerPasswordToggle(button) {
  unregisterPasswordToggle(button);
  const abort = new AbortController();
  let pointer = false;
  let automaticLabel = null;
  const label = () => {
    const current = button.getAttribute("aria-label");
    if (current !== null && current !== automaticLabel) return;
    if (button.textContent?.trim()) {
      if (automaticLabel !== null) button.removeAttribute("aria-label");
      automaticLabel = null;
    } else {
      automaticLabel = button.dataset.state === "visible" ? "Hide password" : "Show password";
      if (current !== automaticLabel) button.setAttribute("aria-label", automaticLabel);
    }
  };
  const observer = new MutationObserver(label);
  observer.observe(button, { childList: true, characterData: true, subtree: true, attributes: true, attributeFilter: ["data-state", "aria-label"] });
  label();
  button.addEventListener("pointerdown", () => { pointer = true; }, { signal: abort.signal });
  button.addEventListener("pointercancel", () => { pointer = false; }, { signal: abort.signal });
  button.addEventListener("keydown", () => { pointer = false; }, { signal: abort.signal });
  button.addEventListener("click", event => {
    const restore = pointer;
    pointer = false;
    if (!restore || event.defaultPrevented || button.disabled) return;
    const input = button.ownerDocument.getElementById(button.getAttribute("aria-controls"));
    const state = input && passwordInputs.get(input);
    if (!state) return;
    state.restoreSelection = true;
    input.focus({ preventScroll: true });
    if (state.start !== null) {
      const { start, end, direction } = state;
      input.ownerDocument.defaultView.requestAnimationFrame(() => {
        if (!abort.signal.aborted && input.ownerDocument.activeElement === input)
          input.setSelectionRange(start, end, direction);
      });
    }
  }, { signal: abort.signal });
  passwordToggles.set(button, { abort, observer });
}

export function unregisterPasswordToggle(button) {
  const state = passwordToggles.get(button);
  state?.abort.abort();
  state?.observer.disconnect();
  passwordToggles.delete(button);
}
