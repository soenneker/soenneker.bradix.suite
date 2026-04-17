import {
  createDismissablePointerSnapshot,
  createDismissableKeyboardSnapshot,
  createDismissableFocusSnapshot
} from "./core/eventSnapshots.js";

const dismissableBranches = new Set();
const dismissableLayers = [];
let dismissableLayerListenersRegistered = false;
let dismissableLayerPointerDownListenerRegistered = false;
let originalDismissableBodyPointerEvents = "";
let hasStoredDismissableBodyPointerEvents = false;

function invokeDotNetSafely(dotNetRef, methodName, ...args) {
  try {
    const invocation = dotNetRef?.invokeMethodAsync?.(methodName, ...args);
    if (invocation && typeof invocation.catch === "function") {
      return invocation.catch(() => undefined);
    }
    return Promise.resolve(invocation);
  } catch {
    return Promise.resolve(undefined);
  }
}

function isRegisteredLayer(layer) {
  return !!layer && dismissableLayers.includes(layer);
}

function getTopmostLayerForTarget(target) {
  if (!target) {
    return dismissableLayers[dismissableLayers.length - 1];
  }

  for (let index = dismissableLayers.length - 1; index >= 0; index -= 1) {
    const layer = dismissableLayers[index];

    if (layer?.element?.contains?.(target)) {
      return layer;
    }
  }

  return dismissableLayers[dismissableLayers.length - 1];
}

function isBranchTarget(target) {
  if (!target) {
    return false;
  }

  for (const branch of dismissableBranches) {
    if (branch.contains(target)) {
      return true;
    }
  }

  return false;
}

function updateDismissableLayerPointerEvents() {
  const highestDisabledIndex = [...dismissableLayers].map((layer) => layer.disableOutsidePointerEvents).lastIndexOf(true);

  if (highestDisabledIndex >= 0) {
    if (!hasStoredDismissableBodyPointerEvents) {
      originalDismissableBodyPointerEvents = document.body.style.pointerEvents || "";
      hasStoredDismissableBodyPointerEvents = true;
    }

    document.body.style.pointerEvents = "none";
  } else if (hasStoredDismissableBodyPointerEvents) {
    document.body.style.pointerEvents = originalDismissableBodyPointerEvents;
    originalDismissableBodyPointerEvents = "";
    hasStoredDismissableBodyPointerEvents = false;
  } else {
    document.body.style.pointerEvents = "";
  }

  dismissableLayers.forEach((layer, index) => {
    if (highestDisabledIndex >= 0) {
      layer.element.style.pointerEvents = index >= highestDisabledIndex ? "auto" : "none";
    } else {
      layer.element.style.pointerEvents = "";
    }
  });

  dismissableBranches.forEach((branch) => {
    branch.style.pointerEvents = highestDisabledIndex >= 0 ? "auto" : "";
  });
}

function ensureDismissableLayerListeners() {
  if (dismissableLayerListenersRegistered) {
    return;
  }

  if (!dismissableLayerPointerDownListenerRegistered) {
    const dispatchPointerDownOutside = (topLayer, event) => {
      if (!isRegisteredLayer(topLayer)) {
        return;
      }

      const snapshot = createDismissablePointerSnapshot(event);
      snapshot.activeElementInsideLayer = !!(document.activeElement && topLayer.element.contains(document.activeElement));
      invokeDotNetSafely(topLayer.dotNetRef, "HandlePointerDownOutside", snapshot);
    };

    document.addEventListener("pointerdown", (event) => {
      const topLayer = dismissableLayers[dismissableLayers.length - 1];

      if (!topLayer || !event.target) {
        return;
      }

      // Match Radix more closely by only enabling outside pointer handling
      // after the gesture that mounted the layer has fully completed.
      if (!topLayer.isPointerDownOutsideEnabled) {
        topLayer.isPointerInside = false;
        return;
      }

      topLayer.lastPointerDownWasOutside = false;

      if (!topLayer.isPointerInside) {
        if (isBranchTarget(event.target)) {
          topLayer.isPointerInside = false;
          return;
        }

        if (event.pointerType === "touch") {
          if (topLayer.handleDocumentClick) {
            document.removeEventListener("click", topLayer.handleDocumentClick);
          }

          const originalPointerEvent = event;
          topLayer.handleDocumentClick = () => {
            if (!isRegisteredLayer(topLayer) || dismissableLayers[dismissableLayers.length - 1] !== topLayer) {
              return;
            }

            dispatchPointerDownOutside(topLayer, originalPointerEvent);
          };
          document.addEventListener("click", topLayer.handleDocumentClick, { once: true });
        } else {
          if (topLayer.handleDocumentClick) {
            document.removeEventListener("click", topLayer.handleDocumentClick);
            topLayer.handleDocumentClick = null;
          }

          topLayer.lastPointerDownWasOutside = true;
          dispatchPointerDownOutside(topLayer, event);
        }
      } else {
        if (topLayer.handleDocumentClick) {
          document.removeEventListener("click", topLayer.handleDocumentClick);
        }
      }

      topLayer.isPointerInside = false;
      if (event.pointerType !== "touch") {
        topLayer.handleDocumentClick = null;
      }
    });

    document.addEventListener("click", (event) => {
      const topLayer = dismissableLayers[dismissableLayers.length - 1];

      if (!topLayer || !event.target) {
        return;
      }

      if (!topLayer.isPointerDownOutsideEnabled) {
        return;
      }

      if (topLayer.lastPointerDownWasOutside) {
        topLayer.lastPointerDownWasOutside = false;
        return;
      }

      if (topLayer.element.contains(event.target)) {
        return;
      }

      if (isBranchTarget(event.target)) {
        return;
      }

      dispatchPointerDownOutside(topLayer, event);
    });

    dismissableLayerPointerDownListenerRegistered = true;
  }

  document.addEventListener("focusin", (event) => {
    const topLayer = dismissableLayers[dismissableLayers.length - 1];

    if (!topLayer || !event.target) {
      return;
    }

    if (!topLayer.isFocusOutsideEnabled) {
      return;
    }

    if (topLayer.isFocusInside) {
      return;
    }

    if (isBranchTarget(event.target)) {
      return;
    }

    invokeDotNetSafely(topLayer.dotNetRef, "HandleFocusOutside", createDismissableFocusSnapshot(event));
  });

  document.addEventListener("keydown", (event) => {
    if (event.key !== "Escape") {
      return;
    }

    const topLayer = getTopmostLayerForTarget(event.target);
    if (!topLayer) {
      return;
    }

    invokeDotNetSafely(topLayer.dotNetRef, "HandleEscapeKeyDown", createDismissableKeyboardSnapshot(event)).then((shouldPreventDefault) => {
      if (shouldPreventDefault) {
        event.preventDefault();
      }
    });
  }, true);

  dismissableLayerListenersRegistered = true;
}

export function registerDismissableLayer(element, dotNetRef, disableOutsidePointerEvents) {
  if (!element) {
    return;
  }

  unregisterDismissableLayer(element);
  ensureDismissableLayerListeners();

  const handlePointerDownCapture = () => {
    const layer = dismissableLayers.find((item) => item.element === element);
    if (layer) {
      layer.isPointerInside = true;
    }
  };
  const handleFocusInCapture = () => {
    const layer = dismissableLayers.find((item) => item.element === element);
    if (layer) {
      layer.isFocusInside = true;
    }
  };
  const handleFocusOutCapture = () => {
    const layer = dismissableLayers.find((item) => item.element === element);
    if (layer) {
      layer.isFocusInside = false;
    }
  };

  element.addEventListener("pointerdown", handlePointerDownCapture, true);
  element.addEventListener("focusin", handleFocusInCapture, true);
  element.addEventListener("focusout", handleFocusOutCapture, true);

  dismissableLayers.push({
    element,
    dotNetRef,
    disableOutsidePointerEvents: !!disableOutsidePointerEvents,
    lastPointerDownWasOutside: false,
    isPointerInside: false,
    isFocusInside: false,
    isPointerDownOutsideEnabled: false,
    isFocusOutsideEnabled: false,
    handlePointerDownCapture,
    handleFocusInCapture,
    handleFocusOutCapture,
    handleDocumentClick: null,
    pointerDownOutsideTimer: window.setTimeout(() => {
      const layer = dismissableLayers.find((item) => item.element === element);
      if (layer) {
        layer.isPointerDownOutsideEnabled = true;
      }
    }, 0),
    focusOutsideTimer: window.setTimeout(() => {
      const layer = dismissableLayers.find((item) => item.element === element);
      if (layer) {
        layer.isFocusOutsideEnabled = true;
      }
    }, 0)
  });
  updateDismissableLayerPointerEvents();
}

export function updateDismissableLayer(element, disableOutsidePointerEvents) {
  const layer = dismissableLayers.find((item) => item.element === element);

  if (!layer) {
    return;
  }

  layer.disableOutsidePointerEvents = !!disableOutsidePointerEvents;
  updateDismissableLayerPointerEvents();
}

export function unregisterDismissableLayer(element) {
  const index = dismissableLayers.findIndex((item) => item.element === element);

  if (index < 0) {
    return;
  }

  const [layer] = dismissableLayers.splice(index, 1);

  if (layer?.handleDocumentClick) {
    document.removeEventListener("click", layer.handleDocumentClick);
  }

  if (typeof layer?.pointerDownOutsideTimer === "number") {
    clearTimeout(layer.pointerDownOutsideTimer);
  }

  if (typeof layer?.focusOutsideTimer === "number") {
    clearTimeout(layer.focusOutsideTimer);
  }

  if (element) {
    element.removeEventListener("pointerdown", layer.handlePointerDownCapture, true);
    element.removeEventListener("focusin", layer.handleFocusInCapture, true);
    element.removeEventListener("focusout", layer.handleFocusOutCapture, true);
    element.style.pointerEvents = "";
  }

  updateDismissableLayerPointerEvents();
}

export function registerDismissableLayerBranch(element) {
  if (!element) {
    return;
  }

  dismissableBranches.add(element);
}

export function unregisterDismissableLayerBranch(element) {
  if (!element) {
    return;
  }

  dismissableBranches.delete(element);
}
