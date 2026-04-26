import { focusElement } from "./core/focus.js";

const sliderPointerHandlers = new WeakMap();
const oneTimePasswordInputHandlers = new WeakMap();
const selectBubbleInputHandlers = new WeakMap();
const selectBubbleInputSyncing = new WeakSet();

export function syncCheckboxBubbleInputState(element, isChecked, isIndeterminate, dispatchEvent, bubbles = true) {
  if (!element) {
    return;
  }

  const control = element.previousElementSibling instanceof HTMLElement ? element.previousElementSibling : null;

  if (control) {
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

  window.setTimeout(() => {
    element.focus();
  }, 0);
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
    const sliderThumbs = Array.from(element.querySelectorAll('[role="slider"]'));

    sliderThumbs.forEach((thumb) => {
      const wrapper = thumb.parentElement;
      if (!wrapper) {
        return;
      }

      const orientation = thumb.getAttribute("data-orientation") || "horizontal";
      const rect = thumb.getBoundingClientRect();
      const size = orientation === "vertical" ? rect.height : rect.width;
      const percent = Number.parseFloat(wrapper.style.getPropertyValue("--bradix-slider-thumb-percent"));
      const direction = Number.parseFloat(wrapper.style.getPropertyValue("--bradix-slider-thumb-direction")) || 1;

      if (!Number.isFinite(size) || size <= 0 || !Number.isFinite(percent)) {
        wrapper.style.setProperty("--bradix-slider-thumb-in-bounds-offset", "0px");
        return;
      }

      const halfSize = size / 2;
      const offsetAtPercent = (percent / 50) * halfSize;
      const offset = (halfSize - offsetAtPercent * direction) * direction;
      wrapper.style.setProperty("--bradix-slider-thumb-in-bounds-offset", `${offset}px`);
    });
  };

  updateThumbOffsets();

  if (typeof ResizeObserver === "function") {
    resizeObserver = new ResizeObserver(updateThumbOffsets);
    resizeObserver.observe(element);
    Array.from(element.querySelectorAll('[role="slider"]')).forEach((thumb) => resizeObserver.observe(thumb));
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

  const getFractions = (event) => {
    const rect = element.getBoundingClientRect();
    const x = rect.width <= 0 ? 0 : (event.clientX - rect.left) / rect.width;
    const y = rect.height <= 0 ? 0 : (event.clientY - rect.top) / rect.height;

    return {
      x: Math.min(1, Math.max(0, x)),
      y: Math.min(1, Math.max(0, y))
    };
  };

  const getThumbIndex = (event) => {
    const thumb = event.target && typeof event.target.closest === "function"
      ? event.target.closest('[role="slider"]')
      : null;
    const sliderThumbs = Array.from(element.querySelectorAll('[role="slider"]'));
    return thumb ? sliderThumbs.indexOf(thumb) : -1;
  };

  const pointerdown = async (event) => {
    if (event.button !== 0) {
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
    const fractions = getFractions(event);

    await dotNetRef.invokeMethodAsync("HandlePointerStart", fractions.x, fractions.y, thumbIndex);

    const pointermove = async (moveEvent) => {
      const moveFractions = getFractions(moveEvent);
      await dotNetRef.invokeMethodAsync("HandlePointerMove", moveFractions.x, moveFractions.y);
    };

    const pointerup = async () => {
      document.removeEventListener("pointermove", pointermove);
      document.removeEventListener("pointerup", pointerup);
      document.removeEventListener("pointercancel", pointercancel);
      if (typeof element.hasPointerCapture === "function" &&
        typeof element.releasePointerCapture === "function") {
        try {
          if (element.hasPointerCapture(event.pointerId)) {
            element.releasePointerCapture(event.pointerId);
          }
        } catch {
        }
      }
      await dotNetRef.invokeMethodAsync("HandlePointerEnd");
    };

    const pointercancel = async () => {
      document.removeEventListener("pointermove", pointermove);
      document.removeEventListener("pointerup", pointerup);
      document.removeEventListener("pointercancel", pointercancel);
      if (typeof element.hasPointerCapture === "function" &&
        typeof element.releasePointerCapture === "function") {
        try {
          if (element.hasPointerCapture(event.pointerId)) {
            element.releasePointerCapture(event.pointerId);
          }
        } catch {
        }
      }
      await dotNetRef.invokeMethodAsync("HandlePointerCancel");
    };

    document.addEventListener("pointermove", pointermove);
    document.addEventListener("pointerup", pointerup);
    document.addEventListener("pointercancel", pointercancel);

    const handlers = sliderPointerHandlers.get(element);
    if (handlers) {
      handlers.pointermove = pointermove;
      handlers.pointerup = pointerup;
      handlers.pointercancel = pointercancel;
    }
  };

  const click = async (event) => {
    if (event.button !== 0) {
      return;
    }

    if (window.performance.now() < suppressClickUntil) {
      return;
    }

    const fractions = getFractions(event);
    const thumbIndex = getThumbIndex(event);
    await dotNetRef.invokeMethodAsync("HandlePointerStart", fractions.x, fractions.y, thumbIndex);
    await dotNetRef.invokeMethodAsync("HandlePointerEnd");
  };

  sliderPointerHandlers.set(element, { pointerdown, click, pointermove: null, pointerup: null, pointercancel: null, resizeObserver, mutationObserver });
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

  if (handlers.pointermove) {
    document.removeEventListener("pointermove", handlers.pointermove);
  }

  if (handlers.pointerup) {
    document.removeEventListener("pointerup", handlers.pointerup);
  }

  if (handlers.pointercancel) {
    document.removeEventListener("pointercancel", handlers.pointercancel);
  }

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

    await new Promise((resolve) => requestAnimationFrame(() => resolve()));
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

  const keydown = async (event) => {
    if (event.target !== element) {
      return;
    }

    const focusBoundaryInput = (start) => {
      const root = element.closest('[role="group"]');
      const inputs = Array.from((root || document).querySelectorAll("[data-radix-otp-input]"))
        .filter((input) => !input.disabled && !input.readOnly);
      const next = start ? inputs[0] : inputs[inputs.length - 1];

      if (next && typeof next.focus === "function") {
        next.focus({ preventScroll: true });
        if (typeof next.select === "function") {
          next.select();
        }
      }
    };

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
